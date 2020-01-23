using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpeedType
{
    Constant,       //子彈的初始速度皆相同
    ConstantDecay,  //子彈的初始速度皆相同，且速度會慢慢遞減
    Random,         //子彈的初始速度會乘上一個隨機常數
    RandomDecay     //子彈的初始速度會乘上一個隨機常數，且速度會慢慢遞減
}

public enum SpreadType
{
    Random,     //隨機分散
    Evenly      //平均分佈
}

public enum SizeType
{
    Constant,   //子彈大小不變
    Decay       //子彈大小會慢慢遞減
}

public enum ChargeType
{
    SlowerSpeed,    //充能的子彈速度會變慢
    None            //充能的子彈速度不變
}

[CreateAssetMenu]
public class WeaponInfo : ScriptableObject
{
    public GameObject bullet;
    [Space(15)]
    [Range(10f, 500f)]
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
    [Range(1f, 10f)]
    public float blastRad = 7.5f;
    [Range(0.1f, 0.5f)]
    public float blastDmg = 0.2f;
    [Space(15)]
    public bool flak = false;
    public float flakSizeDmg = 0.5f;
    public int flakCount = 3;
    [Space(15)]
    public int reflectCount = 0;
    [Space(15)]
    public float fireRate;

    public void ValueCopy(out WeaponInfo infoOut)
    {
        infoOut = CreateInstance<WeaponInfo>();
        infoOut.size = size;
        infoOut.damage = damage;
        infoOut.speed = speed;
        infoOut.spread = spread;
        infoOut.fireRate = fireRate;
        infoOut.count = count;

        infoOut.charge = charge;
        infoOut.blast = blast;
        infoOut.flak = flak;

        infoOut.chargeAff = chargeAff;
        infoOut.chargeTime = chargeTime;
        infoOut.flakCount = flakCount;
        infoOut.flakSizeDmg = flakSizeDmg;
        infoOut.blastRad = blastRad;
        infoOut.blastDmg = blastDmg;
        infoOut.reflectCount = reflectCount;

        infoOut.bullet = bullet;

        infoOut.sizeType = sizeType;
        infoOut.speedType = speedType;
        infoOut.spreadType = spreadType;
        infoOut.chargeType = chargeType;
    }
}
