using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CircleMage : Enemy
{
    private BulletManager manager;
    public BulletPattern pattern1,pattern2;
    protected override void Start()
    {
        base.Start();
        manager = BulletManager.instance;
        Attacks.Add(attack1);
        Attacks.Add(attack2);
        atkPatternLen = Attacks.Count;
        current = 0;
        pattern = new int[atkPatternLen];
        PatternShuffle();
        if (parentLevel != null)
            this.gameObject.SetActive(false);
    }

    public void attack1()
    {
        StartCoroutine(forAttack1());
    }

    IEnumerator forAttack1()
    {
        Color originalColor = sr.color;
        isAttacking = true;
        rb.mass = 10000;
        sr.DOColor(Color.white, 0.75f);
        enemyTransform.DOShakePosition(0.75f, 0.5f, 15);
        yield return new WaitForSeconds(0.75f);
        sr.DOColor(originalColor, 0.5f);
        float angle = Vector2.SignedAngle(Vector2.right, playerTransform.transform.position - enemyTransform.transform.position);
        for (int i = 0; i < 20; i++)
        {
            float newAngle = Vector2.SignedAngle(Vector2.right, playerTransform.transform.position - enemyTransform.transform.position);
            if(Mathf.Abs(angle - newAngle) > 180)
            {
                angle = (angle < 0 ? angle + 360 : angle);
                newAngle = (newAngle < 0 ? newAngle + 360 : newAngle);
            }
            angle = Mathf.Lerp(angle, newAngle, 0.3f);
            manager.StartCoroutine(manager.SpawnPattern(pattern1, this.transform.position, Quaternion.Euler(0, 0, angle)));
            yield return new WaitForSeconds(0.05f);
        }
        isAttacking = false;
        rb.mass = 1;
    }

    public void attack2()
    {
        StartCoroutine(forAttack2());
    }

    IEnumerator forAttack2()
    {
        Color originalColor = sr.color;
        isAttacking = true;
        rb.mass = 10000;
        sr.DOColor(Color.white, 0.75f);
        enemyTransform.DOShakePosition(0.75f, 0.5f,15);
        yield return new WaitForSeconds(0.75f);
        sr.DOColor(originalColor, 0.5f);
        rb.mass = 10000;
        for (int i = 0; i < 5; i++)
        {
            float angle = Vector2.SignedAngle(Vector2.right, playerTransform.transform.position - enemyTransform.transform.position);
            manager.StartCoroutine(manager.SpawnPattern(pattern2, this.transform.position, Quaternion.Euler(0, 0, angle)));
            yield return new WaitForSeconds(0.4f);
        }
        isAttacking = false;
        rb.mass = 1;
    }

    protected override void Follow()
    {
        RaycastHit2D raycast = Physics2D.CircleCast(enemyTransform.position, 1f, playerTransform.position - enemyTransform.position, float.PositiveInfinity, 1 << 8 | 1 << 10);
        if (raycast.collider.gameObject.layer != 10)
            base.Follow();
        else
        {
            if (destination.isActiveAndEnabled)
            {
                iPath.enabled = false;
                destination.enabled = false;
            }
            rb.velocity = (playerTransform.position - enemyTransform.position).normalized * speed;
            Vector3 from = enemyTransform.transform.up;
            Vector3 to = enemyTransform.transform.position - playerTransform.transform.position;
            enemyTransform.transform.up = Vector3.Lerp(from, to, 0.05f);
        }
    }
}
