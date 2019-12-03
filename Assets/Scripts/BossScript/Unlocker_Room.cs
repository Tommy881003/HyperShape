using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Unlocker_Room : MonoBehaviour
{
    [SerializeField]
    private GameObject tile = null;
    private Level level;
    private Unlocker_Tile[,] tiles;
    private Vector2Int[] popPos;
    void Start()
    {
        DOTween.SetTweensCapacity(500, 100);
        level = GetComponent<Level>();
        tiles = new Unlocker_Tile[level.X,level.Y];
        popPos = new Vector2Int[level.X * level.Y];
        for (int i = 0; i < level.X; i++)
            for (int j = 0; j < level.Y; j++)
                tiles[i, j] = Instantiate(tile, ind2vec(i, j), Quaternion.identity, transform).GetComponent<Unlocker_Tile>();
        for (int i = 0; i < level.X * level.Y; i++)
            popPos[i] = new Vector2Int(i % level.X, i / level.X);
        for (int i = 0; i < level.X * level.Y; i++)
        {
            int rand = UnityEngine.Random.Range(0, level.X * level.Y);
            Vector2Int temp = popPos[i];
            popPos[i] = popPos[rand];
            popPos[rand] = temp;
        }
        //InvokeRepeating("TestPop", 3, 1);
    }

    public Vector3 ind2vec(int x,int y)
    {
        Vector3 startVect = new Vector3(transform.position.x - (2*level.X - 2), transform.position.y + (2*level.Y - 2));
        return startVect + new Vector3(4*x,-4*y);
    }

    public Vector2Int vec2ind(Vector3 position)
    {
        Vector3 startVect = position - new Vector3(transform.position.x - (2 * level.X), transform.position.y + (2 * level.Y));
        return new Vector2Int(Mathf.CeilToInt(Mathf.Abs(startVect.x / 4f)) - 1, Mathf.CeilToInt(Mathf.Abs(startVect.y / 4f)) - 1);
    }

    public void RandomPop(int num, float warnTime, float popTime)
    {
        for (int i = 0; i < num; i++)
        {
            if(tiles[popPos[i].x, popPos[i].y].isPoping == false)
               StartCoroutine(tiles[popPos[i].x, popPos[i].y].Pop(warnTime, popTime));
        }
            
        for (int i = 0; i < num; i++)
        {
            int rand = UnityEngine.Random.Range(2*num, level.X * level.Y);
            Vector2Int temp = popPos[i];
            popPos[i] = popPos[rand];
            popPos[rand] = temp;
        }
    }

    public void DoPop(int x,int y, float warnTime, float popTime)
    {
        if (x >= 0 && x < level.X && y >= 0 && y < level.Y)
            StartCoroutine(tiles[x, y].Pop(warnTime, popTime));
    }

    public void DoFade(int x, int y, float time)
    {
        if (x >= 0 && x < level.X && y >= 0 && y < level.Y)
            StartCoroutine(tiles[x, y].Fade(time));
    }
}
