using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpeedType
{
    Constant,
    ConstantDecay,
    Random,
    RandomDecay
}

public enum SpreadType
{
    Random,
    Evenly
}

public enum SizeType
{
    Constant,
    Decay
}

[CreateAssetMenu]
public class WeaponInfo : ScriptableObject
{
    public GameObject bullet;
    [Space(15)]
    [Range(10f, 150f)]
    public float speed;
    public SpeedType speedType;
    public SizeType sizeType;
    public float damage;
    [Space(15)]
    [Range(1f, 15f)]
    public int count = 1;
    public float spread;
    public SpreadType spreadType;
    [Space(15)]
    public float fireRate;
}
