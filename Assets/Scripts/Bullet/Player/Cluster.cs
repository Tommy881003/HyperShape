using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Cluster : PlayerBulletInfo
{
    private Vector3 halfVelocity, oriVelocity;

    protected override void Update()
    {
        if (info.speedType == SpeedType.ConstantDecay || info.speedType == SpeedType.RandomDecay)
            velocity *= 0.995f;
        if (info.sizeType == SizeType.Decay)
            transform.localScale *= 0.998f;
        if (velocity.magnitude < 0.1f || transform.localScale.x < 0.1f)
            StartCoroutine(DelayDestroy());
    }

    public override void SetBullet(WeaponInfo weapon, float angle)
    {
        base.SetBullet(weapon, angle);
        oriVelocity = velocity;
        halfVelocity = velocity * 0.5f;
        transform.DORotate(new Vector3(0, 0, 360f), 1, RotateMode.LocalAxisAdd).SetLoops(-1,LoopType.Incremental).SetEase(Ease.Linear);
        InvokeRepeating("Attack", 0.1f, 0.1f);
    }

    protected override void Reflect(Vector2 normal)
    {
        base.Reflect(normal);
        oriVelocity = velocity;
        halfVelocity = velocity * 0.5f;
    }

    protected override void LateUpdate()
    {
        if (!isDisabled && frameCounter % 2 == 0)
        {
            RaycastHit2D hit = Physics2D.CircleCast(previous, 0.5f * transform.localScale.x, transform.position - previous, (transform.position - previous).magnitude, 1 << 8);
            if (hit.collider != null)
            {
                transform.position = hit.point;
                OnHit(hit);
                if(reflectCount > info.reflectCount)
                    StartCoroutine(DelayDestroy());
            }
            previous = transform.position;
        }
        frameCounter++;
    }

    public override IEnumerator DelayDestroy()
    {
        CancelInvoke();
        transform.DOKill();
        enabled = false;
        isDisabled = true;
        foreach (Collider2D c in GetComponents<Collider2D>())
            c.enabled = false;
        if (sr != null)
            sr.enabled = false;
        foreach (SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>())
            s.enabled = false;
        velocity = Vector2.zero;
        ps.Play();
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    private void Attack()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 3f * transform.localScale.x, Vector2.zero, 0, 1 << 9 | 1 << 13 | 1 << 14);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.TryGetComponent(out Enemy enemy))
            {
                enemy.hp -= damage / 4;
                enemy.StartCoroutine(enemy.Hurt());
                if (enemy.maxHp > 1000)
                    velocity = halfVelocity;
                else
                    velocity = halfVelocity * 0.5f;
            }
        }
        if (hits.Length == 0)
            velocity = oriVelocity;
    }
}
