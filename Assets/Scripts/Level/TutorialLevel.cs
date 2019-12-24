using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLevel : MonoBehaviour
{
    public int X, Y;
    private MiniMap miniMap;
    private Turret[] turrets;
    void Start()
    {
        miniMap = MiniMap.instance;
        miniMap.SetUp(X, Y);
        turrets = GetComponentsInChildren<Turret>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            miniMap.Modify(X, Y);
            foreach (Turret t in turrets)
                t.InvokeRepeating("Shoot",0,t.shootTime);
        }  
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            foreach (Turret t in turrets)
                t.CancelInvoke();
    }
}
