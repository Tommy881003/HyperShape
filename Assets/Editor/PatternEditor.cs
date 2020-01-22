using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BulletCreator;

public enum BulletType
{
    none = 0,
    pistol = 1,
    rifle = 2,
    shotgun = 3,
    laser = 4,
    enemies = 5
}

[CustomEditor(typeof(BulletPattern))]
public class PatternEditor : Editor
{
    private int degree;
    private int bCount;
    private int eCount;
    private float startRotation;
    private Vector2 position;
    private float radius;
    [SerializeField]
    private Vector2 from, to;

    private readonly BulletArray bulletAR = BulletArray.instance;
    private Dictionary<GameObject, SpawnData> dict;
    private List<GameObject> list;
    public Stack<List<SpawnData>> undoStack;
    private BulletType index = 0;
    private int bulletNum = 0;
    private GameObject selectedPrefab;
    private string[] P, R, S, L, E;
    private bool showCircle, showPolygon, showLine;
    BulletPattern Inspecting;

    private void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
        index = 0;
        Inspecting = (BulletPattern)target;
        undoStack = new Stack<List<SpawnData>>();
        List<SpawnData> newlist = new List<SpawnData>();
        foreach (SpawnData data in Inspecting.spawns)
            newlist.Add(data);
        undoStack.Push(newlist);
        dict = new Dictionary<GameObject, SpawnData>();
        list = new List<GameObject>();
        selectedPrefab = null;
        showCircle = false; showPolygon = false; showLine = false;
        degree = 0;
        DrawGO();
        E = new string[bulletAR.enemies.Count + 1];
        E[0] = "None";
        for (int i = 0; i < bulletAR.enemies.Count; i++)
            E[i + 1] = bulletAR.enemies[i].name;
    }

    private void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        selectedPrefab = null;
        ClearGO();
        undoStack.Clear();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.BeginHorizontal();
        index = (BulletType)EditorGUILayout.EnumPopup("Bullet Type : ", index);
        switch (index)
        {
            case BulletType.none:
                selectedPrefab = null;
                break;
            case BulletType.enemies:
                bulletNum = EditorGUILayout.Popup(bulletNum, E);
                selectedPrefab = bulletNum > 0 ? bulletAR.enemies[bulletNum - 1].gameObject : null;
                break;
            default:
                Debug.LogWarning("Undefined category.");
                selectedPrefab = null;
                break;
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        bCount = EditorGUILayout.IntSlider("Bullets", bCount, 1, 60);
        eCount = EditorGUILayout.IntSlider("Edges", eCount, 3, 8);
        radius = EditorGUILayout.Slider("Radius",radius, 1, 10);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        startRotation = EditorGUILayout.Slider("Rotation", startRotation, 0, 359);
        position = EditorGUILayout.Vector2Field("Position", position);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        from = EditorGUILayout.Vector2Field("From", from);
        to = EditorGUILayout.Vector2Field("To", to);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("DrawCircle"))
            DrawCircle();
        showCircle = GUILayout.Toggle(showCircle, "Show?");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("DrawPolygon"))
            DrawPolygon();
        showPolygon = GUILayout.Toggle(showPolygon, "Show?");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("DrawLine"))
            DrawLine(from,to);
        showLine = GUILayout.Toggle(showLine, "Show?");
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Undo"))
            UndoOperation();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        degree = EditorGUILayout.IntSlider("Degree", degree, 0, 359);
        if (GUILayout.Button("Test Pattern"))
            RunPattern();
    }

    public void DrawCircle()
    {
        if (selectedPrefab == null)
            return;
        List<SpawnData> newList = new List<SpawnData>();
        foreach (SpawnData data in Inspecting.spawns)
            newList.Add(data);
        undoStack.Push(newList);
        for (int i = 0; i < bCount; i++)
        {
            float degree = (startRotation + 360 * ((float)i / (float)bCount)) * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(radius * Mathf.Cos(degree), radius * Mathf.Sin(degree)) + position;
            GameObject bullet = Instantiate(selectedPrefab, pos, Quaternion.identity);
            SpawnData newSpawn = new SpawnData();
            newSpawn.name = selectedPrefab.name;
            newSpawn.position = pos;
            dict.Add(bullet, newSpawn);
            Inspecting.spawns.Add(newSpawn);
            list.Add(bullet);
        }
        EditorUtility.SetDirty(Inspecting);
    }

    public void DrawPolygon()
    {
        if (selectedPrefab == null)
            return;
        List<SpawnData> newList = new List<SpawnData>();
        foreach (SpawnData data in Inspecting.spawns)
            newList.Add(data);
        undoStack.Push(newList);
        Vector2 a = position + new Vector2(radius * Mathf.Cos(startRotation * Mathf.Deg2Rad), radius * Mathf.Sin(startRotation * Mathf.Deg2Rad)), b;
        for (int i = 1; i <= eCount; i++)
        {
            float degree = (startRotation + 360 * ((float)i / (float)eCount)) * Mathf.Deg2Rad;
            b = position + new Vector2(radius * Mathf.Cos(degree), radius * Mathf.Sin(degree));
            for (int j = 0; j < bCount; j++)
            {
                float lerpValue = (float)j / (float)(bCount);
                Vector2 pos = Vector2.Lerp(a, b, lerpValue);
                GameObject bullet = Instantiate(selectedPrefab, pos, Quaternion.identity);
                SpawnData newSpawn = new SpawnData();
                newSpawn.name = selectedPrefab.name;
                newSpawn.position = pos;
                dict.Add(bullet, newSpawn);
                Inspecting.spawns.Add(newSpawn);
                list.Add(bullet);
            }
            a = b;
        }
        EditorUtility.SetDirty(Inspecting);
    }

    public void DrawLine(Vector2 a, Vector2 b)
    {
        if (selectedPrefab == null)
            return;
        List<SpawnData> newList = new List<SpawnData>();
        foreach (SpawnData data in Inspecting.spawns)
            newList.Add(data);
        undoStack.Push(newList);
        for (int i = 0; i < bCount; i++)
        {
            float lerpValue = (float)i / (float)(bCount - 1);
            Vector2 pos = Vector2.Lerp(a,b,lerpValue);
            GameObject bullet = Instantiate(selectedPrefab, pos, Quaternion.identity);
            SpawnData newSpawn = new SpawnData();
            newSpawn.name = selectedPrefab.name;
            newSpawn.position = pos;
            dict.Add(bullet, newSpawn);
            Inspecting.spawns.Add(newSpawn);
            list.Add(bullet);
        }
        EditorUtility.SetDirty(Inspecting);
    }

    public void DrawGO()
    {
        if (EditorApplication.isPlaying)
            return;
        foreach (SpawnData data in Inspecting.spawns)
        {
            GameObject newBullet = Instantiate(bulletAR.bulletDictionary[data.name], data.position, Quaternion.identity);
            dict.Add(newBullet, data);
            list.Add(newBullet);
        }
    }

    public void ClearGO()
    {
        while (list.Count != 0)
        {
            GameObject toDestroy = list[0];
            list.RemoveAt(0);
            DestroyImmediate(toDestroy);
        }
        dict.Clear();
    }

    public void UndoOperation()
    {
        if(undoStack.Count == 0)
        {
            Debug.Log("Cannot perfrom undo!");
            return;
        }
        List<SpawnData> debug = undoStack.Pop();
        Inspecting.spawns = debug;
        ClearGO();
        DrawGO();
        EditorUtility.SetDirty(Inspecting);
    }

    public void OnSceneGUI(SceneView sceneView)
    {
        
        Inspecting = (BulletPattern)target;
        if(showCircle)
        {
            Handles.color = new Color(1, 1, 1, 0.4f);
            Handles.DrawWireDisc(Vector2.zero + position, Vector3.back, radius);
            Handles.color = new Color(0,1,1,0.4f);
            for (int i = 0; i < bCount; i++)
            {
                float degree = (startRotation + 360 * ((float)i / (float)bCount)) * Mathf.Deg2Rad;
                Vector2 pos = position + new Vector2(radius * Mathf.Cos(degree), radius * Mathf.Sin(degree));
                Handles.DrawSolidDisc(pos, Vector3.back, 0.35f);
            }
        }
        if (showPolygon)
        {
            Vector2 a = position + new Vector2(radius*Mathf.Cos(startRotation * Mathf.Deg2Rad), radius * Mathf.Sin(startRotation * Mathf.Deg2Rad)), b;
            for (int i = 1; i <= eCount; i++)
            {
                Handles.color = new Color(1, 1, 1, 0.4f);
                float degree = (startRotation + 360 * ((float)i / (float)eCount)) * Mathf.Deg2Rad;
                b = position + new Vector2(radius * Mathf.Cos(degree), radius * Mathf.Sin(degree));
                Handles.DrawLine(a, b);
                Handles.color = new Color(0, 1, 1, 0.4f);
                for (int j = 0; j < bCount; j++)
                {
                    float lerpValue = (float)j / (float)(bCount);
                    Vector2 pos = Vector2.Lerp(a, b, lerpValue);
                    Handles.DrawSolidDisc(pos, Vector3.back, 0.35f);
                }
                a = b;
            }
        }
        if (showLine)
        {
            Handles.color = new Color(1, 1, 1, 0.4f);
            Handles.DrawLine(from, to);
            Handles.color = new Color(0, 1, 1, 0.4f);
            for (int i = 0; i < bCount; i++)
            {
                float lerpValue = (float)i / (float)(bCount - 1);
                Vector2 pos = Vector2.Lerp(from, to, lerpValue);
                Handles.DrawSolidDisc(pos, Vector3.back, 0.35f);
            }
        }
        SceneView.RepaintAll();
    }

    public void RunPattern()
    {
        if (BulletManager.instance != null)
            BulletManager.instance.StartCoroutine(BulletManager.instance.SpawnPattern(Inspecting, Vector2.zero, Quaternion.Euler(0,0,degree)));
        else
            Debug.Log("Instance is not found.");
    }
}
