using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    int a = 0;
    Dictionary<string, int> dir = new Dictionary<string, int>();
    // Start is called before the first frame update
    void Start()
    {
        dir.Add("a", a);
        InvokeRepeating("Poke", 0, 2);
    }

    void Poke()
    {
        dir["a"]++;
        Debug.Log("dir[a] = " + dir["a"] + ", a = " + a);
    }
}
