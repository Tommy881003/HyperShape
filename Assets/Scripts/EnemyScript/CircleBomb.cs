using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CircleBomb : Enemy
{
    protected static float changeRotation = 0; 
    private BulletManager manager;
    public BulletPattern diePattern;
    private CircleCollider2D cc;
    protected override void Start()
    {
        base.Start();
        manager = BulletManager.instance;
        Attacks.Add(attack1);
        CircleCollider2D[] ccs = enemyTransform.GetComponents<CircleCollider2D>();
        foreach(CircleCollider2D collider in ccs)
        {
            if (collider.isTrigger == false)
            {
                cc = collider;
                break;
            }
        }
        atkPatternLen = Attacks.Count;
        current = 0;
        pattern = new int[atkPatternLen];
        PatternShuffle();
        if (parentLevel != null)
            this.gameObject.SetActive(false);
    }

    public void attack1()
    {
        return;
    }

    protected override void Follow()
    {
        RaycastHit2D raycast = Physics2D.CircleCast(enemyTransform.position, 1f, playerTransform.position - enemyTransform.position, float.PositiveInfinity, 1 << 8 | 1 << 10);
        if (raycast.collider.gameObject.layer != 10)
            base.Follow();
        else
        {
            if (raycast.distance < 4)
            {
                OnDead();
                this.enabled = false;
                return;
            }
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
        StartCoroutine(Suicide());
    }

    protected IEnumerator Suicide()
    {
        rb.mass = 10000;
        this.enabled = false;
        rb.velocity = Vector2.zero;
        cc.enabled = false;
        destination.enabled = false;
        iPath.enabled = false;
        enemyTransform.DOScale(new Vector3(2.5f, 2.5f, 1), 0.5f).SetEase(Ease.InBack);
        enemyTransform.DOShakePosition(0.5f, 0.5f, 10).SetEase(Ease.InCubic);
        sr.DOColor(Color.white, 0.5f).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(0.5f);
        sr.enabled = false;
        float dieTime = dieParticle.main.duration;
        dieParticle.Play();
        manager.StartCoroutine(manager.SpawnPattern(diePattern, this.transform.position, Quaternion.Euler(0,0,changeRotation)));
        changeRotation += 22.5f;
        if (parentLevel != null && m_MyEvent != null)
        {
            m_MyEvent.Invoke();
            m_MyEvent.RemoveAllListeners();
        }
        yield return new WaitForSeconds(dieTime);
        Destroy(this.gameObject);
    }
}
