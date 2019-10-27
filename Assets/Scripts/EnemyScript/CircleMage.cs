using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CircleMage : Enemy
{
    private BulletManager manager;
    public BulletPattern pattern1;
    protected override void Start()
    {
        base.Start();
        manager = BulletManager.instance;
        Attacks.Add(attack1);
        atkPatternLen = Attacks.Count;
        current = 0;
        pattern = new int[atkPatternLen];
        PatternShuffle();
    }

    public void attack1()
    {
        StartCoroutine(forAttack1());
    }

    IEnumerator forAttack1()
    {
        isAttacking = true;
        rb.mass = 1000;
        float angle = Vector2.SignedAngle(Vector2.right, playerTransform.transform.position - enemyTransform.transform.position);
        for (int i = 0; i < 20; i++)
        {
            float newAngle = Vector2.SignedAngle(Vector2.right, playerTransform.transform.position - enemyTransform.transform.position);
            angle = Mathf.Lerp(angle, newAngle, 0.3f);
            manager.StartCoroutine(manager.SpawnPattern(pattern1, this.transform.position, Quaternion.Euler(0, 0, angle)));
            yield return new WaitForSeconds(0.05f);
        }
        isAttacking = false;
        rb.mass = 1;
    }

    /*public void attack2()
    {
        float distance = UnityEngine.Random.Range(10, 15);
    }

    IEnumerator forattack2()
    {
        timer = 1;
        isAttacking = true;
        Vector3 originalScale = enemyTransform.localScale;
        enemyTransform.DOScale(Vector3.zero, 0.5f);
        yield return new WaitForSeconds(1);
        float distance = UnityEngine.Random.Range(10, 15);
        for(int i = 0; i < 24; i++)
        {
            Vector3 dis = distance * new Vector3(Mathf.Cos(i * 15 * Mathf.Deg2Rad), Mathf.Sin(i * 15 * Mathf.Deg2Rad));
            RaycastHit2D raycast = Physics2D.CircleCast(playerTransform.position, 1f, playerTransform.position - enemyTransform.position, float.PositiveInfinity, 1 << 8 | 1 << 9);
        }
    }*/

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
