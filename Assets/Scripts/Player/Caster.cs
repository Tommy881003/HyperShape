using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Buff
{
    public struct Modifier
    {
        public string param;
        public float multiplier;
        public float adder;
    };
    public List<Modifier> modifiers = new List<Modifier>();
}

public class Property
{
    public static List<string> intList = null, floatList = null;
    public Dictionary<string, float> floatDict = new Dictionary<string, float>();
    public Dictionary<string, int> intDict = new Dictionary<string, int>();
    /*以下是屬性參數，可以隨意新增新內容*/
    public float damageAmp = 1;
    public float sizeAmp = 1;
    public float speedAmp = 1;
    public float spreadAmp = 1;
    public float rateAmp = 1;
    public int countOffset = 0;
    public int countAmp = 1;
    public int burstNum = 1;
    /*以上是屬性參數，可以隨意新增新內容*/
    public void Init()
    {
        var properties = GetType().GetFields();
        if(intList != null && floatList != null)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].GetType() == typeof(int))
                    intDict.Add(properties[i].Name, (int)properties[i].GetValue(this));
                else if (properties[i].GetType() == typeof(float))
                    floatDict.Add(properties[i].Name, (int)properties[i].GetValue(this));
            }
        }
        else
        {
            intList = new List<string>();
            floatList = new List<string>();
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].GetType() == typeof(int))
                {
                    intList.Add(properties[i].Name);
                    intDict.Add(properties[i].Name, (int)properties[i].GetValue(this));
                } 
                else if (properties[i].GetType() == typeof(float))
                {
                    floatList.Add(properties[i].Name);
                    floatDict.Add(properties[i].Name, (int)properties[i].GetValue(this));
                }  
            }
        }
    }
}

public class Caster : MonoBehaviour
{
    /*發射武器時的畫面晃動*/
    protected Camera cam;
    protected CameraFollower follower;
    private float strength;
    
    [HideInInspector]
    public float fireTime;
    private float time;
    [HideInInspector]
    public bool canAttack = true;

    private float speed;
    private float spread;
    private int countNum;
    private GameObject bullet = null;
    private PlayerBulletInfo bulletInfo = null;

    private static Property property = null;
    private static List<Buff> Buffs = null;
    private static WeaponInfo weapon = null;
    private PlayerController player;

    private void Awake()
    {
        if (property == null)
        {
            property = new Property();
            property.Init();
        }
        if (Buffs == null)
            Buffs = new List<Buff>();
        if (weapon == null)
            weapon = ScriptableObject.CreateInstance<WeaponInfo>();
        Modify();
    }

    void Start()
    {
        cam = Camera.main;
        follower = cam.GetComponent<CameraFollower>();
        time = 0;
        player = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.enabled == false)
            return;
        if (canAttack == true)
            time = Mathf.Max(time - Time.deltaTime, 0);
        if (Input.GetMouseButton(0) && canAttack && time <= 0 && Time.timeScale >= 0.5f)
            StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        canAttack = false;
        time = fireTime;
        for(int i = 0; i < property.burstNum; i++)
        {
            Vector3 mousePos = new Vector3(cam.ScreenToWorldPoint(Input.mousePosition).x, cam.ScreenToWorldPoint(Input.mousePosition).y);
            for(int j = 0; j < countNum; j++)
            {
                float angle = (Vector2.SignedAngle(Vector3.right, mousePos - transform.position) + Random.Range(-spread, spread)) * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
                GameObject newBullet = Instantiate(bullet, transform.position + dir.normalized * 0.5f, Quaternion.Euler(0, 0, angle));
            }
            if (i != property.burstNum - 1)
                yield return new WaitForSeconds(0.4f / property.burstNum);
        }
        canAttack = true;
        yield break;
    }

    void Modify()
    {
        bullet = weapon.bullet;
        bulletInfo = bullet.GetComponent<PlayerBulletInfo>();
        bullet.transform.localScale = Vector2.one * property.sizeAmp;
        bulletInfo.damage = weapon.damage * property.damageAmp;
        fireTime = 1 / (weapon.fireRate * property.rateAmp);
        countNum = (weapon.count * property.countAmp) + property.countOffset;
        speed = weapon.speed * property.speedAmp;
        spread = weapon.spread * property.spreadAmp;
    }

    public void AddBuff(Buff buff)
    {
        foreach(Buff.Modifier modifier in buff.modifiers)
        {
            string param = modifier.param;
            float multiplier = modifier.multiplier;
            float adder = modifier.adder;
            if (Property.intList.Contains(param))
                property.intDict[param] = property.intDict[param] * (int)multiplier + (int)adder;
            if (Property.floatList.Contains(param))
                property.floatDict[param] = property.floatDict[param] * multiplier + adder;
        }
        Buffs.Add(buff);
    }

    public void RemoveBuff(Buff buff)
    {
        if (!Buffs.Contains(buff))
            return;
        foreach (Buff.Modifier modifier in buff.modifiers)
        {
            string param = modifier.param;
            float multiplier = modifier.multiplier;
            float adder = modifier.adder;
            if (Property.intList.Contains(param))
                property.intDict[param] = property.intDict[param] * (int)multiplier + (int)adder;
            if (Property.floatList.Contains(param))
                property.floatDict[param] = property.floatDict[param] * multiplier + adder;
        }
    }
}
