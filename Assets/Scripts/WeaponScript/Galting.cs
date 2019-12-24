using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galting : PlayerBulletInfo
{
    // Start is called before the first frame update

    protected override void Start()
    {
        base.Start();
        sr.color = new Color(Random.Range(0.5f, 1), Random.Range(0.5f, 1), Random.Range(0.5f, 1));
        rb.AddTorque(0.005f);
    }
}
