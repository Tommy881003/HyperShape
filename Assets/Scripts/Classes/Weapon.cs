using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Weapon : MonoBehaviour
{
    public float reloadTime;
    public int ammoCapacity;
    public float fireRate;
    public float damage;
    public float spread;
    public Weapon previous;

    protected Camera cam;
    protected float fireTime;
    protected CameraFollower follower;
    public GameObject bullet;
    [Range(10f, 50f)]
    public float bulletSpeed = 20;
    [HideInInspector]
    public int currentAmmo;
    [HideInInspector]
    public bool canAttack = true;

    private void Start()
    {
        currentAmmo = ammoCapacity;
        cam = Camera.main;
        follower = cam.GetComponent<CameraFollower>();
        if (fireRate > 0)
            fireTime = 1 / fireRate;
        else
            Debug.LogWarning("The fire rate should always be positive");
    }

    public virtual void Attack()
    {
        if (fireRate > 0 && canAttack && currentAmmo > 0)
        {
            StartCoroutine(Shoot());
            StartCoroutine(follower.CamShake(0.3f));
        } 
    }

    public virtual IEnumerator Shoot()
    {
        yield return null;
    }
}
