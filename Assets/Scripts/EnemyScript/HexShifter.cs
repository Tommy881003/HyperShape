﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HexShifter : Enemy
{
    private BulletManager manager;
    private Vector3 scale;
    public BulletPattern pattern1,diePattern;
    public ObjAudioManager audios;
    protected override void Start()
    {
        base.Start();
        audios = GetComponent<ObjAudioManager>();
        scale = transform.localScale;
        manager = BulletManager.instance;
        Attacks.Add(attack1);
        atkPatternLen = Attacks.Count;
        current = 0;
        pattern = new int[atkPatternLen];
        PatternShuffle();
        if (parentLevel != null)
            this.gameObject.SetActive(false);
    }

    protected override bool CanAttack()
    { 
        if (timer <= 0)
        {
            timer = coolDown;
            return true;
        }
        else
            return false;
    }

    public void attack1()
    {
        StartCoroutine(forAttack1());
    }

    IEnumerator forAttack1()
    {
        isAttacking = true;
        for(int i = 0; i < 2; i++)
        {
            rb.constraints = RigidbodyConstraints2D.None;
            enemyTransform.localScale = 0.5f * scale;
            enemyTransform.DOMove(rayDir(), 0.2f);
            enemyTransform.DOScale(scale, 0.2f);
            audios.PlayByName("cha");
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            yield return new WaitForSeconds(0.4f);
        }
        Color oriColor = sr.color;
        sr.DOColor(Color.white,0.5f);
        enemyTransform.DOScale(1.3f * scale, 0.5f);
        yield return new WaitForSeconds(0.5f);
        transform.DOShakePosition(0.5f, 0.5f, 10);
        enemyTransform.DOScale(scale, 0.5f);
        sr.DOColor(oriColor, 0.5f);
        audios.PlayByName("atk");
        float angle = Vector2.SignedAngle(Vector2.right, playerTransform.transform.position - enemyTransform.transform.position);
        manager.StartCoroutine(manager.SpawnPattern(pattern1, this.transform.position, Quaternion.Euler(0, 0, angle)));
        yield return new WaitForSeconds(0.4f);
        rb.constraints = RigidbodyConstraints2D.None;
        isAttacking = false;
    }

    Vector3 rayDir()
    {
        List<Vector3> list = new List<Vector3>();
        float degree = 0;
        Vector3 currentPos = new Vector3(enemyTransform.position.x, enemyTransform.position.y);
        Vector3 bestVector = new Vector3();
        float distance = float.MaxValue;
        while(list.Count == 0)
        {
            for (int i = 0; i < 24; i++)
            {
                degree = 15 * i * Mathf.Deg2Rad;
                RaycastHit2D raycast = Physics2D.CircleCast(enemyTransform.position, 0.8f, new Vector2(Mathf.Cos(degree), Mathf.Sin(degree)), 8, 1 << 8);
                if (raycast.collider == null)
                    list.Add(currentPos + new Vector3(8 * Mathf.Cos(degree), 8 * Mathf.Sin(degree)));
            }
        }
        foreach (Vector3 vector in list)
        {
            float newDis = (vector - playerTransform.position).magnitude;
            if (newDis < distance && newDis >= 6)
            {
                distance = newDis;
                bestVector = vector;
            }
        }
        return bestVector;
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

    protected override IEnumerator playDead()
    {
        sr.material = oriSr;
        rb.velocity = Vector2.zero;
        foreach (Collider2D c in colliders)
            c.enabled = false;
        destination.enabled = false;
        iPath.enabled = false;
        enemyTransform.DOScale(0.6f * scale, 1f).SetEase(Ease.Linear);
        enemyTransform.DOShakePosition(1f, 0.5f, 20).SetEase(Ease.Linear);
        sr.DOColor(Color.white, 1f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(1f);
        float dieTime = Mathf.Max(dieParticle.main.duration - 0.2f,0);
        audios.PlayByName("die");
        dieParticle.Play();
        manager.StartCoroutine(manager.SpawnPattern(diePattern, this.transform.position, Quaternion.identity));
        if (parentLevel != null && m_MyEvent != null)
        {
            m_MyEvent.Invoke();
            m_MyEvent.RemoveAllListeners();
        }
        yield return new WaitForSeconds(0.2f);
        sr.enabled = false;
        giveShard();
        yield return new WaitForSeconds(dieTime);
        Destroy(this.gameObject);
    }
}
