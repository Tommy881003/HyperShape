using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletCreator;

[CreateAssetMenu]
public class BulletPattern : ScriptableObject
{
    [Header("彈幕持續時間"),Range(1,20)]
    public float timer = 10;
    public AnimationCurve vRotation = new AnimationCurve();
    public AnimationCurve velocity = new AnimationCurve();
    public AnimationCurve rotation = new AnimationCurve();
    public AnimationCurve scale = new AnimationCurve();
    public List<SpawnData> spawns = new List<SpawnData>();
}
