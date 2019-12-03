using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Level : MonoBehaviour
{
    private PlayerController player;
    public LevelInfo info;
    private LineRenderer lr;
    private int maxWaveNum = 4;
    private float spawnTime;
    private GameObject warning;
    private Ease ease, fadeEase;
    public GameObject map, enemy, obstacle;
    public bool isBoss;
    public Boss boss;
    [HideInInspector]
    public int WaveNum, currentWaveNum = 0;
    [Header("關卡大小")]
    public int X, Y;
    [HideInInspector]
    public Vector2Int PosInArray = new Vector2Int(-1, -1);
    [HideInInspector]
    public int connection = 0;
    public List<Enemy> waveOne = new List<Enemy>(), waveTwo = new List<Enemy>(), waveThree = new List<Enemy>(), waveFour = new List<Enemy>();
    private BoxCollider2D box;
    public int enemyNum = 0;
    public int maxEnemyNum = 0;
    private static Gradient gradient = null, battleGrad = null;
    private Gradient now;

    private void Awake()
    {
        if(battleGrad == null)
        {
            battleGrad = new Gradient();
            battleGrad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 1.0f) }
            );
        }
        else
            return;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        maxWaveNum = info.maxWaveNum;
        spawnTime = info.spawnTime;
        warning = info.warning;
        ease = info.ease;
        fadeEase = info.fadeEase;
        BoxCollider2D[] boxes = this.GetComponents<BoxCollider2D>();
        foreach(BoxCollider2D temp in boxes)
        {
            if(temp.isTrigger)
            {
                box = temp;
                box.size = new Vector2(4*X - 2,4*Y - 2);
                break;
            }
        }
        maxEnemyNum = waveOne.Count + waveTwo.Count + waveThree.Count + waveFour.Count;
        lr = GetComponent<LineRenderer>();
        if (gradient == null)
            gradient = lr.colorGradient;
        now = gradient;
        Invoke("DrawLine", 0.2f);
        shuffle();
    }

    private void Update()
    {
        if(lr.colorGradient != now)
            lr.colorGradient = now;
    }

    void DrawLine()
    {
        Transform map = transform.Find("Map");
        Vector3[] lines = new Vector3[4];
        lines[0] = new Vector3(map.position.x + map.localScale.x / 2, map.position.y + map.localScale.y / 2);
        lines[1] = new Vector3(map.position.x - map.localScale.x / 2, map.position.y + map.localScale.y / 2);
        lines[2] = new Vector3(map.position.x - map.localScale.x / 2, map.position.y - map.localScale.y / 2);
        lines[3] = new Vector3(map.position.x + map.localScale.x / 2, map.position.y - map.localScale.y / 2);
        lr.positionCount = 4;
        lr.SetPositions(lines);
        lr.loop = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(box.enabled == true && collision.gameObject.CompareTag("Player"))
        {
            box.enabled = false;
            
            if(isBoss)
            {
                Camera cam = Camera.main;
                CameraFollower follower = cam.GetComponent<CameraFollower>();
                StartCoroutine(StartBossCutScene(cam, follower));
                player.isBattling = true;
                now = battleGrad;
            }
            else if(maxEnemyNum > 0)
            {
                currentWaveNum++;
                StartCoroutine(SpawnEnemy());
                player.isBattling = true;
                now = battleGrad;
            }
        }
    }

    IEnumerator StartBossCutScene(Camera cam, CameraFollower follower)
    {
        follower.isCutScene = true;
        Vector3 oriPos = cam.transform.position;
        cam.transform.DOMove(new Vector3(boss.transform.position.x, boss.transform.position.y, cam.transform.position.z), 1);
        yield return new WaitForSeconds(1.5f);
        boss.gameObject.SetActive(true);
        boss.StartCoroutine(boss.StartCutScene());
        BossHealthBar.instance.StartCoroutine(BossHealthBar.instance.StartBoss(boss));
        yield return new WaitForSeconds(3);
        cam.transform.DOMove(oriPos, 1);
        yield return new WaitForSeconds(1);
        follower.isCutScene = false;
    }

    IEnumerator SpawnEnemy()
    {
        switch(currentWaveNum)
        {
            case 1:
                enemyNum = waveOne.Count;
                foreach(Enemy enemy in waveOne)
                {
                    StartCoroutine(SpawnWarning(enemy.gameObject));
                    yield return new WaitForSeconds(spawnTime);
                }
                break;
            case 2:
                enemyNum = waveTwo.Count;
                foreach (Enemy enemy in waveTwo)
                {
                    StartCoroutine(SpawnWarning(enemy.gameObject));
                    yield return new WaitForSeconds(spawnTime);
                }
                break;
            case 3:
                enemyNum = waveThree.Count;
                foreach (Enemy enemy in waveThree)
                {
                    StartCoroutine(SpawnWarning(enemy.gameObject));
                    yield return new WaitForSeconds(spawnTime);
                }
                break;
            case 4:
                enemyNum = waveFour.Count;
                foreach (Enemy enemy in waveFour)
                {
                    StartCoroutine(SpawnWarning(enemy.gameObject));
                    yield return new WaitForSeconds(spawnTime);
                }
                break;
            default:
                Debug.LogWarning("Something in the " + this.gameObject.name + " went wrong!");
                break;
        }
        yield break;
    }

    IEnumerator SpawnWarning(GameObject go)
    {
        GameObject newWarning = Instantiate(warning, go.transform.position, Quaternion.identity);
        SpriteRenderer sr = newWarning.GetComponent<SpriteRenderer>();
        ParticleSystem ps = newWarning.GetComponent<ParticleSystem>();
        Vector3 scale = newWarning.transform.localScale;
        sr.DOFade(1, 2f).SetEase(Ease.OutExpo);
        sr.DOColor(Color.white, 2f).SetEase(Ease.InCubic);
        newWarning.transform.DOScale(1.75f * scale, 2f).SetEase(Ease.InCubic);
        newWarning.transform.DOShakePosition(2f, 0.3f, 30, 90, false, false).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(2f);
        go.SetActive(true);
        sr.enabled = false;
        ps.Play();
        float time = ps.main.duration;
        yield return new WaitForSeconds(time);
        Destroy(newWarning);
    }

    public void EnemyDie()
    {
        enemyNum--;
        maxEnemyNum--;
        if(enemyNum <= 0 && maxEnemyNum > 0)
        {
            currentWaveNum++;
            StartCoroutine(SpawnEnemy());
        }
        if(maxEnemyNum <= 0)
        {
            player.isBattling = false;
            now = gradient;
        }
    }

    public void BossDie()
    {
        transform.Find("KillBullet").gameObject.GetComponent<BoxCollider2D>().enabled = true;
        player.isBattling = false;
        now = gradient;
        Camera cam = Camera.main;
        CameraFollower follower = cam.GetComponent<CameraFollower>();
        StartCoroutine(EndBossCutScene(cam, follower));
    }

    IEnumerator EndBossCutScene(Camera cam, CameraFollower follower)
    {
        follower.isCutScene = true;
        Vector3 oriPos = cam.transform.position;
        cam.transform.DOMove(new Vector3(boss.transform.position.x, boss.transform.position.y, cam.transform.position.z), 0.5f);
        BossHealthBar.instance.StartCoroutine(BossHealthBar.instance.EndBoss());
        yield return new WaitForSeconds(3);
        cam.transform.DOMove(oriPos, 1);
        yield return new WaitForSeconds(1);
        follower.isCutScene = false;
    }

    void shuffle()
    {
        for(int i = 0; i < waveOne.Count; i++)
        {
            int rand = UnityEngine.Random.Range(0, waveOne.Count);
            Enemy temp = waveOne[i];
            waveOne[i] = waveOne[rand];
            waveOne[rand] = temp;
        }
        for (int i = 0; i < waveTwo.Count; i++)
        {
            int rand = UnityEngine.Random.Range(0, waveTwo.Count);
            Enemy temp = waveTwo[i];
            waveTwo[i] = waveTwo[rand];
            waveTwo[rand] = temp;
        }
        for (int i = 0; i < waveThree.Count; i++)
        {
            int rand = UnityEngine.Random.Range(0, waveThree.Count);
            Enemy temp = waveThree[i];
            waveThree[i] = waveThree[rand];
            waveThree[rand] = temp;
        }
        for (int i = 0; i < waveFour.Count; i++)
        {
            int rand = UnityEngine.Random.Range(0, waveFour.Count);
            Enemy temp = waveFour[i];
            waveFour[i] = waveFour[rand];
            waveFour[rand] = temp;
        }
    }
}
