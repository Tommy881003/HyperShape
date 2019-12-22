using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MiniMap : MonoBehaviour
{
    public enum status
    {
        none = 0,
        hidden = 1,
        undiscovered = 2,
        discovered = 3,
        current = 3
    }
    public static MiniMap instance = null;
    public GameObject[] b, r, c;
    public Color undis, dis, now, boss, edge;
    private CanvasGroup canvas;
    private Image[,] blocks, rows, columns;
    private status[,] bStat;
    private int bossX, bossY;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        canvas = GetComponent<CanvasGroup>();
        blocks = new Image[8, 8];
        rows = new Image[8, 7];
        columns = new Image[8, 7];
        bStat = new status[8, 8];
        for(int i = 0; i < 8; i++)
        {
            Image[] block = b[i].GetComponentsInChildren<Image>();
            Image[] row = r[i].GetComponentsInChildren<Image>();
            Image[] column = c[i].GetComponentsInChildren<Image>();
            for(int j = 0; j < 7; j++)
            {
                blocks[j, i] = block[j];
                rows[i, j] = row[j];
                columns[i, j] = column[j];
                bStat[j, i] = status.none;
            }
            blocks[7,i] = block[7];
            bStat[7,i] = status.none;
        }
    }

    public void SetUp(int x, int y, bool isBoss = false)
    {
        bStat[x, y] = status.hidden;
        if (isBoss)
        {
            bossX = x;
            bossY = y;
        }
    }

    public void Modify(int x, int y)
    {
        bStat[x, y] = status.current;
        blocks[x, y].color = now;
        if(x - 1 >= 0 && bStat[x - 1, y] != status.none)
        {
            if(bStat[x - 1, y] == status.hidden)
            {
                bStat[x - 1, y] = status.undiscovered;
                blocks[x - 1, y].color = (x - 1 == bossX && y == bossY)? boss : undis;
            }
            else if(bStat[x - 1, y] == status.current)
            {
                bStat[x - 1, y] = status.discovered;
                blocks[x - 1, y].color = (x - 1 == bossX && y == bossY) ? boss : dis;
            }
            rows[y, x - 1].color = edge;
        }
        if (x + 1 < 8 && bStat[x + 1, y] != status.none)
        {
            if (bStat[x + 1, y] == status.hidden)
            {
                bStat[x + 1, y] = status.undiscovered;
                blocks[x + 1, y].color = (x + 1 == bossX && y == bossY) ? boss : undis;
            }
            else if (bStat[x + 1, y] == status.current)
            {
                bStat[x + 1, y] = status.discovered;
                blocks[x + 1, y].color = (x + 1 == bossX && y == bossY) ? boss : dis;
            }
            rows[y, x].color = edge;
        }
        if (y - 1 >= 0 && bStat[x, y - 1] != status.none)
        {
            if (bStat[x, y - 1] == status.hidden)
            {
                bStat[x, y - 1] = status.undiscovered;
                blocks[x, y - 1].color = (x == bossX && y - 1 == bossY) ? boss : undis; 
            }
            else if (bStat[x, y - 1] == status.current)
            {
                bStat[x, y - 1] = status.discovered;
                blocks[x, y - 1].color = (x == bossX && y - 1 == bossY) ? boss : dis;
            }
            columns[x, y - 1].color = edge;
        }
        if (y + 1 < 8 && bStat[x, y + 1] != status.none)
        {
            if (bStat[x, y + 1] == status.hidden)
            {
                bStat[x, y + 1] = status.undiscovered;
                blocks[x, y + 1].color = (x == bossX && y + 1 == bossY) ? boss : undis;
            }
            else if (bStat[x, y + 1] == status.current)
            {
                bStat[x, y + 1] = status.discovered;
                blocks[x, y + 1].color = (x == bossX && y + 1 == bossY) ? boss : dis;
            }
            columns[x, y].color = edge;
        }
    }

    public void On()
    {
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, 1, 1);
    }

    public void Off()
    {
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, 0, 1);
    }
}
