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

public enum ChargeType
{
    SlowerSpeed,
    None
}

[CreateAssetMenu]
public class WeaponInfo : ScriptableObject
{
    public GameObject bullet;
    [Space(15)]
    [Range(10f, 150f)]
    public float speed;
    public SpeedType speedType;
    public float size = 1;
    public SizeType sizeType;
    public float damage;
    [Space(15)]
    [Range(1f, 15f)]
    public int count = 1;
    public float spread;
    public SpreadType spreadType;
    [Space(15)]
    public bool charge = false;
    [Range(1f, 5f)]
    public float chargeTime = 2;
    [Range(1f, 5f)]
    public float chargeAff = 2;
    public ChargeType chargeType;
    [Space(15)]
    public bool blast = false;
    [Range(1f, 5f)]
    public float blastRad = 2;
    [Range(0.1f, 0.5f)]
    public float blastDmg = 0.2f;
    [Space(15)]
    public bool flak = false;
    public float flakSizeDmg = 0.5f;
    public int flakCount = 3;
    [Space(15)]
    public float fireRate;
}
