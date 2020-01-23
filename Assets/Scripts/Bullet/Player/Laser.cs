using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Laser : PlayerBulletInfo
{
    private LineRenderer lr;
    private float timer = 0;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        lr.SetPosition(lr.positionCount - 1, transform.position);
        timer += Time.fixedDeltaTime;
        if (timer > 2 && isDisabled == false)
            StartCoroutine(DelayDestroy());
    }

    protected override void LateUpdate()
    {
        if (!isDisabled)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(previous, 0.5f * transform.localScale.x, transform.position - previous, (transform.position - previous).magnitude, 1 << 8 | 1 << 9 | 1 << 13 | 1 << 14);
            foreach(RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                {

                    if (hit.collider.gameObject.TryGetComponent(out Enemy enemy))
                    {
                        enemy.hp -= damage;
                        enemy.StartCoroutine(enemy.Hurt());
                    }
                    else
                    {
                        transform.position = hit.point;
                        OnHit(hit);
                        break;
                    }
                }
            }
            if (reflectCount > info.reflectCount)
                StartCoroutine(DelayDestroy());
            previous = transform.position;
        }
    }

    protected override void Reflect(Vector2 normal)
    {
        lr.SetPosition(lr.positionCount - 1, transform.position - 0.1f * velocity.normalized);
        lr.positionCount++;
        lr.SetPosition(lr.positionCount - 1, transform.position);
        lr.positionCount++;
        reflectCount++;
        damage *= 0.8f;
        velocity = Vector2.Reflect(velocity, normal);
        transform.position += 0.55f * transform.localScale.x * new Vector3(normal.x, normal.y).normalized;
        previous = transform.position;
        lr.SetPosition(lr.positionCount - 1, transform.position + 0.1f * velocity.normalized);
        lr.positionCount++;
        lr.SetPosition(lr.positionCount - 1, transform.position);
    }

    public override IEnumerator DelayDestroy()
    {
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
        DOTween.To(() => lr.startWidth, x => lr.startWidth = x, 0, 0.5f);
        DOTween.To(() => lr.endWidth, x => lr.endWidth = x, 0, 0.5f);
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    public override void SetBullet(WeaponInfo weapon, float angle)
    {
        base.SetBullet(weapon, angle);
        lr = GetComponent<LineRenderer>();
        sr = GetComponent<SpriteRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position);
        lr.startColor = sr.color;
        lr.endColor = sr.color;
        lr.startWidth = transform.localScale.x;
        lr.endWidth = transform.localScale.x;
    }
}
