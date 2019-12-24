using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Weapon : MonoBehaviour
{
    public WeaponInfo gun;
    private WeaponInfo currentGun;
    private List<WeaponInfo> weapons = new List<WeaponInfo>();
    [SerializeField]
    private List<WeaponInfo> startWps = new List<WeaponInfo>();
    protected Camera cam;
    protected CameraFollower follower;
    [HideInInspector]
    public float fireTime;
    [HideInInspector]
    public int currentAmmo;
    [HideInInspector]
    public bool canAttack = true;
    [HideInInspector]
    public bool canSwitch = false;
    private int index = 0;
    private int WpCount;
    private float strength;
    private float time;
    private AudioSource audios;
    private PlayerController player;
    public TextMeshPro text;
    public GameObject parent,follow;
    public Color level0, level1, level2, level3;


    private void Start()
    {
        weapons.Add(gun);
        currentGun = gun;
        currentAmmo = currentGun.ammoCapacity;
        audios = GetComponent<AudioSource>();
        cam = Camera.main;
        follower = cam.GetComponent<CameraFollower>();
        if (currentGun.fireRate > 0)
            fireTime = 1 / currentGun.fireRate;
        else
            Debug.LogWarning("The fire rate should always be positive");
        strength = Mathf.Lerp(0.5f, 3,Mathf.Clamp01((currentGun.damage * currentGun.count)/40f));
        time = Mathf.Clamp(fireTime, 0.1f, 0.3f);
        WpCount = startWps.Count;
        player = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        if (player.enabled == false)
            return;
        if (Time.timeScale >= 0.5f && canAttack && ((currentGun.type == weapon_type.auto && Input.GetMouseButton(0)) || Input.GetMouseButtonDown(0)))
        {
            if(currentAmmo > 0)
            {
                StartCoroutine(Shoot());
                StartCoroutine(follower.CamShake(strength, time));
            }
        }
        if (canSwitch && Input.GetKeyDown(KeyCode.E))
            StartCoroutine(Switch());
    }

    private void FixedUpdate()
    {
        parent.transform.position = follow.transform.position + new Vector3(0, 2.5f, 0);
    }

    public void AddWeapon()
    {
        int idx = -1;
        int maxCount = 0;
        for (int i = 0; i < weapons.Count; i++)
            maxCount += weapons[i].upgrades.Length;
        float rand = Random.Range(0, 0.999f);
        if(maxCount > 0 && rand > (float)startWps.Count / (float)WpCount)   //upgrade
        {
            int seed = Random.Range(0, maxCount);
            for (int i = 0; i < weapons.Count; i++)
            {
                if (seed >= weapons[i].upgrades.Length)
                    seed -= weapons[i].upgrades.Length;
                else
                {
                    weapons[i] = weapons[i].upgrades[seed];
                    idx = i;
                    break;
                }
            }
        }
        else    //add gun
        {
            idx = Mathf.FloorToInt(rand * startWps.Count);
            weapons.Add(startWps[idx]);
            startWps.RemoveAt(idx);
            idx = weapons.Count - 1;
        }
        StartCoroutine(Switch(idx));
    }

    private IEnumerator Switch(int forceIndex = -1)
    {
        text.DOKill();
        canSwitch = false;
        if (forceIndex == -1)
            index = (index + 1) % weapons.Count;
        else
            index = forceIndex;
        currentGun = weapons[index];
        audios.Play();
        fireTime = 1 / currentGun.fireRate;
        strength = Mathf.Lerp(0.5f, 3, Mathf.Clamp01((currentGun.damage * currentGun.count) / 40f));
        time = Mathf.Clamp(fireTime, 0.1f, 0.3f);
        if(currentGun.level == 0)
            text.color = level0;
        else if (currentGun.level == 1)
            text.color = level1;
        else if (currentGun.level == 2)
            text.color = level2;
        else if (currentGun.level == 3)
            text.color = level3;
        text.text = currentGun.name;
        yield return new WaitForSeconds(0.5f);
        canSwitch = true;
        yield return new WaitForSeconds(1f);
        text.DOColor(Color.clear, 0.5f);
    }

    private IEnumerator Shoot()
    {
        canAttack = false;
        Vector3 mousePos = new Vector3(cam.ScreenToWorldPoint(Input.mousePosition).x, cam.ScreenToWorldPoint(Input.mousePosition).y);
        float angle = (Vector2.SignedAngle(Vector3.right, mousePos - transform.position) + UnityEngine.Random.Range(-currentGun.spread, currentGun.spread)) * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        switch(currentGun.type)
        {
            case weapon_type.burst:
                {
                    for (int i = 0; i < currentGun.burst_count; i++)
                    {
                        mousePos = new Vector3(cam.ScreenToWorldPoint(Input.mousePosition).x, cam.ScreenToWorldPoint(Input.mousePosition).y);
                        for (int j = 0; j < currentGun.count; j++)
                        {
                            angle = (Vector2.SignedAngle(Vector3.right, mousePos - transform.position) + UnityEngine.Random.Range(-currentGun.spread, currentGun.spread)) * Mathf.Deg2Rad;
                            dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
                            GameObject newBullet = Instantiate(currentGun.bullet, transform.position + dir.normalized * UnityEngine.Random.Range(0.1f, 1), Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir)));
                            newBullet.GetComponent<PlayerBulletInfo>().damage = currentGun.damage;
                            newBullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * currentGun.speed;
                        }
                        yield return new WaitForSeconds(currentGun.burst_spacing);
                    }
                }
                break;
            case weapon_type.charge:
                {
                    GameObject newBullet = Instantiate(currentGun.bullet, transform.position + dir.normalized, Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir)));
                    newBullet.GetComponent<PlayerBulletInfo>().damage = currentGun.damage;
                    newBullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * currentGun.speed;
                }
                break;
            default:
                {
                    if(currentGun.count > 1)
                    {
                        for(int i = 0; i < currentGun.count; i++)
                        {
                            angle = (Vector2.SignedAngle(Vector3.right, mousePos - transform.position) + UnityEngine.Random.Range(-gun.spread, gun.spread)) * Mathf.Deg2Rad;
                            dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
                            GameObject newBullet = Instantiate(currentGun.bullet, transform.position + dir.normalized * UnityEngine.Random.Range(0.1f, 1), Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir)));
                            newBullet.GetComponent<PlayerBulletInfo>().damage = currentGun.damage;
                            newBullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * currentGun.speed;
                        }
                    }
                    else
                    {
                        GameObject newBullet = Instantiate(currentGun.bullet, transform.position + dir.normalized, Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir)));
                        newBullet.GetComponent<PlayerBulletInfo>().damage = currentGun.damage;
                        newBullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * currentGun.speed;
                    }
                }
                break;
        }
        yield return new WaitForSeconds(fireTime);
        canAttack = true;
    }
}
