using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Unlocker : Enemy
{
    private BulletManager manager;
    public BulletPattern ring;
    //public BulletPattern[] tetris;
    public Unlocker_Room room;
    [SerializeField]
    private GameObject body1 = null, body2 = null;
    [SerializeField]
    private SpriteRenderer b11 = null, b12 = null, b21 = null, b22 = null;
    private GameObject currentBody;
    [SerializeField]
    private bool isRage = false;
    protected override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerTransform = player.gameObject.transform.Find("Player");
        enemyTransform = this.transform;
        rb = enemyTransform.gameObject.GetComponent<Rigidbody2D>();
        sr = enemyTransform.gameObject.GetComponent<SpriteRenderer>();
        dieParticle = enemyTransform.Find("Die").gameObject.GetComponent<ParticleSystem>();
        parentLevel = this.GetComponentInParent<Level>();
        room = GetComponentInParent<Unlocker_Room>();
        manager = BulletManager.instance;

        Attacks.Add(attack1);
        Attacks.Add(attack2);
        Attacks.Add(attack3);

        timer = coolDown;
        currentBody = body1;
        atkPatternLen = Attacks.Count;
        current = 0;
        pattern = new int[atkPatternLen];

        PatternShuffle();
        if (parentLevel != null)
            m_MyEvent.AddListener(parentLevel.EnemyDie);
        /*if (parentLevel != null)
            this.gameObject.SetActive(false);*/
    }

    public void attack1()
    {
        StartCoroutine(forAttack1());
    }

    IEnumerator forAttack1()
    {
        isAttacking = true;
        Color oriColor = b11.color;
        Vector3 scale = transform.localScale;
        float distance = (transform.position - room.gameObject.transform.position).magnitude;
        float time = Mathf.Lerp(0.5f, 1f, Mathf.Clamp01(distance / 15f));
        int degree = 90 * Mathf.FloorToInt(time / 0.5f) + 90;
        transform.DOMove(room.gameObject.transform.position, time).SetEase(Ease.OutQuad);
        transform.DORotate(new Vector3(0, 0, degree), time).SetEase(Ease.OutQuad);
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
            for (int i = 0; i <= 16; i++)
            {
                if (i <= 8)
                {
                    for (int j = 0; j < 2 * i + 1; j++)
                    {
                        room.DoPop(7 - i + j, 4 - i, 0.5f, 0.5f);
                        room.DoPop(7 - i, 5 - i + j, 0.5f, 0.5f);
                        room.DoPop(8 + i - j, 5 + i, 0.5f, 0.5f);
                        room.DoPop(8 + i, 4 - i + j, 0.5f, 0.5f);
                    }
                }
                if (i >= 4)
                {
                    for (int j = 0; j < (i - 4) + 1; j++)
                    {
                        room.DoPop(7 - j, 4 - (i - 4 - j), 0.5f, 0.5f);
                        room.DoPop(7 - j, 5 + (i - 4 - j), 0.5f, 0.5f);
                        room.DoPop(8 + j, 5 + (i - 4 - j), 0.5f, 0.5f);
                        room.DoPop(8 + j, 4 - (i - 4 - j), 0.5f, 0.5f);
                    }
                }
                if (i != 16)
                    yield return new WaitForSeconds(0.5f);
            }
        }
        else
        {
            for (int i = 0; i <= 8; i++)
            {
                for (int j = 0; j < 2 * i + 1; j++)
                {
                    room.DoPop(7 - i + j, 4 - i, 0.5f, 0.5f);
                    room.DoPop(7 - i, 5 - i + j, 0.5f, 0.5f);
                    room.DoPop(8 + i - j, 5 + i, 0.5f, 0.5f);
                    room.DoPop(8 + i, 4 - i + j, 0.5f, 0.5f);
                }
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
        Vector3 scale = transform.localScale;
        transform.DORotate(new Vector3(0, 0, -90), 0.75f).SetEase(Ease.InBack);
        transform.DOScale(Vector3.zero, 0.75f).SetEase(Ease.InBack);
        yield return new WaitForSeconds(1.5f);
        if(isRage)
        {
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
                transform.DORotate(new Vector3(0, 0, 0), 0.4f).SetEase(Ease.OutBack);
                transform.DOScale(scale, 0.4f).SetEase(Ease.OutBack);
                if (i != 5)
                {
                    yield return new WaitForSeconds(0.4f);
                    transform.DORotate(new Vector3(0, 0, -90), 0.4f).SetEase(Ease.OutCubic);
                    transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.OutCubic);
                    yield return new WaitForSeconds(0.75f);
                }
            }
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
                transform.DORotate(new Vector3(0, 0, 0), 0.4f).SetEase(Ease.OutBack);
                transform.DOScale(scale, 0.4f).SetEase(Ease.OutBack);
                if (i != 3)
                {
                    yield return new WaitForSeconds(0.4f);
                    transform.DORotate(new Vector3(0, 0, -90), 0.4f).SetEase(Ease.OutCubic);
                    transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.OutCubic);
                    yield return new WaitForSeconds(1f);
                }
            }
        }
        isAttacking = false;
    }

    protected override IEnumerator playDead()
    {
        rb.velocity = Vector2.zero;
        rb.mass = 10000;
        sr.enabled = false;
        float dieTime = dieParticle.main.duration;
        dieParticle.Play();
        if (parentLevel != null && m_MyEvent != null)
        {
            m_MyEvent.Invoke();
            m_MyEvent.RemoveAllListeners();
        }
        yield return new WaitForSeconds(dieTime);
        Destroy(this.gameObject);
    }

    protected override bool CanFollow()
    {
        return true;
    }

    protected override void Follow()
    {
        return;
    }
}
