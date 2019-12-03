using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering.PostProcessing;

public class CameraFollower : MonoBehaviour
{
    private LevelGenerator generator = null;
    private Camera cam;
    public GameObject player = null;
    public PlayerController controller = null;
    private bool isShakeing;
    public bool isCutScene;
    [Header("攝影機跟隨強度"),Range(0f,1f)]
    public float lerp = 0.2f;
    [Header("攝影機跟隨強度"), Range(0f, 10f)]
    public float mousePlayerLerp = 5f;
    private float seed1,seed2,timer,shakePower = 0.5f,frequency = 12;
    private float originalShake, originalFrequency;
    private float oriLerp;
    private float screenRatio;
    private Vector3 previousLerp = Vector3.zero;
    private float pi = Mathf.PI;
    public bool isTest = false;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        oriLerp = lerp;
        screenRatio = (float)Screen.height / (float)Screen.width;
        cam = GetComponent<Camera>();
        isShakeing = false;
        seed1 = 0; seed2 = 0; timer = 0;
        originalFrequency = frequency; originalShake = shakePower;
        if(isTest == false)
            generator = GameObject.FindGameObjectWithTag("LevelGenerator").GetComponent<LevelGenerator>();
        if (generator == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform.Find("Player").gameObject;
            controller = player.GetComponentInParent<PlayerController>();
        }
        else
            StartCoroutine(FindPlayer());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isCutScene)
            return;
        if (player != null && controller != null)
        {
            if (controller.isShifting == true)
                lerp = 0.5f * oriLerp;
            else
                lerp = oriLerp;
            Vector2 previousPos = cam.transform.position - previousLerp;
            Vector2 newPos = Vector2.Lerp(previousPos, player.transform.position, lerp);
            Vector2 mousePos = new Vector2((2*Input.mousePosition.x - Screen.width) / Screen.width, (2*Input.mousePosition.y - Screen.height) / Screen.height);
            mousePos = new Vector2(mousePlayerLerp * mousePos.x, mousePlayerLerp * mousePos.y * screenRatio);
            previousLerp = mousePos;
            cam.transform.position = new Vector3(newPos.x + mousePos.x, newPos.y + mousePos.y, cam.transform.position.z);
        }
        if (isShakeing)
        {
            Vector2 pos = transform.position;
            pos.x += (Mathf.PerlinNoise(seed1, timer * frequency) - 0.5f) * (shakePower - (1.2f * timer));
            pos.y += (Mathf.PerlinNoise(seed2, timer * frequency) - 0.5f) * (shakePower - (1.2f * timer));
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
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

    public IEnumerator CamShake(float power, float duration)
    {
        frequency = originalFrequency * power;
        shakePower = originalShake * power;
        float rand1 = Random.Range(-5 * power, 5 * power);
        float rand2 = Random.Range(-5 * power, 5 * power);
        seed1 = rand1;
        seed2 = rand2;
        timer = 0;
        isShakeing = true;
        while (timer < duration)
        {
            timer += Time.fixedDeltaTime;
            yield return null;
        }
        isShakeing = false;
        duration = 0;
        yield return null;
    }
}
