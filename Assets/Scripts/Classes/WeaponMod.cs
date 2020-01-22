using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeapodMod
{
    public static List<string> intList = null, floatList = null, boolList = null;
    /*以下是屬性參數，可以隨意新增新內容*/
    public float damageAmp = 1;
    public float sizeAmp = 1;
    public float speedAmp = 1;
    public float spreadAmp = 1;
    public float rateAmp = 1;
    public int countOffset = 0;
    public int countAmp = 1;
    public int burstNum = 1;
    public bool charge = false;
    public bool blast = false;
    public bool flak = false;
    /*以上是屬性參數，可以隨意新增新內容*/
    public static WeapodMod Default { get; set; } = new WeapodMod();
    public static void LoadString()
    {
        System.Type type = typeof(WeapodMod);
        var properties = type.GetFields();
        intList = new List<string>();
        floatList = new List<string>();
        boolList = new List<string>();
        for (int i = 0; i < properties.Length; i++)
        {
            if (properties[i].FieldType.Equals(typeof(int)))
                intList.Add(properties[i].Name);
            else if (properties[i].FieldType.Equals(typeof(float)))
                floatList.Add(properties[i].Name);
            else if (properties[i].FieldType.Equals(typeof(bool)))
                boolList.Add(properties[i].Name);
        }
    }

    public T Get<T>(string name)
    {
        object value = GetType().GetField(name).GetValue(this);
        return (T)value;
    }

    public void Set<T>(string name, T result)
    {
        GetType().GetField(name).SetValue(this, result);
    }

    public void Modify(Modifier modifier)
    {
        string name = modifier.param;
        float multiplier = modifier.multiplier;
        float adder = modifier.adder;
        bool active = modifier.active;
        if (intList.Contains(name))
        {
            int raw = Get<int>(name);
            int processed = (int)multiplier * raw + (int)adder;
            Set<int>(name, processed);
        }
        else if(floatList.Contains(name))
        {
            float raw = Get<float>(name);
            float processed = multiplier * raw + adder;
            Set<float>(name, processed);
        }
        else if(boolList.Contains(name))
            Set<bool>(name, active);
    }

    public void FinalizeWeapon(WeaponInfo infoIn, out WeaponInfo infoOut)
    {
        infoOut = ScriptableObject.CreateInstance<WeaponInfo>();
        infoOut.size = infoIn.size * sizeAmp;
        infoOut.damage = infoIn.damage * damageAmp;
        infoOut.speed = infoIn.speed * speedAmp;
        infoOut.spread = infoIn.spread * spreadAmp;
        infoOut.fireRate = infoIn.fireRate * rateAmp;
        infoOut.count = infoIn.count * countAmp + countOffset;

        infoOut.charge = charge;
        infoOut.blast = blast;
        infoOut.flak = flak;

        infoOut.chargeAff = infoIn.chargeAff;
        infoOut.chargeTime = infoIn.chargeTime;
        infoOut.flakCount = infoIn.flakCount;
        infoOut.flakSizeDmg = infoIn.flakSizeDmg;
        infoOut.blastRad = infoIn.blastRad;
        infoOut.blastDmg = infoIn.blastDmg;

        infoOut.bullet = infoIn.bullet;
        
        infoOut.sizeType = infoIn.sizeType;
        infoOut.speedType = infoIn.speedType;
        infoOut.spreadType = infoIn.spreadType;
        infoOut.chargeType = infoIn.chargeType;
    }
}
