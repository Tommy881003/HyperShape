using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Level : MonoBehaviour
{
    private PlayerController player;
    public LevelInfo info;
    private int maxWaveNum = 4;
    private float spawnTime;
    private GameObject warning;
    private Ease ease, fadeEase;
    public GameObject map, enemy, obstacle;
    [HideInInspector]
    public int WaveNum, currentWaveNum = 0;
    [Header("關卡大小")]
    public int X, Y;
    [Header("關卡接口")]
    public Direction[] directions;
    [HideInInspector]
    public Vector2Int PosInArray = new Vector2Int(-1, -1);
    [HideInInspector]
    public int connection = 0;
    public List<Enemy> waveOne = new List<Enemy>(), waveTwo = new List<Enemy>(), waveThree = new List<Enemy>(), waveFour = new List<Enemy>();
    private BoxCollider2D box;
    public int enemyNum = 0;
    public int maxEnemyNum = 0;

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
                break;
            }
        }
        maxEnemyNum = waveOne.Count + waveTwo.Count + waveThree.Count + waveFour.Count;
        shuffle();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            box.enabled = false;
            currentWaveNum++;
            StartCoroutine(SpawnEnemy());
            player.isBattling = true;
        }
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
        if(enemyNum <= 0)
        {
            currentWaveNum++;
            StartCoroutine(SpawnEnemy());
        }
        if(maxEnemyNum <= 0)
            player.isBattling = false;
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
