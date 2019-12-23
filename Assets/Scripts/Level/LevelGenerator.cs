using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Pathfinding;

public class LevelGenerator : MonoBehaviour
{
    [HideInInspector]
    public enum GeneratingStatus
    {
        Initiating = 0,         //初始化
        Selecting = 1,          //從Prefab資料夾中選出若干個關卡並依據關卡大小決定放置順序
        Place_Levels = 2,       //放置關卡
        Construct_Paths = 3,    //決定關卡之間的連結
        Final_Makeup = 4
    }
    [Header("關卡數量限制"), Range(0,15)]
    public int minMap = 7, maxMap = 7;
    [Header("地圖邊界限制")]
    public readonly int mapX = 8, mapY = 8;
    [Header("方格大小")]
    public int blockSize = 20;
    [HideInInspector]
    public int[,] map;
    [HideInInspector]
    public GeneratingStatus currentStatus = GeneratingStatus.Initiating;
    public GameObject Composite;
    public GameObject StartingPlace;
    public GameObject Boss;
    public GameObject path;
    private GameObject[] levels, lvToPut;
    private Level[] lvInform;      //儲存lvToPut內的Level class資料
    private Stack<Vector2Int> search = new Stack<Vector2Int>();
    private int halfRand;
    private int minX,minY,maxX,maxY;
    private MiniMap miniMap;

    void Start()
    {
        map = new int[mapX, mapY];
        miniMap = MiniMap.instance;
        levels = Resources.LoadAll<GameObject>("Prefab(LC)/SubLevels");
        for (int i = 0; i < mapY; i++)
            for (int j = 0; j < mapX; j++)
                map[i,j] = -1;
        Selecting();
    }

    private void Selecting() //步驟1
    {
        currentStatus = GeneratingStatus.Selecting;
        /*此段以上負責狀態更改*/

        int len = levels.Length;
        for (int i = 0; i < len; i++)
        {
            int rand = UnityEngine.Random.Range(0, len - 1);
            GameObject newGO = levels[i];
            levels[i] = levels[rand];
            levels[rand] = newGO;
        }
        /*此段以上負責洗牌*/

        int lvCount = UnityEngine.Random.Range(minMap, maxMap);
        halfRand = UnityEngine.Random.Range((lvCount / 2) - 2, (lvCount / 2) + 2);
        lvToPut = new GameObject[lvCount + 2];
        lvInform = new Level[lvCount + 2];
        lvToPut[0] = StartingPlace;
        lvInform[0] = StartingPlace.GetComponent<Level>();
        lvToPut[1] = Boss;
        lvInform[1] = Boss.GetComponent<Level>();
        for (int i = 2; i < lvCount + 2; i ++)
        {
            lvToPut[i] = levels[i];
            lvInform[i] = lvToPut[i].GetComponent<Level>();
        }
        /*此段以上負責抽關卡*/

        PlaceLevels();
    }

    private void PlaceLevels() //步驟2
    {
        currentStatus = GeneratingStatus.Place_Levels;
        /*此段以上負責狀態更改*/

        int len = lvToPut.Length;
        int[] dir = { 0, 1, 2, 3 };
        int x = 3;
        int y = 3;
        map[3, 3] = 0;
        lvToPut[0].transform.position = idx2Vec(3, 3);
        lvInform[0].idxX = 3;
        lvInform[0].idxY = 3;
        miniMap.SetUp(3, 3);
        minX = x; maxX = x; minY = y; maxY = y;
        /*第一個被隨機生成的關卡*/
        for (int i = 1; i < len; i ++)
        {
            for (int j = 0; j < 4; j++)
            {
                int rand = UnityEngine.Random.Range(0, 4);
                int temp = dir[j];
                dir[j] = dir[rand];
                dir[rand] = temp;
            }
            for (int j = 0; j < 4; j++)
            {
                int tempX = x;
                int tempY = y;
                if (dir[j] == 0)
                    tempX++;
                else if (dir[j] == 1)
                    tempX--;
                else if (dir[j] == 2)
                    tempY++;
                else
                    tempY--;
                if (tempX >= 0 && tempX < mapX && tempY >= 0 && tempY < mapY && map[tempX, tempY] == -1)
                {
                    lvToPut[i] = Instantiate(lvToPut[i], idx2Vec(tempX, tempY), Quaternion.identity, Composite.transform);
                    lvInform[i] = lvToPut[i].GetComponent<Level>();
                    lvInform[i].idxX = tempX;
                    lvInform[i].idxY = tempY;
                    miniMap.SetUp(tempX, tempY, i == 1);
                    map[tempX, tempY] = i;
                    minX = (minX > tempX ? tempX : minX);
                    minY = (minY > tempY ? tempY : minY);
                    maxX = (maxX < tempX ? tempX : maxX);
                    maxY = (maxY < tempY ? tempY : maxY);
                    x = (i == 1 || i == halfRand) ? 3 : tempX;
                    y = (i == 1 || i == halfRand) ? 3 : tempY;
                    break;
                }
            }
        }
#if UNITY_EDITOR
        string path = "Assets/Resources/Test.txt";
        StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.Truncate));
        for (int i = 0; i < mapY; i++)
        {
            string str = "";
            for (int j = 0; j < mapX; j++)
            {
                if (map[j, i] >= 0)
                    str += map[j, i].ToString();
                else
                    str += "+";
            }
            str += "\n";
            writer.Write(str);
        }
        writer.Close();
#endif
        ConstructPath();
    }

    private void ConstructPath()
    {
        currentStatus = GeneratingStatus.Construct_Paths;
        for (int i = 0; i < mapY; i++)
        {
            for (int j = 0; j < mapX; j++)
            {
                if (map[j, i] >= 0)
                {
                    if (j + 1 < mapX && map[j + 1, i] != -1)
                        Link(j,i,true);
                    if (i + 1 < mapY && map[j, i + 1] != -1)
                        Link(j,i,false);
                }
            }
        }
        currentStatus = GeneratingStatus.Final_Makeup;
        AstarPath.active.data.gridGraph.center = idx2Vec((minX + maxX) / 2f, (minY + maxY) / 2f);
        AstarPath.active.data.gridGraph.SetDimensions(4 * blockSize * (maxX - minX) + 72, 4 * blockSize * (maxY - minY) + 72, 1);
        AstarPath.active.Scan();
    }

    void Link(int i, int j, bool forX)
    {
        int a = map[i, j], b;
        if (forX)
            b = map[i + 1, j];
        else
            b = map[i, j + 1];
        GameObject temp;
        if (lvToPut[a].transform.position.y == lvToPut[b].transform.position.y)
        {
            float xPos = (lvToPut[a].transform.position.x + 2 * lvInform[a].X + lvToPut[b].transform.position.x - 2 * lvInform[b].X) / 2f;
            float xScale = (2*blockSize - lvInform[a].X - lvInform[b].X)/2f;
            temp = Instantiate(path, new Vector3(xPos, lvToPut[a].transform.position.y), Quaternion.identity);
            temp.transform.localScale = new Vector3(xScale * temp.transform.localScale.x, temp.transform.localScale.y);
        }
        else
        {
            float yPos = (lvToPut[a].transform.position.y - 2 * lvInform[a].Y + lvToPut[b].transform.position.y + 2 * lvInform[b].Y) / 2f;
            float yScale = (2*blockSize - lvInform[a].Y - lvInform[b].Y)/2f;
            temp = Instantiate(path, new Vector3(lvToPut[a].transform.position.x, yPos), Quaternion.identity);
            temp.transform.localScale = new Vector3(temp.transform.localScale.x, yScale * temp.transform.localScale.y);
        }
        temp.transform.SetParent(Composite.transform);
    }

    Vector3 idx2Vec(float x,float y)
    {
        return new Vector3((-(((float)mapX/2f) - 0.5f) + x) * 4f * blockSize, ((((float)mapY / 2f) - 0.5f) - y) * 4f * blockSize);
    }
}
