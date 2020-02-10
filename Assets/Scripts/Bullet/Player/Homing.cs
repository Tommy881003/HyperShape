using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homing : PlayerBulletInfo
{
    private float velocityValue;
    private float velocityAngle;
    private float rotateStr;
    public ParticleSystem trail;
    private Transform enemy = null;


    protected override void Update()
    {
        if (info.speedType == SpeedType.ConstantDecay || info.speedType == SpeedType.RandomDecay)
            velocityValue *= 0.99f;
        if (info.sizeType == SizeType.Decay)
            transform.localScale *= 0.99f;
        if (velocityValue < 2 || transform.localScale.x < 0.1f)
            Destroy(gameObject);
    }

    protected override void FixedUpdate()
    {
        if(enemy != null)
        {
            Vector2 enemyDir = (enemy.position - transform.position).normalized;
            Vector2 myDir = new Vector2(Mathf.Cos(velocityAngle), Mathf.Sin(velocityAngle));
            float rotate = Vector3.Cross(myDir, enemyDir).z;
            velocityAngle += rotateStr * rotate;
        }
        velocity = velocityValue * new Vector2(Mathf.Cos(velocityAngle), Mathf.Sin(velocityAngle));
        transform.position += velocity * Time.fixedDeltaTime;
    }

    protected override void LateUpdate()
    {
        if (!isDisabled && frameCounter % 2 == 0)
        {
            RaycastHit2D hit = Physics2D.CircleCast(previous, 0.5f * transform.localScale.x, transform.position - previous, (transform.position - previous).magnitude, 1 << 8 | 1 << 9 | 1 << 13 | 1 << 14);
            if (hit.collider != null)
            {
                transform.position = hit.point;
                if (hit.collider.gameObject.TryGetComponent(out Enemy enemy))
                {
                    enemy.hp -= damage;
                    enemy.StartCoroutine(enemy.Hurt());
                }
                OnHit(hit);
                StartCoroutine(DelayDestroy());
            }
            previous = transform.position;
        }
        frameCounter++;
    }

    protected override void Reflect(Vector2 normal)
    {
        return;
    }

    protected override void Flak(Vector2 normal)
    {
        WeaponInfo newInfo;
        info.ValueCopy(out newInfo);
        newInfo.size *= newInfo.flakSizeDmg;
        newInfo.damage = damage * newInfo.flakSizeDmg;
        newInfo.speedType = SpeedType.Random;
        newInfo.flak = false;
        for (int i = 0; i < newInfo.flakCount; i++)
        {
            float angle = (Vector2.SignedAngle(Vector2.right, normal) + Random.Range(-75, 75)) * Mathf.Deg2Rad;
            GameObject newBullet = Instantiate(newInfo.bullet, transform.position + 0.6f * transform.localScale.x * (Vector3)normal.normalized, Quaternion.identity);
            newBullet.GetComponent<PlayerBulletInfo>().SetBullet(newInfo, angle);
        }
    }

    public override void SetBullet(WeaponInfo weapon, float angle)
    {
        enabled = false;
        info = weapon;
        damage = info.damage;
        velocityValue = info.speed;
        velocityAngle = angle;
        if (info.speedType == SpeedType.Random || info.speedType == SpeedType.RandomDecay)
            velocityValue *= Random.Range(0.8f, 1.2f);
        velocity = velocityValue * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        transform.localScale *= info.size;
        ParticleSystem.MainModule mainModule = trail.main;
        mainModule.startSize = new ParticleSystem.MinMaxCurve(transform.localScale.x);
        rotateStr = Random.Range(0.2f, 0.4f) * (info.speed / 50f);
        enabled = true;
        InvokeRepeating("FindEnemy", 0.25f, 0.25f);
    }

    private void FindEnemy()
    {
        float dis = 10000;
        Transform best = null;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 100, Vector2.zero, 0, 1 << 9 | 1 << 13 | 1 << 14);
        foreach(RaycastHit2D hit in hits)
        {
            if ((hit.collider.gameObject.transform.position - transform.position).magnitude < dis)
            {
                dis = (hit.collider.gameObject.transform.position - transform.position).magnitude;
                best = hit.collider.gameObject.transform;
            }
        }
        enemy = best;
    }
}
