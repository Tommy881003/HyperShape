using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu]
public class LevelInfo : ScriptableObject
{
    public int maxWaveNum;
    public float spawnTime;
    public GameObject warning;
    public Ease ease, fadeEase;
}
