using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;
using Pathfinding;
using System.Linq;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public int wave;
    protected static GameObject shard = null, health = null;
    protected static Material hurt = null;
    Root aiRoot = BT.Root();
    protected PlayerController player;
    protected Transform playerTransform;
    protected Transform enemyTransform;
    protected AIDestinationSetter destination;
    protected AIPath iPath;
    protected Rigidbody2D rb;
    protected Collider2D[] colliders;
    protected ParticleSystem dieParticle;
    protected SpriteRenderer sr;
    protected Level parentLevel = null;
    protected UnityEvent m_MyEvent = new UnityEvent();

    public float maxHp;
    [SerializeField,Header("生命值"),Range(1,10000)]
    public float hp = 50;
    [SerializeField, Header("速度"), Range(0, 100)]
    protected float speed;
    [SerializeField, Header("冷卻時間"), Range(0.5f, 20)]
    protected float coolDown = 4f;
    [SerializeField, Header("是否洗牌?")]
    protected bool shuffle;
    [SerializeField, Header("碎片數量"), Range(0, 25)]
    protected int shardNum = 0;
    protected float timer = 0;
    [SerializeField]
    public List<System.Action> Attacks = new List<System.Action>();
    protected int current = 0;
    protected int atkPatternLen = 0;
    protected int[] pattern;
    protected bool isAttacking;
    protected int currentShard = 0;
    protected Material oriSr;

    //AI邏輯：
    //IF(怪物血量歸零) => 死亡function(放特效、音效，死亡時攻擊之類的)
    //ELSE IF(可以攻擊) => 攻擊
    //ELSE => 跟隨玩家
    private void OnEnable()
    {
        aiRoot.OpenBranch(
                BT.Selector().OpenBranch(
                    BT.If(() => hp <= 0).OpenBranch(
                        BT.Call(OnDead)
                    ),
                    BT.If(CanAttack).OpenBranch(
                        BT.Call(Attack)
                    ),
                    BT.If(CanFollow).OpenBranch(
                        BT.Call(Follow)
                    ),
                    BT.Call(DoNothing)
                )
        );
    }

    private void Awake()
    {
        if(shard == null)
            shard = Resources.Load<GameObject>("Prefab(I)/Shard");
        if (health == null)
            health = Resources.Load<GameObject>("Prefab(I)/HealthShard");
        if (hurt == null)
            hurt = Resources.Load<Material>("Materials/Hurt");
    }

    protected virtual void DoNothing()
    {
        if(destination.isActiveAndEnabled)
            destination.enabled = false;
        if (iPath.isActiveAndEnabled)
            iPath.enabled = false;
        if (rb.velocity != Vector2.zero && isAttacking == false)
            rb.velocity = Vector2.zero;
        return;
    }

    ///<summary>處理跟隨玩家的機制。預設執行動作：透過A*找路徑跟隨玩家</summary>
    protected virtual void Follow()//可以改寫
    {
        destination.enabled = true;
        iPath.enabled = true;
        iPath.maxSpeed = speed;
    }

    ///<summary>當此怪物被生成時要執行的動作。預設執行動作：無</summary>
    protected virtual void OnSpawn()//要自己寫，如果沒有特殊需求則維持不變
    {
        return;
    }

    ///<summary>當此怪物被殺死時要執行的動作。預設執行動作：放出死亡特效</summary>
    protected virtual void OnDead()//要自己寫，如果沒有特殊需求則維持不變
    {
        StopAllCoroutines();
        this.enabled = false;
        StartCoroutine(playDead());
        return;
    }

    protected virtual IEnumerator playDead()
    {
        sr.material = oriSr;
        rb.velocity = Vector2.zero;
        rb.mass = 10000;
        Destroy(rb);
        foreach(Collider2D c in colliders)
            c.enabled = false;
        destination.enabled = false;
        iPath.enabled = false;
        sr.enabled = false;
        float dieTime = dieParticle.main.duration;
        dieParticle.Play();
        if (parentLevel != null && m_MyEvent != null)
        {
            m_MyEvent.Invoke();
            m_MyEvent.RemoveAllListeners();
        }
        giveShard();
        yield return new WaitForSeconds(dieTime);
        Destroy(gameObject);
    }

    protected void giveShard()
    {
        int seed = Random.Range(0, 100);
        for(int i = 0; i < shardNum; i++)
        {
            if (currentShard < shardNum)
            {
                currentShard++;
                float rand = Random.Range(0, 360) * Mathf.Deg2Rad;
                float speed = Random.Range(5, 10);
                GameObject newShard = Instantiate((i == 0 && seed <= 3)? health : shard, transform.position, Quaternion.identity);
                newShard.GetComponent<Rigidbody2D>().velocity = speed * new Vector2(Mathf.Cos(rand), Mathf.Sin(rand));
            }
            else
                return;
        }
    }

    ///<summary>處理攻擊。預設執行動作：無，必須加入攻擊的函式</summary>
    protected virtual void Attack()//要自己寫攻擊模式
    {
        if(Attacks[pattern[current]] != null)
            Attacks[pattern[current]].Invoke();
        current++;
        if(current == atkPatternLen)
        {
            current = 0;
            PatternShuffle();
        }
        return;
    }

    ///<summary>判斷是否能執行攻擊。預設判斷：如果冷卻時間歸零且玩家與敵人間沒有障礙物則回傳true</summary>
    protected virtual bool CanAttack()//可以改寫
    {
        RaycastHit2D raycast = Physics2D.CircleCast(enemyTransform.position, 1f, playerTransform.position - enemyTransform.position, float.PositiveInfinity, 1 << 8 | 1 << 10);
        bool canATK = (raycast.collider.gameObject.layer == 10 && timer <= 0);
        if (canATK)
            timer = coolDown;
        return canATK;
    }

    ///<summary>判斷是否能執行跟隨。預設判斷：如果此怪物正在攻擊則回傳false</summary>
    protected virtual bool CanFollow()
    {
        return !isAttacking;
    }

    protected virtual void Start()//需要override以獲取繼承此class的script所涵蓋的資料，關於如何override請參考底下的註解
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform.Find("Player");
        player = playerTransform.gameObject.GetComponentInParent<PlayerController>();
        enemyTransform = transform;
        rb = enemyTransform.gameObject.GetComponent<Rigidbody2D>();
        sr = enemyTransform.gameObject.GetComponent<SpriteRenderer>();
        colliders = GetComponents<Collider2D>();
        dieParticle = enemyTransform.Find("Die").gameObject.GetComponent<ParticleSystem>();
        destination = enemyTransform.gameObject.GetComponent<AIDestinationSetter>();
        destination.target = playerTransform;
        iPath = enemyTransform.gameObject.GetComponent<AIPath>();
        timer = coolDown;
        maxHp = hp;
        speed = speed * Random.Range(0.8f, 1.2f);
        rb.angularDrag = 10000;
        oriSr = sr.material;
        parentLevel = this.GetComponentInParent<Level>();
        if (parentLevel != null)
            m_MyEvent.AddListener(parentLevel.EnemyDie);
    }
    /* override範例
     
        public class test : Enemy <--- 這裡要改成繼承Enemy而不是MonoBehaviour
        {
            protected override void Start() <--- 這個start是在其他繼承Enemy這個class的script裡
            {
                base.Start();                       <--- base代表使用原本的code，只是後面會再額外加東西
                Attacks.Add(attack1);               <--- attack1是在test這個script裡的function，有幾種攻擊模式就要add幾次
                atkPatternLen = Attacks.Count;
                current = 0;
                pattern = new int[atkPatternLen];
                PatternShuffle();
            }

            void attack1()
            {
                //這裡寫攻擊模式
            }
            .
            .
            .
            下略
        }
     
    */

    protected void PatternShuffle() //洗牌
    {
        for(int i = 0; i < atkPatternLen; i ++)
            pattern[i] = i;
        if(shuffle)
        {
            for (int i = 0; i < atkPatternLen; i++)
            {
                int rand = UnityEngine.Random.Range(0, atkPatternLen);
                int temp = pattern[i];
                pattern[i] = pattern[rand];
                pattern[rand] = temp;
            }
        }
    }

    protected virtual void Update()
    {
        if(isAttacking == false)
        {
            timer -= Time.deltaTime;
        }
        aiRoot.Tick();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Contact");
            player.StartCoroutine(player.GetDamage());
        }
    }

    public virtual IEnumerator Hurt()
    {
        sr.material = hurt;
        yield return new WaitForSeconds(0.075f);
        sr.material = oriSr;
    }

    public Root GetAIRoot()
    {
        return aiRoot;
    }
}
