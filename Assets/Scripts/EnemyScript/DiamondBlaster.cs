using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DiamondBlaster : Enemy
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
        rb.mass = 10000;
        Color orignialColor = sr.color;
        Vector3 scale = this.transform.localScale;
        sr.DOColor(Color.white, 0.5f).SetEase(Ease.OutCubic);
        enemyTransform.DOScale(2f * scale, 0.5f).SetEase(Ease.OutCubic);
        enemyTransform.DOShakePosition(0.5f, 0.2f, 10, 90).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(0.5f);
        manager.StartCoroutine(manager.SpawnPattern(pattern1, this.transform.position, Quaternion.identity));
        sr.DOColor(orignialColor, 0.5f).SetEase(Ease.OutBack);
        enemyTransform.DOScale(scale, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.5f);
        rb.mass = 1;
        isAttacking = false;
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
        }
    }
}
