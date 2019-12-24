using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleRifleGuy : Enemy
{
    private BulletManager manager;
    public BulletPattern pattern1;
    public ObjAudioManager audios;
    protected override void Start()
    {
        base.Start();
        audios = GetComponent<ObjAudioManager>();
        manager = BulletManager.instance;
        Attacks.Add(attack1);
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
        isAttacking = true;
        float angle = Vector2.SignedAngle(Vector2.right, playerTransform.transform.position - enemyTransform.transform.position);
        for (int i = 0; i < 10; i++)
        {
            float newAngle = Vector2.SignedAngle(Vector2.right, playerTransform.transform.position - enemyTransform.transform.position);
            if (Mathf.Abs(angle - newAngle) > 180)
            {
                angle = (angle < 0 ? angle + 360 : angle);
                newAngle = (newAngle < 0 ? newAngle + 360 : newAngle);
            }
            angle = Mathf.Lerp(angle, newAngle, 0.8f) + Random.Range(-10,10);
            audios.PlayByName("atk");
            manager.StartCoroutine(manager.SpawnPattern(pattern1, this.transform.position, Quaternion.Euler(0, 0, angle)));
            yield return new WaitForSeconds(0.2f);
        }
        isAttacking = false;
    }

    protected override bool CanFollow()
    {
        return true;
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

    protected override void OnDead()
    {
        base.OnDead();
        audios.PlayByName("die");
    }
}
