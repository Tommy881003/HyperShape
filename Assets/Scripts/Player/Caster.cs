using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private WeapodMod property = null;
    private static List<Buff> Buffs = null;
    public WeaponInfo startWeapon;
    private static WeaponInfo weaponBase = null;
    private WeaponInfo weapon = null;
    private PlayerController player;

    private void Awake()
    {
        WeapodMod.LoadString();
        if (property == null)
            property = new WeapodMod();
        if (Buffs == null)
            Buffs = new List<Buff>();
        if (weaponBase == null)
            weaponBase = startWeapon;
        property.FinalizeWeapon(weaponBase, out weapon);
    }

    void Start()
    {
        cam = Camera.main;
        follower = cam.GetComponent<CameraFollower>();
        time = 0;
        player = GetComponentInParent<PlayerController>();
        fireTime = 1 / weapon.fireRate;
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
        int burst = property.burstNum;
        int count = weapon.count;
        float spread = weapon.spread;
        GameObject bullet = weapon.bullet;
        if(weapon.charge)
        {
            float timer = 0;
            float amp = 1;
            while(Input.GetMouseButtonUp(0) == false)
            {
                timer += Time.deltaTime;
                amp = 1 + Mathf.Min(1, timer);
                yield return null;
            }
        }
        for(int i = 0; i < burst; i++)
        {
            Vector3 mousePos = new Vector3(cam.ScreenToWorldPoint(Input.mousePosition).x, cam.ScreenToWorldPoint(Input.mousePosition).y);
            for(int j = 0; j < count; j++)
            {
                float angle = (Vector2.SignedAngle(Vector3.right, mousePos - transform.position) + Random.Range(-spread, spread)) * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
                GameObject newBullet = Instantiate(bullet, transform.position + dir.normalized * 0.5f, Quaternion.Euler(0, 0, angle));
                newBullet.GetComponent<PlayerBulletInfo>().SetBullet(weapon, angle);
            }
            if (i != burst - 1)
                yield return new WaitForSeconds(0.4f / property.burstNum);
        }
        canAttack = true;
        yield break;
    }

    public void AddBuff(Buff buff)
    {
        foreach (Modifier modifier in buff.modifiers)
            property.Modify(modifier);
        Buffs.Add(buff);
        property.FinalizeWeapon(weaponBase, out weapon);
        fireTime = 1 / weapon.fireRate;
    }

    public void RemoveBuff(Buff buff)
    {
        if (!Buffs.Contains(buff))
            return;
        Buffs.Remove(buff);
        ReBuff();
    }

    public void ReBuff()
    {
        property = WeapodMod.Default;
        foreach(Buff b in Buffs)
        {
            foreach(Modifier modifier in b.modifiers)
                property.Modify(modifier);
        }
        property.FinalizeWeapon(weaponBase, out weapon);
        fireTime = 1 / weapon.fireRate;
    }
}
