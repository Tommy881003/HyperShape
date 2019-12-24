using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
    public UnityEvent Enable, Disable;

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
        pause.blocksRaycasts = false;
        canvas = GetComponent<Canvas>();
        cam = CameraFollower.instance.cam;
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = cam;
    }

    private void OnEnable()
    {
        Enable.Invoke();
        pause.alpha = 1;
        pause.interactable = true;
        pause.blocksRaycasts = true;
        music.value = 10 * audioManager.musicAmp;
        fx.value = 10 * audioManager.fxAmp;
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        Disable.Invoke();
        pause.alpha = 0;
        pause.interactable = false;
        pause.blocksRaycasts = false;
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
        Disable.Invoke();
        pause.alpha = 0;
        pause.interactable = false;
        pause.blocksRaycasts = false;
        Time.timeScale = 1;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Reload()
    {
        loading.StartCoroutine(loading.SceneTransition(SceneManager.GetActiveScene().buildIndex));
        Disable.Invoke();
        pause.alpha = 0;
        pause.interactable = false;
        pause.blocksRaycasts = false;
        Time.timeScale = 1;
    }
}
