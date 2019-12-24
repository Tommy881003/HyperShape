using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaShotgun : PlayerBulletInfo
{
    public float bulletSpeed = 35;
    public float bulletDmg = 4;
    public GameObject bullet;

    public override IEnumerator DelayDestroy()
    {
        isDisabled = true;
        foreach (Collider2D c in GetComponents<Collider2D>())
        {
            c.enabled = false;
        }
        if (sr != null)
            sr.enabled = false;
        foreach (SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>())
        {
            s.enabled = false;
        }
        float rand = Random.Range(-30, 30);
        for(int i = 0; i < 6; i++)
        {
            float angle = (60 * i + rand) * Mathf.Deg2Rad;
            GameObject newBullet = Instantiate(bullet, transform.position + 0.5f * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)), Quaternion.identity);
            newBullet.GetComponent<Rigidbody2D>().velocity = bulletSpeed * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            newBullet.GetComponent<PlayerBulletInfo>().damage = bulletDmg;
        }
        rb.velocity = Vector2.zero;
        Destroy(rb);
        ps.Play();
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
