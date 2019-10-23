using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Circle : Weapon
{
    public override void Attack()
    {
        if (fireRate > 0 && canAttack && currentAmmo > 0)
            StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        canAttack = false;
        Vector3 mousePos = new Vector3(cam.ScreenToWorldPoint(Input.mousePosition).x, cam.ScreenToWorldPoint(Input.mousePosition).y);
        float angle = (Vector2.SignedAngle(Vector3.right, mousePos - transform.position) + UnityEngine.Random.Range(-spread, spread)) * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Cos(angle),Mathf.Sin(angle));
        GameObject newBullet = Instantiate(bullet, transform.position + dir.normalized * 0.6f, Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, dir)));
        newBullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * bulletSpeed;
        yield return new WaitForSeconds(fireTime);
        canAttack = true;
    }
}
