using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Unlocker : Boss
{
    private BoxCollider2D box;
    private CameraFollower follower;
    private BulletManager manager;
    private BossHealthBar healthBar;
    private ParticleSystem explode; 
    public ParticleSystem rageParticle;
    public BulletPattern ring,dot4,helix4;
    //public BulletPattern[] tetris;
    public Unlocker_Room room;
    [SerializeField]
    private GameObject body1 = null, body2 = null;
    [SerializeField]
    private SpriteRenderer b11 = null, b12 = null, b21 = null, b22 = null;
    private GameObject currentBody;
    private bool canFollow = false,isRage = false;
    private float halfHp;
    protected override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerTransform = player.gameObject.transform.Find("Player");
        enemyTransform = this.transform;
        rb = enemyTransform.gameObject.GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        sr = enemyTransform.gameObject.GetComponent<SpriteRenderer>();
        dieParticle = enemyTransform.Find("Die").gameObject.GetComponent<ParticleSystem>();
        explode = enemyTransform.Find("Explode").gameObject.GetComponent<ParticleSystem>();
        parentLevel = this.GetComponentInParent<Level>();
        room = GetComponentInParent<Unlocker_Room>();
        manager = BulletManager.instance;
        follower = Camera.main.GetComponent<CameraFollower>();
        healthBar = BossHealthBar.instance;
        healthBar.StartBoss(this);

        Attacks.Add(attack1);
        Attacks.Add(attack2);
        Attacks.Add(attack3);
        Attacks.Add(attack4);
        Attacks.Add(attack5);

        timer = coolDown;
        currentBody = body1;
        atkPatternLen = Attacks.Count;
        current = 0;
        pattern = new int[atkPatternLen];
        maxHp = hp;
        halfHp = hp/2;

        PatternShuffle();
        if (parentLevel != null)
            m_MyEvent.AddListener(parentLevel.BossDie);
        /*if (parentLevel != null)
            this.gameObject.SetActive(false);*/
    }

    protected override void Update()
    {
        if(follower.isCutScene == false)
            base.Update();
        if(isRage == false && hp <= halfHp)
        {
            isRage = true;
            StopAllCoroutines();
            StartCoroutine(Phase2());
        }
    }

    IEnumerator Phase2()
    {
        enabled = false;
        canFollow = false;
        rb.velocity = Vector2.zero;
        Vector3 scale = transform.localScale;
        transform.DOKill();
        b11.DOKill();
        b12.DOKill();
        Color oriColor = b11.color;
        yield return new WaitForSeconds(0.5f);
        b11.DOColor(Color.white, 1.5f);
        b12.DOColor(Color.white, 1.5f);
        transform.DOShakePosition(1.5f, 1f, 30);
        transform.DORotate(new Vector3(0, 0, 360), 1.5f, RotateMode.FastBeyond360).SetEase(Ease.OutCubic);
        transform.DOScale(1.4f * scale, 1.5f).SetEase(Ease.OutCubic);
        yield return new WaitForSeconds(1.5f);
        transform.DOScale(scale, 0.25f).SetEase(Ease.OutBack);
        transform.DOShakePosition(0.5f, 1f, 15);
        yield return new WaitForSeconds(0.1f);
        follower.StartCoroutine(follower.CamShake(3.5f, 0.5f));
        body2.SetActive(true);
        currentBody = body2;
        body1.SetActive(false);
        b21.DOColor(oriColor, 0.5f);
        b22.DOColor(oriColor, 0.5f);
        yield return new WaitForSeconds(0.5f);
        enabled = true;
        isAttacking = false;
        canFollow = true;
        speed = 1.5f * speed;
        timer = 0;
    }

    public void attack1()
    {
        StartCoroutine(forAttack1());
    }

    IEnumerator forAttack1()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;
        canFollow = false;
        Color oriColor = b21.color;
        Vector3 scale = transform.localScale;
        float distance = (transform.position - room.gameObject.transform.position).magnitude;
        float time = Mathf.Lerp(0.5f, 1f, Mathf.Clamp01(distance / 15f));
        int degree = 90 * Mathf.FloorToInt(time / 0.5f) + 90;
        transform.DOMove(room.gameObject.transform.position, time).SetEase(Ease.OutQuad);
        transform.DORotate(new Vector3(0, 0, degree), time, RotateMode.FastBeyond360).SetEase(Ease.OutQuad);
        transform.DOScale(1.4f * scale, time).SetEase(Ease.OutQuad);
        if(isRage)
        {
            b21.DOColor(Color.white,time).SetEase(Ease.OutQuad);
            b22.DOColor(Color.white, time).SetEase(Ease.OutQuad);
        }
        else
        {
            b11.DOColor(Color.white, time).SetEase(Ease.OutQuad);
            b12.DOColor(Color.white, time).SetEase(Ease.OutQuad);
        }
        yield return new WaitForSeconds(time);
        transform.DOScale(scale, 0.3f).SetEase(Ease.OutBack);
        transform.localEulerAngles = new Vector3(0, 0, 0);
        if (isRage)
        {
            b21.DOColor(oriColor, 0.3f).SetEase(Ease.OutBack);
            b22.DOColor(oriColor, 0.3f).SetEase(Ease.OutBack);
        }
        else
        {
            b11.DOColor(oriColor, 0.3f).SetEase(Ease.OutBack);
            b12.DOColor(oriColor, 0.3f).SetEase(Ease.OutBack);
        }
        yield return new WaitForSeconds(0.15f);
        transform.DOShakePosition(0.5f, 0.5f, 20);
        if (isRage)
        {
            transform.position = room.gameObject.transform.position;
            for (int i = 0; i <= 16; i++)
            {
                if (i <= 8)
                {
                    for (int j = 0; j < 2 * i + 1; j++)
                    {
                        room.DoPop(7 - i + j, 7 - i, 0.5f, 0.5f);
                        room.DoPop(7 - i, 8 - i + j, 0.5f, 0.5f);
                        room.DoPop(8 + i - j, 8 + i, 0.5f, 0.5f);
                        room.DoPop(8 + i, 7 - i + j, 0.5f, 0.5f);
                    }
                }
                if (i >= 4)
                {
                    for (int j = 0; j < (i - 4) + 1; j++)
                    {
                        room.DoPop(7 - j, 7 - (i - 4 - j), 0.5f, 0.5f);
                        room.DoPop(7 - j, 8 + (i - 4 - j), 0.5f, 0.5f);
                        room.DoPop(8 + j, 8 + (i - 4 - j), 0.5f, 0.5f);
                        room.DoPop(8 + j, 7 - (i - 4 - j), 0.5f, 0.5f);
                    }
                }
                transform.DOShakePosition(0.5f, new Vector3(0, 0.5f, 0), 20);
                b21.color = Color.white;
                b22.color = Color.white;
                b21.DOColor(oriColor, 0.5f);
                b22.DOColor(oriColor, 0.5f);
                if (i != 16)
                    yield return new WaitForSeconds(0.5f);
            }
        }
        else
        {
            transform.position = room.gameObject.transform.position;
            for (int i = 0; i <= 8; i++)
            {
                for (int j = 0; j < 2 * i + 1; j++)
                {
                    room.DoPop(7 - i + j, 7 - i, 0.5f, 0.5f);
                    room.DoPop(7 - i, 8 - i + j, 0.5f, 0.5f);
                    room.DoPop(8 + i - j, 8 + i, 0.5f, 0.5f);
                    room.DoPop(8 + i, 7 - i + j, 0.5f, 0.5f);
                }
                transform.DOShakePosition(0.5f, new Vector3(0, 0.5f, 0), 20);
                b11.color = Color.white;
                b12.color = Color.white;
                b11.DOColor(oriColor, 0.5f);
                b12.DOColor(oriColor, 0.5f);
                if (i != 8)
                    yield return new WaitForSeconds(0.5f);
            }
        }
        isAttacking = false;
    }

    public void attack2()
    {
        StartCoroutine(forAttack2());
    }

    IEnumerator forAttack2()
    {
        isAttacking = true;
        canFollow = true;
        for (int i = 0; i <= 80; i++)
        {
            if(isRage)
            {
                manager.StartCoroutine(manager.SpawnOneShot("Boss_Square", 3, 360, 17.5f, true, transform.position, 0));
                if (i % 8 == 0)
                    room.RandomPop(8, 0.8f, 0.8f);
            }
            else
                manager.StartCoroutine(manager.SpawnOneShot("Boss_Square", 3, 360, 15f, true, transform.position, 0));
            yield return new WaitForSeconds(0.1f);
        }   
        isAttacking = false;
    }

    public void attack3()
    {
        StartCoroutine(forAttack3());
    }

    IEnumerator forAttack3()
    {
        isAttacking = true;
        canFollow = false;
        box.enabled = false;
        Vector3 scale = transform.localScale;
        transform.DORotate(new Vector3(0, 0, -90), 0.75f).SetEase(Ease.InBack);
        transform.DOScale(Vector3.zero, 0.75f).SetEase(Ease.InBack);
        Vector2Int pos = room.vec2ind(transform.position);
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i <= 15; i++)
        {
            for (int j = 0; j < 2 * i + 1; j++)
            {
                room.DoFade(pos.x - i + j, pos.y - i, 0.2f);
                room.DoFade(pos.x - i, pos.y - i + j, 0.2f);
                room.DoFade(pos.x + i - j, pos.y + i, 0.2f);
                room.DoFade(pos.x + i, pos.y - i + j, 0.2f);
            }
            yield return new WaitForSeconds(0.1f);
        }
        if (isRage)
        {
            rageParticle.Stop();
            for (int i = 0; i < 6; i++)
            {
                Vector2Int tileVect = room.vec2ind(playerTransform.position);
                tileVect = new Vector2Int((int)Mathf.Clamp(tileVect.x, 2, parentLevel.X - 3), (int)Mathf.Clamp(tileVect.y, 2, parentLevel.Y - 3));
                Vector3 toPosition = room.ind2vec(tileVect.x, tileVect.y);
                room.DoPop(tileVect.x - 1, tileVect.y - 1, 0.75f, 0.5f);
                room.DoPop(tileVect.x - 1, tileVect.y + 1, 0.75f, 0.5f);
                room.DoPop(tileVect.x, tileVect.y, 0.75f, 0.5f);
                room.DoPop(tileVect.x + 1, tileVect.y - 1, 0.75f, 0.5f);
                room.DoPop(tileVect.x + 1, tileVect.y + 1, 0.75f, 0.5f);
                for(int j = 0; j < parentLevel.X; j++)
                    if(j != tileVect.x)
                        room.DoPop(j, tileVect.y, 0.75f, 0.5f);
                for (int j = 0; j < parentLevel.Y; j++)
                    if (j != tileVect.y)
                        room.DoPop(tileVect.x, j, 0.75f, 0.5f);
                yield return new WaitForSeconds(0.75f);
                transform.position = toPosition;
                manager.StartCoroutine(manager.SpawnPattern(ring, toPosition, Quaternion.identity));
                if (i != 5)
                {
                    transform.DORotate(new Vector3(0, 0, 0), 0.4f).SetEase(Ease.OutBack);
                    transform.DOScale(scale, 0.4f).SetEase(Ease.OutBack);
                    yield return new WaitForSeconds(0.4f);
                    transform.DORotate(new Vector3(0, 0, -90), 0.4f).SetEase(Ease.OutCubic);
                    transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.OutCubic);
                    yield return new WaitForSeconds(0.75f);
                }
                else
                {
                    transform.DORotate(new Vector3(0, 0, 0), 1f).SetEase(Ease.OutBack);
                    transform.DOScale(scale, 1f).SetEase(Ease.OutBack);
                    yield return new WaitForSeconds(1f);
                }
            }
            rageParticle.Play();
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2Int tileVect = room.vec2ind(playerTransform.position);
                tileVect = new Vector2Int((int)Mathf.Clamp(tileVect.x, 2, parentLevel.X - 3), (int)Mathf.Clamp(tileVect.y, 2, parentLevel.Y - 3));
                Vector3 toPosition = room.ind2vec(tileVect.x, tileVect.y);
                room.DoPop(tileVect.x - 1, tileVect.y - 1, 0.75f, 0.5f);
                room.DoPop(tileVect.x - 1, tileVect.y, 0.75f, 0.5f);
                room.DoPop(tileVect.x - 1, tileVect.y + 1, 0.75f, 0.5f);
                room.DoPop(tileVect.x, tileVect.y - 1, 0.75f, 0.5f);
                room.DoPop(tileVect.x, tileVect.y, 0.75f, 0.5f);
                room.DoPop(tileVect.x, tileVect.y + 1, 0.75f, 0.5f);
                room.DoPop(tileVect.x + 1, tileVect.y - 1, 0.75f, 0.5f);
                room.DoPop(tileVect.x + 1, tileVect.y, 0.75f, 0.5f);
                room.DoPop(tileVect.x + 1, tileVect.y + 1, 0.75f, 0.5f);
                yield return new WaitForSeconds(0.75f);
                transform.position = toPosition;
                manager.StartCoroutine(manager.SpawnPattern(ring, toPosition, Quaternion.identity));
                if (i != 3)
                {
                    transform.DORotate(new Vector3(0, 0, 0), 0.4f).SetEase(Ease.OutBack);
                    transform.DOScale(scale, 0.4f).SetEase(Ease.OutBack);
                    yield return new WaitForSeconds(0.4f);
                    transform.DORotate(new Vector3(0, 0, -90), 0.4f).SetEase(Ease.OutCubic);
                    transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.OutCubic);
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    transform.DORotate(new Vector3(0, 0, 0), 1f).SetEase(Ease.OutBack);
                    transform.DOScale(scale, 1f).SetEase(Ease.OutBack);
                    yield return new WaitForSeconds(1f);
                }
            }
        }
        box.enabled = true;
        canFollow = true;
        isAttacking = false;
    }

    public void attack4()
    {
        StartCoroutine(forAttack4());
    }

    IEnumerator forAttack4()
    {
        isAttacking = true;
        canFollow = false;
        float timer = 0;
        int count = 0;
        transform.DORotate(new Vector3(0, 0, 2880), 8, RotateMode.FastBeyond360).SetEase(Ease.Linear);
        while(timer <= 8)
        {
            if(timer >= count * 0.1f)
            {
                manager.StartCoroutine(manager.SpawnPattern(dot4, transform.position, Quaternion.Euler(0,0,37*count),true));
                count++;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        isAttacking = false;
    }

    public void attack5()
    {
        StartCoroutine(forAttack5());
    }

    IEnumerator forAttack5()
    {
        isAttacking = true;
        canFollow = true;
        transform.DORotate(new Vector3(0, 0, 360), 8, RotateMode.FastBeyond360).SetEase(Ease.Linear);
        float timer = 0;
        int count = 0;
        int rageCount = 0;
        int[] dir = { 0, 1, 2, 3, 0, 1, 2, 3 };
        for(int i = 0; i < 8; i++)
        {
            int rand = UnityEngine.Random.Range(0, 8);
            int temp = dir[i];
            dir[i] = dir[rand];
            dir[rand] = temp; 
        }
        while(timer <= 8)
        {
            if (timer >= count * 0.16f)
            {
                float angle = Vector2.SignedAngle(Vector2.right, playerTransform.transform.position - transform.position) + UnityEngine.Random.Range(-25, 25);
                manager.StartCoroutine(manager.SpawnPattern(helix4, transform.position, Quaternion.Euler(0, 0, angle)));
                count++;
            }
            if(isRage && timer >= 1.333f * rageCount)
            {
                StartCoroutine(zigZag(dir[rageCount]));
                rageCount++;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, 0);
        isAttacking = false;
    }

    IEnumerator zigZag(int rand)
    {
        int dis = UnityEngine.Random.Range(3, 8);
        int crossDis = UnityEngine.Random.Range(8, 13);
        Vector2Int[] pos = new Vector2Int[4];
        Vector2Int index = room.vec2ind(playerTransform.position);
        pos[0] = new Vector2Int(Mathf.Clamp(index.x - dis, 0, parentLevel.X), Mathf.Clamp(index.y, 1, parentLevel.Y - 1));
        pos[1] = new Vector2Int(Mathf.Clamp(index.x + dis, 0, parentLevel.X), Mathf.Clamp(index.y, 1, parentLevel.Y - 1));
        pos[2] = new Vector2Int(Mathf.Clamp(index.x, 1, parentLevel.X - 1), Mathf.Clamp(index.y - dis, 0, parentLevel.Y));
        pos[3] = new Vector2Int(Mathf.Clamp(index.x, 1, parentLevel.X - 1), Mathf.Clamp(index.y + dis, 0, parentLevel.Y));
        for(int i = 0; i < crossDis; i++)
        {
            switch(rand)
            {
                case 0:
                    room.DoPop(pos[rand].x + i, pos[rand].y + (i % 2), 0.5f, 0.5f);
                    break;
                case 1:
                    room.DoPop(pos[rand].x - i, pos[rand].y + (i % 2), 0.5f, 0.5f);
                    break;
                case 2:
                    room.DoPop(pos[rand].x + (i % 2), pos[rand].y + i, 0.5f, 0.5f);
                    break;
                default:
                    room.DoPop(pos[rand].x + (i % 2), pos[rand].y - i, 0.5f, 0.5f);
                    break;
            }
            yield return new WaitForSeconds(0.125f);
        }
    }

    protected override IEnumerator playDead()
    {
        transform.DOKill();
        b21.DOKill();
        b22.DOKill();
        rb.velocity = Vector2.zero;
        rb.mass = 10000;
        StartCoroutine(EndCutScene());
        float dieTime = dieParticle.main.duration;
        dieParticle.Play();
        if (parentLevel != null && m_MyEvent != null)
        {
            m_MyEvent.Invoke();
            m_MyEvent.RemoveAllListeners();
        }
        yield return new WaitForSeconds(dieTime);
        dieTime = explode.main.duration;
        currentBody.SetActive(false);
        explode.Play();
        yield return new WaitForSeconds(dieTime);
        Destroy(this.gameObject);
    }

    protected override bool CanFollow()
    {
        if (rb.velocity != Vector2.zero && canFollow == false)
            rb.velocity = 0.95f * rb.velocity;
        return canFollow;
    }

    protected override void Follow()
    {
        rb.velocity = (playerTransform.position - enemyTransform.position).normalized * speed;
    }

    protected override void DoNothing()
    {
        return;
    }

    public override IEnumerator StartCutScene()
    {
        Vector3 scale = transform.localScale;
        transform.localScale = Vector3.zero;
        room = GetComponentInParent<Unlocker_Room>();
        for (int i = 8; i >= 0; i--)
        {
            for (int j = 0; j < 2 * i + 1; j++)
            {
                room.DoFade(7 - i + j, 7 - i, 0.25f);
                room.DoFade(7 - i, 8 - i + j, 0.25f);
                room.DoFade(8 + i - j, 8 + i, 0.25f);
                room.DoFade(8 + i, 7 - i + j, 0.25f);
            }
            yield return new WaitForSeconds(0.15f);
        }
        transform.rotation = Quaternion.Euler(0, 0, -180);
        transform.DORotate(new Vector3(0, 0, 0), 1f).SetEase(Ease.OutBack);
        transform.DOScale(scale, 1f).SetEase(Ease.OutBack);
    }

    public override IEnumerator EndCutScene()
    {
        b21.color = Color.white;
        b22.color = Color.white;
        transform.DOShakePosition(2.5f, 1, 50);
        yield break;
    }
}
