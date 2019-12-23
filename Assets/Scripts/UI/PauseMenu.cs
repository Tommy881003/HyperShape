using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance = null;
    public Camera cam;
    public SceneAudioManager audioManager;
    public CanvasGroup pause;
    [SerializeField]
    private Slider music = null, fx = null;
    public Loading loading;
    public Canvas canvas;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        audioManager = SceneAudioManager.instance;
        loading = Loading.instance;
        enabled = false;
        pause.alpha = 0;
        pause.interactable = false;
        canvas = GetComponent<Canvas>();
        cam = CameraFollower.instance.cam;
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = cam;
    }

    private void OnEnable()
    {
        pause.alpha = 1;
        pause.interactable = true;
        music.value = 10 * audioManager.musicAmp;
        fx.value = 10 * audioManager.fxAmp;
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        pause.alpha = 0;
        pause.interactable = false;
        Time.timeScale = 1;
    }

    public void MusicAmp()
    {
        audioManager.musicAmp = music.value * 0.1f;
    }

    public void FXAmp()
    {
        audioManager.fxAmp = fx.value * 0.1f;
    }

    public void Credit()
    {
        Debug.Log("Credit");
    }

    public void Return()
    {
        enabled = false;
    }

    public void Title()
    {
        loading.StartCoroutine(loading.SceneTransition(0));
        enabled = false;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
