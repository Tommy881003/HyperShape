using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletInfo : MonoBehaviour
{
    public static int currentBulletAmount = 0;

    public float damage;
    protected Vector3 previous;
    protected Vector3 velocity;
    protected SpriteRenderer sr = null;
    protected ParticleSystem ps = null;
    protected SceneAudioManager manager;
    protected AudioSource source = null;
    protected WeaponInfo info = null;
    public bool isDisabled = false;
    protected int reflectCount = 0;
    protected int frameCounter = 0;

    protected virtual void Start()
    {
        currentBulletAmount++;
        enabled = false;
        previous = transform.position;
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
            velocity *= 0.99f;
        if (info.sizeType == SizeType.Decay)
            transform.localScale *= 0.99f;
        if (velocity.magnitude < 2 || transform.localScale.x < 0.1f)
            StartCoroutine(DelayDestroy());
    }

    protected virtual void FixedUpdate()
    {
        transform.position += velocity * Time.fixedDeltaTime;    
    }

    protected virtual void LateUpdate()
    {
        if (!isDisabled && frameCounter % 2 == 0)
        {
            RaycastHit2D hit = Physics2D.CircleCast(previous, 0.5f * transform.localScale.x, transform.position - previous, (transform.position - previous).magnitude, 1 << 8 | 1 << 9 | 1 << 13 | 1 << 14);
            if(hit.collider != null)
            {
                transform.position = hit.point;
                if (hit.collider.gameObject.TryGetComponent(out Enemy enemy))
                {
                    enemy.hp -= damage;
                    enemy.StartCoroutine(enemy.Hurt());
                }
                OnHit(hit);
                if(reflectCount > info.reflectCount)
                    StartCoroutine(DelayDestroy());
            }
            previous = transform.position;
        }
        frameCounter++;
    }

    /*消滅子彈(異步消滅)*/
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
        velocity = Vector2.zero;
        ps.Play();
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        currentBulletAmount--;
    }

    /*子彈打到某個東西(牆壁、敵人)時呼叫(例如反彈、破片、衝擊波)*/
    protected void OnHit(RaycastHit2D hit)
    {
        Vector2 normal = hit.normal;
        if (info.reflectCount >= reflectCount)
        {
            reflectCount++;
            damage *= 0.8f;
            velocity = Vector2.Reflect(velocity, normal);
            transform.position += 0.55f * transform.localScale.x * new Vector3(normal.x, normal.y).normalized;
            previous = transform.position;
        }
        if (info.blast)
            Debug.Log("blast");
        if (info.flak)
        {
            WeaponInfo newInfo;
            info.ValueCopy(out newInfo);
            newInfo.size *= newInfo.flakSizeDmg;
            newInfo.damage *= newInfo.flakSizeDmg;
            newInfo.speedType = SpeedType.RandomDecay;
            newInfo.flak = false;
            for (int i = 0; i < newInfo.flakCount; i++)
            {
                float angle = (Vector2.SignedAngle(Vector2.right, normal) + Random.Range(-75,75)) * Mathf.Deg2Rad;
                GameObject newBullet = Instantiate(newInfo.bullet, transform.position, Quaternion.identity);
                newBullet.GetComponent<PlayerBulletInfo>().SetBullet(newInfo,angle);
            }
        }
    }

    /*植入武器參數(這函數有很大機率會跟Start()同步執行，所以如果需要component要額外呼叫)*/
    public void SetBullet(WeaponInfo weapon, float angle) 
    {
        enabled = false;
        info = weapon;
        damage = info.damage;
        velocity = info.speed * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        if (info.speedType == SpeedType.Random || info.speedType == SpeedType.RandomDecay)
            velocity *= Random.Range(0.8f, 1.2f);
        transform.localScale *= info.size;
        enabled = true;
    }
}
