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
    public float chargeAffAmp = 1;
    public float chargeTimeAmp = 1;
    public float blastRadAmp = 1;
    public float blastDmgAmp = 1;
    public float flakSizeDmgAmp = 1;

    public int countOffset = 0;
    public int countAmp = 1;
    public int burstNum = 1;
    public int flakCountOffset = 0;
    public int reflectCountOffest = 0;

    public bool charge = false;
    public bool blast = false;
    public bool flak = false;
    /*以上是屬性參數，可以隨意新增新內容*/

    /*WeaponMod的預設值，可以想成是類似Vector3.one的存在*/
    public static WeapodMod Default { get; set; } = new WeapodMod();

    /*把參數名稱存進static Lists中*/
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

    /*根據Modifier的內容調整參數*/
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

    /*把參數內容具現化到武器(WeaponInfo)上*/
    public void FinalizeWeapon(WeaponInfo infoIn, out WeaponInfo infoOut)
    {
        /* 由於這裡的infoIn是位置拷貝，如果直接讓infoOut = infoIn，  *
         * 再對infoOut做修改會讓infoIn也被改到，因此得額外建instance */
        infoOut = ScriptableObject.CreateInstance<WeaponInfo>();
        infoOut.size = infoIn.size * sizeAmp;
        infoOut.damage = infoIn.damage * damageAmp;
        infoOut.speed = infoIn.speed * speedAmp;
        infoOut.spread = infoIn.spread * spreadAmp;
        infoOut.fireRate = infoIn.fireRate * rateAmp;
        infoOut.count = infoIn.count * countAmp + countOffset;

        infoOut.charge = infoIn.charge | charge;
        infoOut.blast = infoIn.blast | blast;
        infoOut.flak = infoIn.flak | flak;

        infoOut.chargeAff = infoIn.chargeAff * chargeAffAmp;
        infoOut.chargeTime = infoIn.chargeTime * chargeTimeAmp;
        infoOut.flakCount = infoIn.flakCount + flakCountOffset;
        infoOut.flakSizeDmg = infoIn.flakSizeDmg * flakSizeDmgAmp;
        infoOut.blastRad = infoIn.blastRad * blastRadAmp;
        infoOut.blastDmg = infoIn.blastDmg * blastDmgAmp;
        infoOut.reflectCount = infoIn.reflectCount + reflectCountOffest;

        infoOut.bullet = infoIn.bullet;
        
        infoOut.sizeType = infoIn.sizeType;
        infoOut.speedType = infoIn.speedType;
        infoOut.spreadType = infoIn.spreadType;
        infoOut.chargeType = infoIn.chargeType;
    }
}
