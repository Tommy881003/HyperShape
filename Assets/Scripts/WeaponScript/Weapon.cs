using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Weapon : MonoBehaviour
{
    public WeaponInfo gun;
    protected Camera cam;
    protected CameraFollower follower;
    [HideInInspector]
    public float fireTime;
    [HideInInspector]
    public int currentAmmo;
    [HideInInspector]
    public bool canAttack = true;
    private float strength;
    private float time;

    private void Start()
    {
        currentAmmo = gun.ammoCapacity;
        cam = Camera.main;
        follower = cam.GetComponent<CameraFollower>();
        if (gun.fireRate > 0)
            fireTime = 1 / gun.fireRate;
        else
            Debug.LogWarning("The fire rate should always be positive");
        strength = Mathf.Lerp(0.5f, 3,Mathf.Clamp01((gun.damage * gun.count)/40f));
        time = Mathf.Clamp(fireTime, 0.1f, 0.3f);
    }

    private void Update()
    {
        fireTime = 1 / gun.fireRate;
        if (canAttack && ((gun.type == weapon_type.auto && Input.GetMouseButton(0)) || Input.GetMouseButtonDown(0)))
        {
            if(currentAmmo > 0)
            {
                StartCoroutine(Shoot());
                StartCoroutine(follower.CamShake(strength, time));
            }
        }
    }

    public IEnumerator Shoot()
    {
        canAttack = false;
        Vector3 mousePos = new Vector3(cam.ScreenToWorldPoint(Input.mousePosition).x, cam.ScreenToWorldPoint(Input.mousePosition).y);
        float angle = (Vector2.SignedAngle(Vector3.right, mousePos - transform.position) + UnityEngine.Random.Range(-gun.spread, gun.spread)) * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        switch(gun.type)
        {
            case weapon_type.burst:
                {
                    for (int i = 0; i < gun.burst_count; i++)
                    {
                        mousePos = new Vector3(cam.ScreenToWorldPoint(Input.mousePosition).x, cam.ScreenToWorldPoint(Input.mousePosition).y);
                        for (int j = 0; j < gun.count; j++)
                        {
                            angle = (Vector2.SignedAngle(Vector3.right, mousePos - transform.position) + UnityEngine.Random.Range(-gun.spread, gun.spread)) * Mathf.Deg2Rad;
                            dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
                            GameObject newBullet = Instantiate(gun.bullet, transform.position + dir.normalized * UnityEngine.Random.Range(0.1f, 1), Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir)));
                            newBullet.GetComponent<PlayerBulletInfo>().damage = gun.damage;
                            newBullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * gun.speed;
                        }
                        yield return new WaitForSeconds(gun.burst_spacing);
                    }
                }
                break;
            case weapon_type.charge:
                {
                    GameObject newBullet = Instantiate(gun.bullet, transform.position + dir.normalized, Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir)));
                    newBullet.GetComponent<PlayerBulletInfo>().damage = gun.damage;
                    newBullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * gun.speed;
                }
                break;
            default:
                {
                    if(gun.count > 1)
                    {
                        for(int i = 0; i < gun.count; i++)
                        {
                            angle = (Vector2.SignedAngle(Vector3.right, mousePos - transform.position) + UnityEngine.Random.Range(-gun.spread, gun.spread)) * Mathf.Deg2Rad;
                            dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
                            GameObject newBullet = Instantiate(gun.bullet, transform.position + dir.normalized * UnityEngine.Random.Range(0.1f, 1), Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir)));
                            newBullet.GetComponent<PlayerBulletInfo>().damage = gun.damage;
                            newBullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * gun.speed;
                        }
                    }
                    else
                    {
                        GameObject newBullet = Instantiate(gun.bullet, transform.position + dir.normalized, Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir)));
                        newBullet.GetComponent<PlayerBulletInfo>().damage = gun.damage;
                        newBullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * gun.speed;
                    }
                }
                break;
        }
        yield return new WaitForSeconds(fireTime);
        canAttack = true;
    }
}
