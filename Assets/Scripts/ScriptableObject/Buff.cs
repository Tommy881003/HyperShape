using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Modifier
{
    [ReadOnly]
    public string param;
    [Range(0,2)]
    public float multiplier;
    [Range(-3,3)]
    public float adder;
    public bool active;
};

[CreateAssetMenu]
[System.Serializable]
public class Buff : ScriptableObject
{
    public List<Modifier> modifiers = new List<Modifier>();
}
