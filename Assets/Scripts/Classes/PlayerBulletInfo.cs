using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletInfo : MonoBehaviour
{
    public float damage;
    private Vector3 previous;
    private Rigidbody2D rb = null;
    private SpriteRenderer sr = null;
    private ParticleSystem ps = null;
    public bool isDisabled = false;

    private void Start()
    {
        previous = transform.position;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        ps = GetComponent<ParticleSystem>();
    }

    private void LateUpdate()
    {
        if (!isDisabled)
        {
            RaycastHit2D hit = Physics2D.CircleCast(previous, 0.2f, transform.position - previous, (transform.position - previous).magnitude, 1 << 8 | 1 << 9 | 1 << 13 | 1 << 14);
            if(hit.collider != null)
            {
                transform.position = hit.point;
                rb.velocity = Vector2.zero;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            previous = transform.position;
        }
    }

    public IEnumerator DelayDestroy()
    {
        isDisabled = true;
        foreach (Collider2D c in GetComponents<Collider2D>())
        {
            c.enabled = false;
        }
        if(sr != null)
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
