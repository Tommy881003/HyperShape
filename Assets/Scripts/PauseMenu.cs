using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance = null;
    private SceneAudioManager audioManager;
    private CanvasGroup group;
    [SerializeField]
    private Slider music = null, fx = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        audioManager = SceneAudioManager.instance;
        group = GetComponentInChildren<CanvasGroup>();
        enabled = false;
        group.alpha = 0;
        group.interactable = false;
    }

    private void OnEnable()
    {
        group.alpha = 1;
        group.interactable = true;
        music.value = 10 * audioManager.musicAmp;
        fx.value = 10 * audioManager.fxAmp;
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        group.alpha = 0;
        group.interactable = false;
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
        Debug.Log("ToTitle");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
