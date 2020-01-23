using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Blast : MonoBehaviour
{
    private SpriteRenderer sr;

    public void SetBlast(float radius, float damage, Color color)
    {
        StartCoroutine(StartBlast(radius, damage, color));
    }

    public IEnumerator StartBlast(float radius, float damage, Color color)
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = color;
        sr.DOFade(0, 1).SetEase(Ease.OutCubic);
        transform.DOScale(new Vector3(radius,radius), 1).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.5f);
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, radius, Vector2.zero, 0, 1 << 13 | 1 << 14);
        foreach(RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.TryGetComponent(out Enemy enemy))
            {
                enemy.hp -= damage;
                enemy.StartCoroutine(enemy.Hurt());
            }
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
