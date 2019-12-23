using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using JetFistGames.RetroTVFX;

public class CameraFollower : MonoBehaviour
{
    public static CameraFollower instance = null;
    private LevelGenerator generator = null;
    [HideInInspector]
    public Camera cam;
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
    public bool isTest = false;
    private GlitchEffect glitch;
    private CRTEffect cRT;
    private SceneAudioManager sceneAudio;
    private RippleEffect ripple;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (instance == null)
        {
            instance = this;
            cam = GetComponent<Camera>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        oriLerp = lerp;
        screenRatio = (float)Screen.height / (float)Screen.width;
        isShakeing = false;
        seed1 = 0; seed2 = 0; timer = 0;
        originalFrequency = frequency; originalShake = shakePower;
        glitch = GetComponent<GlitchEffect>();
        cRT = GetComponent<CRTEffect>();
        ripple = GetComponent<RippleEffect>();
        sceneAudio = SceneAudioManager.instance;
        if(isTest == false)
            generator = GameObject.FindGameObjectWithTag("LevelGenerator").GetComponent<LevelGenerator>();
        if (generator == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform.Find("Player").gameObject;
            controller = player.GetComponentInParent<PlayerController>();
        }
        else
            StartCoroutine(FindPlayer());
        SceneManager.sceneLoaded += OnSceneLoad;
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

    public void GetDamage()
    {
        StartCoroutine(CamShake(3, 0.75f));
        glitch.intensity = 1;
        glitch.flipIntensity = 1;
        glitch.colorIntensity = 1;
        glitch.flickIntensity = 1;
        DOTween.To(() => glitch.intensity, x => glitch.intensity = x, 0f, 0.75f).SetEase(Ease.InExpo);
        DOTween.To(() => glitch.flipIntensity, x => glitch.flipIntensity = x, 0f, 0.75f).SetEase(Ease.InExpo);
        DOTween.To(() => glitch.colorIntensity, x => glitch.colorIntensity = x, 0f, 0.75f).SetEase(Ease.InExpo);
        DOTween.To(() => glitch.flickIntensity, x => glitch.flickIntensity = x, 0f, 0.75f).SetEase(Ease.InExpo);
    }

    public void Die()
    {
        StartCoroutine(CamShake(3, 1f));
        sceneAudio.StopAll();
        glitch.intensity = 1;
        glitch.flipIntensity = 1;
        glitch.colorIntensity = 1;
        glitch.flickIntensity = 1;
        DOTween.To(() => glitch.intensity, x => glitch.intensity = x, 0f, 1f);
        DOTween.To(() => glitch.flipIntensity, x => glitch.flipIntensity = x, 0f, 1f);
        DOTween.To(() => glitch.colorIntensity, x => glitch.colorIntensity = x, 0f, 1f);
        DOTween.To(() => glitch.flickIntensity, x => glitch.flickIntensity = x, 0f, 1f);
        DOTween.To(() => cRT.IQScale.x, x => cRT.IQScale.x = x, 0f, 1f);
        DOTween.To(() => cRT.IQScale.y, x => cRT.IQScale.y = x, 0f, 1f);
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0f, 2f).SetUpdate(true).SetEase(Ease.Linear);
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

    public void Nova()
    {
        ripple.Emit(cam.WorldToViewportPoint(player.transform.position));
        StartCoroutine(CamShake(2.5f, 0.5f));
    }

    void OnSceneLoad(Scene s, LoadSceneMode l)
    {
        if (isTest == false)
            generator = GameObject.FindGameObjectWithTag("LevelGenerator").GetComponent<LevelGenerator>();
        if (generator == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform.Find("Player").gameObject;
            controller = player.GetComponentInParent<PlayerController>();
        }
        else
            StartCoroutine(FindPlayer());
    }
}
