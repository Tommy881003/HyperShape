using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaCollision : MonoBehaviour
{
    /*private CompositeCollider2D cc;
    private LineRenderer lr;
    private Vector3[] path;
    private void Start()
    {
        cc = this.GetComponent<CompositeCollider2D>();
        lr = this.GetComponent<LineRenderer>();
        Invoke("Draw", 0.2f);
    }

    private void Draw()
    {
        path = new Vector3[cc.pointCount + cc.pathCount];
        Vector2[] temp = new Vector2[cc.pointCount + cc.pathCount];
        int k = 0;
        for(int i = 0; i < cc.pathCount; i++)
        {
            Vector2[] tPath = new Vector2[cc.GetPathPointCount(i)];
            cc.GetPath(i, tPath);
            for(int j = 0; j < cc.GetPathPointCount(i); j++)
            {
                temp[k] = tPath[j]; 
                k++;
            }
            temp[k] = tPath[0];
            k++;
        }
        for (int i = 0; i < path.Length; i++)
            path[i] = new Vector3(temp[i].x, temp[i].y, 0);
        lr.positionCount = cc.pointCount;
        lr.SetPositions(path);
        lr.loop = true;
    }*/
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
            collision.gameObject.GetComponent<PlayerBulletInfo>().StartCoroutine(collision.gameObject.GetComponent<PlayerBulletInfo>().DelayDestroy());
    }
}
