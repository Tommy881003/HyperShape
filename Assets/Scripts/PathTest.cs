using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PathTest : MonoBehaviour
{
    public GameObject testDummy;
    public Transform player;
    public int count = 0;
    private int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        i = 0;
        InvokeRepeating("SpawnDummy", 1, 0.5f);
    }

    void SpawnDummy()
    {
        GameObject newGO = Instantiate(testDummy, new Vector3(7,0,0), Quaternion.identity);
        //newGO.GetComponent<AIDestinationSetter>().target = player;
        i++;
        if (i >= count)
            Destroy(this);
    }
}
