using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class testscript : Enemy
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
        Color ori = sr.color;
        Vector3 oriScale = transform.localScale;
        sr.DOColor(Color.white, 0.5f);
        transform.DOScale(1.25f * oriScale, 0.5f);
        audios.PlayByName("cha");
        yield return new WaitForSeconds(0.5f);
        transform.DOShakePosition(0.5f, 0.5f, 10);
        sr.DOColor(ori, 0.5f);
        transform.DOScale(oriScale, 0.5f);
        audios.PlayByName("atk");
        float angle = Vector2.SignedAngle(Vector2.right, playerTransform.transform.position - enemyTransform.transform.position);
        manager.StartCoroutine(manager.SpawnPattern(pattern1, this.transform.position, Quaternion.Euler(0, 0, angle)));
    }

    protected override void Follow()
    {
        RaycastHit2D raycast = Physics2D.CircleCast(enemyTransform.position, 1.5f, playerTransform.position - enemyTransform.position, float.PositiveInfinity, 1 << 8 | 1 << 10);
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
            enemyTransform.transform.up = Vector3.Lerp(from, to, 0.025f);
        }
    }

    protected override void OnDead()
    {
        base.OnDead();
        audios.PlayByName("die");
    }
}
