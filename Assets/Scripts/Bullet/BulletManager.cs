using System.Collections;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletCreator;
using UnityEngine.SceneManagement;
using UnityEngine.Jobs;
using Unity.Jobs;

public class BulletManager : MonoBehaviour
{
    public PlayerController player;
    private GameObject dummy;
    [HideInInspector]
    public int currentBulletAmount;

    public static BulletManager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }

    public Dictionary<string, Queue<GameObject>> bulletDictionary = new Dictionary<string, Queue<GameObject>>();

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("DummyPlayer").GetComponent<PlayerController>();
        BulletArray bulletArray = Resources.Load<BulletArray>("ScriptableObject/BA");
        bulletArray.DictIO();
        bulletDictionary = new Dictionary<string, Queue<GameObject>>();
        dummy = new GameObject();
        foreach (Bullet bullet in bulletArray.enemies)
        {
            Queue<GameObject> bulletPool = new Queue<GameObject>();
            for (int i = 0; i < bullet.size; i++)
            {
                GameObject newBulet = Instantiate(bullet.gameObject);
                newBulet.name = bullet.name;
                newBulet.SetActive(false);
                bulletPool.Enqueue(newBulet);
            }
            bulletDictionary.Add(bullet.name, bulletPool);
        }
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    void OnSceneLoad(Scene s, LoadSceneMode l)
    {
        StopAllCoroutines();
        player = GameObject.Find("DummyPlayer").GetComponent<PlayerController>();
        BulletArray bulletArray = Resources.Load<BulletArray>("ScriptableObject/BA");
        bulletArray.DictIO();
        bulletDictionary = new Dictionary<string, Queue<GameObject>>();
        dummy = new GameObject();
        foreach (Bullet bullet in bulletArray.enemies)
        {
            Queue<GameObject> bulletPool = new Queue<GameObject>();
            for (int i = 0; i < bullet.size; i++)
            {
                GameObject newBulet = Instantiate(bullet.gameObject);
                newBulet.name = bullet.name;
                newBulet.SetActive(false);
                bulletPool.Enqueue(newBulet);
            }
            bulletDictionary.Add(bullet.name, bulletPool);
        }
    }

    struct PatternUpdateJob : IJobParallelForTransform
    {
        [ReadOnly]
        public NativeArray<float> distance;
        [ReadOnly]
        public NativeArray<float> angles; //in radian format
        [ReadOnly]
        public NativeArray<bool> isActive;
        public Vector3 parentPos;
        public float parentAngle;
        public float scale;
        public bool rotateWithParent;

        public void Execute(int i, TransformAccess transform)
        {
            if (!isActive[i])
                return;
            float newAngle = parentAngle + angles[i];
            Vector3 newDisance = new Vector3(distance[i] * Mathf.Cos(newAngle), distance[i] * Mathf.Sin(newAngle));
            transform.position = parentPos + (newDisance * scale);
            if (rotateWithParent)
                transform.rotation = Quaternion.Euler(0, 0, parentAngle * Mathf.Rad2Deg);
        }
    }

    struct PositionUpdateJob : IJobParallelForTransform
    {
        [ReadOnly]
        public NativeArray<float> angles; //in radian format
        [ReadOnly]
        public NativeArray<bool> isActive;
        [ReadOnly]
        public float delta;
        [ReadOnly]
        public NativeArray<float> speeds;

        public void Execute(int i, TransformAccess transform)
        {
            if (!isActive[i])
                return;
            transform.position += speeds[i] * new Vector3(Mathf.Cos(angles[i]), Mathf.Sin(angles[i])) * delta;
        }
    }

    public IEnumerator SpawnPattern(BulletPattern pattern, Vector2 pos, Quaternion quaternion, bool rotateWithParent = false)
    {
        GameObject newDummy = Instantiate(dummy, pos, quaternion);
        Transform parent = newDummy.transform;
        Rigidbody2D rb = newDummy.AddComponent<Rigidbody2D>();
        List<GameObject> bullets = new List<GameObject>();
        List<SpriteRenderer> srs = new List<SpriteRenderer>();
        List<ParticleSystem> pss = new List<ParticleSystem>(); 
        rb.gravityScale = 0;
        float timer = 0;
        float vRotate = quaternion.eulerAngles.z * Mathf.Deg2Rad;
        float selfRotate = newDummy.transform.rotation.eulerAngles.z;
        float speed = 0;
        int j = 0, length = pattern.spawns.Count;
        Vector2[] previous = new Vector2[length];
        currentBulletAmount += length;
        Transform[] temp = new Transform[length];
        JobHandle PositionJobHandle;
        NativeArray<float> distance = new NativeArray<float>(length, Allocator.Persistent), angles = new NativeArray<float>(length, Allocator.Persistent);
        NativeArray<bool> isActive = new NativeArray<bool>(length, Allocator.Persistent);
        foreach (SpawnData spawn in pattern.spawns)
        {
            float newRotate = vRotate + Vector2.SignedAngle(Vector2.right,spawn.position) * Mathf.Deg2Rad;
            GameObject newBullet = bulletDictionary[spawn.name].Dequeue();
            newBullet.transform.position = new Vector2(spawn.position.magnitude * Mathf.Cos(newRotate), spawn.position.magnitude * Mathf.Sin(newRotate)) + pos;
            newBullet.transform.rotation = ((rotateWithParent == true) ? quaternion : Quaternion.Euler(0,0,0));
            newBullet.SetActive(true);
            bullets.Add(newBullet);
            bulletDictionary[spawn.name].Enqueue(newBullet);
            srs.Add(newBullet.GetComponent<SpriteRenderer>());
            pss.Add(newBullet.GetComponent<ParticleSystem>());
            previous[j] = newBullet.transform.position;
            distance[j] = spawn.position.magnitude;
            angles[j] = newRotate - vRotate;
            temp[j] = newBullet.transform;
            isActive[j] = true;
            j++;
        }
        TransformAccessArray transforms = new TransformAccessArray(temp);
        float tempRotate = vRotate;
        int templength = length;
        pattern.vRotation.postWrapMode = WrapMode.Loop;
        pattern.velocity.postWrapMode = WrapMode.Loop;
        pattern.rotation.postWrapMode = WrapMode.Loop;
        pattern.scale.postWrapMode = WrapMode.Loop;
        while (timer <= pattern.timer)
        {
            if (length == 0)
                break; 
            vRotate = tempRotate + pattern.vRotation.Evaluate(timer) * Mathf.Deg2Rad;
            speed = pattern.velocity.Evaluate(timer);
            rb.velocity = new Vector2(speed * Mathf.Cos(vRotate), speed * Mathf.Sin(vRotate));
            selfRotate += pattern.rotation.Evaluate(timer) * Time.deltaTime;
            newDummy.transform.rotation = Quaternion.Euler(new Vector3(0, 0, selfRotate));
            PatternUpdateJob m_Job = new PatternUpdateJob()
            {
                isActive = isActive,
                distance = distance,
                angles = angles,
                scale = pattern.scale.Evaluate(timer),
                parentPos = parent.position,
                parentAngle = parent.rotation.eulerAngles.z * Mathf.Deg2Rad,
                rotateWithParent = rotateWithParent,
            };
            PositionJobHandle = m_Job.Schedule(transforms);
            PositionJobHandle.Complete();
            timer += Time.deltaTime;
            yield return null;
            for (int i = 0; i < bullets.Count; i++)
            {
                if (isActive[i] && bullets[i].activeSelf)
                {
                    isActive[i] = circleCast(bullets[i],bullets[i].transform.position,previous[i],0.3f,srs[i],pss[i]);
                    length = (isActive[i] == false ? length - 1 : length);
                }
                previous[i] = bullets[i].transform.position;
            }
        }
        if(length > 0)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i].activeSelf)
                {
                    StartCoroutine(PlayParticle(bullets[i], srs[i], pss[i]));
                    currentBulletAmount = Mathf.Max(currentBulletAmount - 1, 0);
                }
            }
        }
        Destroy(newDummy);
        isActive.Dispose();
        transforms.Dispose();
        distance.Dispose();
        angles.Dispose();
    }

    public IEnumerator SpawnOneShot(string bulletName, int count, float angle, float speed, bool spreadMode, Vector2 pos, float quaternion)
    {
        if(bulletDictionary.ContainsKey(bulletName) == false)
        {
            Debug.LogWarning("Bullet named " + bulletName + " is not in the dictionary");
            yield break;
        }
        List<GameObject> bullets = new List<GameObject>();
        List<SpriteRenderer> srs = new List<SpriteRenderer>();
        List<ParticleSystem> pss = new List<ParticleSystem>();
        currentBulletAmount += count;
        Transform[] temp = new Transform[count];
        JobHandle PositionJobHandle;
        Vector2[] previous = new Vector2[count];
        NativeArray<float> angles = new NativeArray<float>(count, Allocator.Persistent);
        NativeArray<float> speeds = new NativeArray<float>(count, Allocator.Persistent);
        NativeArray<bool> isActive = new NativeArray<bool>(count, Allocator.Persistent);
        for(int i = 0; i < count; i++)
        {
            GameObject newBullet = bulletDictionary[bulletName].Dequeue();
            newBullet.transform.position = pos;
            newBullet.transform.rotation = Quaternion.Euler(0, 0, 0);
            newBullet.SetActive(true);
            bullets.Add(newBullet);
            bulletDictionary[bulletName].Enqueue(newBullet);
            srs.Add(newBullet.GetComponent<SpriteRenderer>());
            pss.Add(newBullet.GetComponent<ParticleSystem>());
            angles[i] = ((spreadMode == true? UnityEngine.Random.Range(-angle / 2, angle / 2) : Mathf.Lerp(-angle/2,angle/2, (float)i/(float)(count - 1))) + quaternion) * Mathf.Deg2Rad;
            speeds[i] = speed + (spreadMode == true ? UnityEngine.Random.Range(-0.1f*speed, 0.1f*speed) : 0);
            temp[i] = newBullet.transform;
            previous[i] = temp[i].position;
            isActive[i] = true;
        }
        TransformAccessArray transforms = new TransformAccessArray(temp);
        int tempCount = count;
        float timer = 0;
        while (tempCount > 0 && timer <= 10)
        {
            PositionUpdateJob m_Job = new PositionUpdateJob()
            {
                isActive = isActive,
                angles = angles,
                speeds = speeds,
                delta = Time.deltaTime
            };
            PositionJobHandle = m_Job.Schedule(transforms);
            PositionJobHandle.Complete();
            yield return null;
            for (int i = 0; i < bullets.Count; i++)
            {
                if (isActive[i] && bullets[i].activeSelf)
                {
                    isActive[i] = circleCast(bullets[i], bullets[i].transform.position, previous[i], 0.3f, srs[i], pss[i]);
                    tempCount = (isActive[i] == false ? tempCount - 1 : tempCount);
                }
                previous[i] = bullets[i].transform.position;
            }
            timer += Time.deltaTime;
        }
        if (tempCount > 0)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i].activeSelf)
                    bullets[i].SetActive(false);
            }
        }
        isActive.Dispose();
        transforms.Dispose();
        speeds.Dispose();
        angles.Dispose();
    }

    bool circleCast(GameObject go ,Vector2 pos, Vector2 previous, float radius, SpriteRenderer sr, ParticleSystem ps)
    {
        RaycastHit2D hit = Physics2D.CircleCast(pos, radius, previous - pos, (previous - pos).magnitude, 1 << 8 | 1 << 10 | 1 << 15);
        if (hit.collider == null)
            return true;
        else
        {
            if (player.vulnerable == true && hit.collider.gameObject.CompareTag("Player"))
                StartCoroutine(player.GetDamage());
                
            if (hit.collider.gameObject.CompareTag("Wall") || hit.collider.gameObject.CompareTag("GhostWall"))
            {
                StartCoroutine(PlayParticle(go, sr, ps));
                currentBulletAmount = Mathf.Max(currentBulletAmount - 1, 0);
                return false;
            }
            else
                return true;
        }
    }

    public IEnumerator PlayParticle(GameObject go,SpriteRenderer sr, ParticleSystem ps)
    {
        foreach (Transform child in go.transform)
            child.gameObject.SetActive(false);
        if(sr.isVisible == false)
        {
            foreach (Transform child in go.transform)
                child.gameObject.SetActive(true);
            go.SetActive(false);
            yield break;
        }
        sr.enabled = false;
        ps.Play();
        float time = ps.main.duration;
        yield return new WaitForSeconds(time);
        sr.enabled = true;
        foreach (Transform child in go.transform)
            child.gameObject.SetActive(true);
        go.SetActive(false);
    }
}
