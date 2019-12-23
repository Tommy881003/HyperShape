using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLevel : MonoBehaviour
{
    public int X, Y;
    private MiniMap miniMap;
    void Start()
    {
        miniMap = MiniMap.instance;
        miniMap.SetUp(X, Y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            miniMap.Modify(X, Y);
    }
}
