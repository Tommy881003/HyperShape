using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; 
using DG.Tweening;

public class Portal : MonoBehaviour
{
    private SpriteRenderer sr;
    private Loading loading = null;
    public Color pColor;
    public UnityEvent trigger;
    private AudioSource audios;
    private SceneAudioManager manager;
    private PauseMenu menu;
    private float volume;

    // Start is called before the first frame update
    void Start()
    {
        audios = GetComponentInParent<AudioSource>();
        volume = audios.volume;
        loading = Loading.instance;
        Vector3 scale = transform.localScale;
        manager = SceneAudioManager.instance;
        menu = PauseMenu.instance;
        menu.Enable.AddListener(Pause);
        menu.Disable.AddListener(UnPause);
        sr = GetComponent<SpriteRenderer>();
        sr.DOBlendableColor(pColor, 0.75f).SetLoops(-1, LoopType.Yoyo);
        transform.DOScale(0.75f*scale,0.5f).SetEase(Ease.OutQuad).SetLoops(-1, LoopType.Restart);
        transform.DOShakePosition(1, 0.5f, 15, 90, false,false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }

    private void Update()
    {
        audios.volume = volume * manager.fxAmp;
    }

    void Pause()
    {
        audios.Pause();
    }

    void UnPause()
    {
        audios.UnPause();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            trigger.Invoke();
    }

    public void NextScene()
    {
        DOTween.To(() => audios.volume, x => audios.volume = x, 0, 0.5f);
        loading.StartCoroutine(loading.SceneTransition(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void StartScene()
    {
        loading.StartCoroutine(loading.SceneTransition(0));
    }
}
