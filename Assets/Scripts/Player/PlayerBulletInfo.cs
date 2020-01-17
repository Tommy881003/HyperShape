﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletInfo : MonoBehaviour
{
    public float damage;
    protected Vector3 previous;
    protected Rigidbody2D rb = null;
    protected SpriteRenderer sr = null;
    protected ParticleSystem ps = null;
    protected SceneAudioManager manager;
    protected AudioSource source = null;
    public bool isDisabled = false;

    protected virtual void Start()
    {
        previous = transform.position;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        ps = GetComponent<ParticleSystem>();
        manager = SceneAudioManager.instance;
        if(TryGetComponent(out source))
        {
            source.volume *= manager.fxAmp;
            source.Play();
        }
    }

    protected virtual void LateUpdate()
    {
        if (!isDisabled)
        {
            RaycastHit2D hit = Physics2D.CircleCast(previous, 0.2f, transform.position - previous, (transform.position - previous).magnitude, 1 << 8 | 1 << 9 | 1 << 13 | 1 << 14);
            if(hit.collider != null)
            {
                transform.position = hit.point;
                rb.velocity = Vector2.zero;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                if (hit.collider.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    enemy.hp -= damage;
                    enemy.StartCoroutine(enemy.Hurt());
                    StartCoroutine(DelayDestroy());
                }
            }
            previous = transform.position;
        }
    }

    public virtual IEnumerator DelayDestroy()
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