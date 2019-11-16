using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaCollision : MonoBehaviour
{
    private CompositeCollider2D cc;
    private LineRenderer lr;
    private Vector3[] path;
    private void Start()
    {
        cc = this.GetComponent<CompositeCollider2D>();
        lr = this.GetComponent<LineRenderer>();
        path = new Vector3[cc.GetPathPointCount(0)];
        Vector2[] temp = new Vector2[cc.GetPathPointCount(0)];
        cc.GetPath(0, temp);
        for (int i = 0; i < cc.GetPathPointCount(0); i++)
            path[i] = new Vector3(temp[i].x,temp[i].y,0);
        lr.positionCount = cc.GetPathPointCount(0);
        lr.SetPositions(path);
        lr.loop = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
            Destroy(collision.gameObject);
    }
}
