using System.Collections;
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
    protected WeaponInfo info = null;
    public bool isDisabled = false;

    protected virtual void Start()
    {
        enabled = false;
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
        enabled = true;
    }

    protected virtual void Update()
    {
        if (info.speedType == SpeedType.ConstantDecay || info.speedType == SpeedType.RandomDecay)
            rb.velocity *= 0.99f;
        if (info.sizeType == SizeType.Decay)
            transform.localScale *= 0.99f;
        if (rb.velocity.magnitude < 1 || transform.localScale.x < 0.05f)
            StartCoroutine(DelayDestroy());
    }

    protected virtual void LateUpdate()
    {
        if (!isDisabled)
        {
            RaycastHit2D hit = Physics2D.CircleCast(previous, 0.5f * transform.localScale.x, transform.position - previous, (transform.position - previous).magnitude, 1 << 8 | 1 << 9 | 1 << 13 | 1 << 14);
            if(hit.collider != null)
            {
                transform.position = hit.point;
                /*rb.velocity = Vector2.zero;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;*/
                if (hit.collider.gameObject.TryGetComponent(out Enemy enemy))
                {
                    enemy.hp -= damage;
                    enemy.StartCoroutine(enemy.Hurt());
                }
                OnHit(hit);
                StartCoroutine(DelayDestroy());
            }
            previous = transform.position;
        }
    }

    public virtual IEnumerator DelayDestroy()
    {
        enabled = false;
        isDisabled = true;
        foreach (Collider2D c in GetComponents<Collider2D>())
            c.enabled = false;
        if(sr != null)
            sr.enabled = false;
        foreach (SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>())
            s.enabled = false;
        rb.velocity = Vector2.zero;
        Destroy(rb);
        ps.Play();
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    protected void OnHit(RaycastHit2D hit)
    {

        if (info.blast)
            Debug.Log("blast");
        if (info.flak)
            Debug.Log("flak");
    }

    public void SetBullet(WeaponInfo weapon, float angle) 
    {
        enabled = false;
        info = weapon;
        damage = info.damage;
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = info.speed * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        if (info.speedType == SpeedType.Random || info.speedType == SpeedType.RandomDecay)
            rb.velocity *= Random.Range(0.8f, 1.2f);
        transform.localScale *= info.size;
        enabled = true;
    }
}
