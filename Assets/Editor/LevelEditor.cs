using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    private Level level;
    private GameObject[] tiles, monsters;
    private string[] tileNames, monsterNames, categories = {"None", "Tiles", "Monsters"};
    private string[] waves = new string[] {"Wave1","Wave2","Wave3","Wave4"};

    private int category = 0, preCate = 0, index = 0, degree = 0, wave = 1;
    private int[] value = new int[] { 1, 2, 3, 4 };
    private GameObject selectedPrefab;

    private void OnEnable()
    {
        level = (Level)target;
        category = 0;
        preCate = 0;
        index = 0;
        degree = 0;
        wave = 1;
        selectedPrefab = null;
        tiles = Resources.LoadAll<GameObject>("Prefab(LE)/Tiles");
        monsters = Resources.LoadAll<GameObject>("Prefab(LE)/Monsters");
        tileNames = new string[tiles.Length + 1];
        monsterNames = new string[monsters.Length + 1];
        tileNames[0] = "None";
        monsterNames[0] = "None";
        for (int i = 0; i < tiles.Length; i++)
            tileNames[i + 1] = tiles[i].name;
        for (int i = 0; i < monsters.Length; i++)
            monsterNames[i + 1] = monsters[i].name;
    }

    public void OnSceneGUI()
    {
        level = (Level)target;
        
        float halfX = ((float)level.X) * 2f;
        float halfY = ((float)level.Y) * 2f;
        float posX = level.transform.position.x;
        float posY = level.transform.position.y;
        DrawRect(posX, posY, halfX, halfY, Color.red);
        /*此段以上負責畫關卡邊界*/

        GUILayout.BeginArea(new Rect(10, 10, 400, 200));
        GUILayout.BeginHorizontal();
        category = EditorGUILayout.Popup(category, categories);
        if (preCate != category)
        {
            index = 0;
            preCate = category;
        }
        switch (category)
        {
            case 1:
                index = EditorGUILayout.Popup(index, tileNames);
                selectedPrefab = ((index > 0) ? tiles[index - 1] : null);
                break;
            case 2:
                index = EditorGUILayout.Popup(index, monsterNames);
                selectedPrefab = ((index > 0) ? monsters[index - 1] : null);
                break;
            default:
                break;
        }
        GUILayout.EndHorizontal();
        string boxstr = "Level Edit Mode : " + ((selectedPrefab == null) ? "No prefab selected" : selectedPrefab.name);
        GUILayout.Box(boxstr);
        if(category == 1)
        {
            string rotatestr = "Rotation : " + degree;
            GUILayout.Box(rotatestr);
        }
        else if(category == 2)
            wave = EditorGUILayout.IntPopup("Wave", wave, waves, value);
        GUILayout.EndArea();
        /*此段以上負責生成SceneGUI*/

        level.map.transform.localScale = new Vector3(4 * level.X, 4 * level.Y);
        Vector3 spawnPosition = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
        Vector3 gridPosition = new Vector3(Mathf.Floor(spawnPosition.x) + 0.5f, Mathf.Floor(spawnPosition.y) + 0.5f, 0);

        GameObject inLayer; Transform pointed = null;
        if(category != 0)
        {
            if (category == 1)
                inLayer = level.obstacle;
            else
                inLayer = level.enemy;
            foreach (Transform obj in inLayer.transform)
            {
                if (obj.position == gridPosition)
                {
                    pointed = obj;
                    break;
                }
            }
        }

        if (category == 1 && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Z)
            degree = (degree == 0 ? 270 : degree - 90);
        if (category == 1 && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.C)
            degree = (degree == 270 ? 0 : degree + 90);

        if (pointed == null && Event.current.type == EventType.MouseDown && Event.current.button == 0 && selectedPrefab != null) //滑鼠左鍵點擊生成
        {
            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
            newObject.transform.position = gridPosition;
            if (category == 1)
            {
                newObject.transform.SetParent(level.obstacle.transform);
                newObject.transform.rotation = Quaternion.Euler(0, 0, degree);
            }
            else if (category == 2)
            {
                newObject.transform.SetParent(level.enemy.transform);
                switch (wave)
                {
                    case 1:
                        level.waveOne.Add(newObject.GetComponent<Enemy>());
                        break;
                    case 2:
                        level.waveTwo.Add(newObject.GetComponent<Enemy>());
                        break;
                    case 3:
                        level.waveThree.Add(newObject.GetComponent<Enemy>());
                        break;
                    case 4:
                        level.waveFour.Add(newObject.GetComponent<Enemy>());
                        break;
                    default:
                        break;
                }
            }
        }
        else if (Event.current.type == EventType.MouseDown && Event.current.button == 1) //滑鼠右鍵點擊退出生成
        {
            index = 0;
            selectedPrefab = null;
        }
        else if (pointed != null && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.X) //按X把滑鼠指到的東西刪掉(如果有指到東西的話)
        {
            if(category == 2)
            {
                switch (wave)
                {
                    case 1:
                        if (level.waveOne.Exists(e => e = pointed.gameObject.GetComponent<Enemy>()))
                        {
                            level.waveOne.Remove(pointed.gameObject.GetComponent<Enemy>());
                            DestroyImmediate(pointed.gameObject);
                        }
                        break;
                    case 2:
                        if (level.waveTwo.Exists(e => e = pointed.gameObject.GetComponent<Enemy>()))
                        {
                            level.waveTwo.Remove(pointed.gameObject.GetComponent<Enemy>());
                            DestroyImmediate(pointed.gameObject);
                        }
                        break;
                    case 3:
                        if (level.waveThree.Exists(e => e = pointed.gameObject.GetComponent<Enemy>()))
                        {
                            level.waveThree.Remove(pointed.gameObject.GetComponent<Enemy>());
                            DestroyImmediate(pointed.gameObject);
                        }
                        break;
                    case 4:
                        if (level.waveFour.Exists(e => e = pointed.gameObject.GetComponent<Enemy>()))
                        {
                            level.waveFour.Remove(pointed.gameObject.GetComponent<Enemy>());
                            DestroyImmediate(pointed.gameObject);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        if(category == 2)
        {
            List<Enemy> newList = new List<Enemy>();
            switch (wave)
            {
                case 1:
                    newList = level.waveOne;
                    break;
                case 2:
                    newList = level.waveTwo;
                    break;
                case 3:
                    newList = level.waveThree;
                    break;
                case 4:
                    newList = level.waveFour;
                    break;
                default:
                    break;
            }
            if (newList.Count != 0)
            {
                for (int i = 0; i < newList.Count; i++)
                    DrawCircle(newList[i].gameObject.transform.position);
            }
        }
            
            
        /*此段以上負責在Scene內生成Prefab*/

        SceneView.RepaintAll();
    }

    private void OnDisable()
    {
        level = (Level)target;
        EditorUtility.SetDirty(level.gameObject);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        level = (Level)target;
    }

    void DrawRect(float posX, float posY, float halfX, float halfY, Color color)
    {
        Vector3[] verts = new Vector3[]
        {
            new Vector3(posX - halfX, posY + halfY, 0),
            new Vector3(posX - halfX, posY - halfY, 0),
            new Vector3(posX + halfX, posY - halfY, 0),
            new Vector3(posX + halfX, posY + halfY, 0)
        };
        Handles.DrawSolidRectangleWithOutline(verts, new Color(color.r, color.g, color.b, 0.1f), color);
    }

    void DrawCircle(Vector2 pos)
    {
        Handles.DrawSolidDisc(pos, Vector3.back, 0.35f);
    }
}
