using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ricocheter : PlayerBulletInfo
{
    private int bounce = 5;
    public float speed = 75;

    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (rb.velocity.magnitude < 5)
            StartCoroutine(DelayDestroy()); 
    }
    public override IEnumerator DelayDestroy()
    {
        if(bounce > 0)
        {
            bounce--;
            damage *= 0.85f;
            yield break;
        }
        enabled = false;
        isDisabled = true;
        foreach (Collider2D c in GetComponents<Collider2D>())
        {
            c.enabled = false;
        }
        if (sr != null)
            sr.enabled = false;
        foreach (SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>())
        {
            s.enabled = false;
        }
        rb.velocity = Vector2.zero;
        Destroy(rb);
        ps.Play();
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
