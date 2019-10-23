using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    private LevelGenerator generator = null;
    private Camera cam;
    public GameObject player = null;
    private PlayerController controller = null;
    private bool isShakeing;
    [Header("攝影機跟隨強度"),Range(0f,1f)]
    public float lerp = 0.2f;
    private float seed1,seed2,timer;

    // Start is called before the first frame update
    void Start()
    {
        cam = this.GetComponent<Camera>();
        isShakeing = false;
        seed1 = 0; seed2 = 0; timer = 0;
        //generator = GameObject.FindGameObjectWithTag("LevelGenerator").GetComponent<LevelGenerator>();
        if (generator == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform.Find("Player").gameObject;
            controller = player.GetComponentInParent<PlayerController>();
        }
        else
            StartCoroutine(FindPlayer());
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player != null && controller != null)
        {
            Vector2 previousPos = cam.transform.position;
            Vector2 newPos = Vector2.Lerp(previousPos, player.transform.position, lerp);
            cam.transform.position = new Vector3(newPos.x, newPos.y, cam.transform.position.z);
        }
        if (isShakeing)
        {
            int frequency = 8;
            Vector2 pos = this.transform.position;
            pos.x += (Mathf.PerlinNoise(seed1, timer * frequency) - 0.5f) * (0.5f - (1.2f * timer));
            pos.y += (Mathf.PerlinNoise(seed2, timer * frequency) - 0.5f) * (0.5f - (1.2f * timer));
            this.transform.position = new Vector3(pos.x, pos.y, this.transform.position.z);
        }
       
    }

    IEnumerator FindPlayer()
    {
        enabled = false;
        yield return new WaitUntil(() => (int)generator.currentStatus >= (int)LevelGenerator.GeneratingStatus.Final_Makeup);
        while(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform.Find("Player").gameObject;
            yield return null;
        }
        cam.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, cam.transform.position.z);
        controller = player.GetComponentInParent<PlayerController>();
        enabled = true;
    }

    public IEnumerator CamShake(float duration)
    {
        seed1 = UnityEngine.Random.Range(-5, 5);
        seed2 = UnityEngine.Random.Range(-5, 5);
        timer = 0;
        isShakeing = true;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        isShakeing = false;
        duration = 0;
    }
}
