using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestWarning : MonoBehaviour
{
    private SpriteRenderer sr;
    private ParticleSystem ps;
    private SceneAudioManager manager;
    private Vector3 scale;
    public AudioSource shake, spawn;
    private PauseMenu menu;

    private void OnDestroy()
    {
        menu.Enable.RemoveListener(Pause);
        menu.Disable.RemoveListener(UnPause);
    }
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        ps = GetComponent<ParticleSystem>();
        scale = transform.localScale;
        manager = SceneAudioManager.instance;
        menu = PauseMenu.instance;
        menu.Enable.AddListener(Pause);
        menu.Disable.AddListener(UnPause);
        StartCoroutine(SpawnWarning());
    }

    IEnumerator SpawnWarning()
    {
        Vector3 scale = transform.localScale;
        sr.DOFade(1, 2f);
        sr.DOBlendableColor(Color.white, 2f).SetEase(Ease.InCubic);
        transform.DOScale(1.75f * scale, 2f).SetEase(Ease.InCubic);
        transform.DOShakePosition(2f, 0.3f, 30, 90, false, false).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(0.25f);
        shake.volume *= manager.fxAmp;
        shake.Play();
        yield return new WaitForSeconds(1.75f);
        spawn.volume *= manager.fxAmp;
        spawn.Play();
        sr.enabled = false;
        ps.Play();
        float time = ps.main.duration;
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    private void Pause()
    {
        shake.Pause();
        spawn.Pause();
    }

    private void UnPause()
    {
        shake.UnPause();
        spawn.UnPause();
    }
}
