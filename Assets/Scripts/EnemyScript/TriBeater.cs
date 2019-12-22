using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TriBeater : Enemy
{
    private BulletManager manager;
    public BulletPattern pattern1;
    private int count;
    protected override void Start()
    {
        base.Start();
        count = Random.Range(0, 3);
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
        Color ori = sr.color;
        Vector3 oriScale = transform.localScale;
        Vector3 oriTransform = transform.position;
        sr.DOColor(Color.white, 0.5f);
        if(count % 3 == 0)
            transform.DOMove(oriTransform + 2 * new Vector3(0, 0.5f), 0.5f);
        else if(count % 3 == 1)
            transform.DOMove(oriTransform + 2 * new Vector3(0.433f, -0.25f), 0.5f);
        else
            transform.DOMove(oriTransform + 2 * new Vector3(-0.433f, -0.25f), 0.5f);
        yield return new WaitForSeconds(0.5f);
        manager.StartCoroutine(manager.SpawnPattern(pattern1, this.transform.position, Quaternion.Euler(0, 0, 30 + 120 * (2 - count % 3))));
        count++;
        sr.DOColor(ori, 0.5f);
        transform.DOShakePosition(0.5f,0.5f);
        transform.DOMove(oriTransform, 0.5f).SetEase(Ease.OutBack);
    }

    protected override bool CanAttack()//可以改寫
    {
        bool canATK = timer <= 0;
        if (canATK)
            timer = coolDown;
        return canATK;
    }

    protected override void Follow()
    {
        return;
    }
}
