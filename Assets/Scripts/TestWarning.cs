using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestWarning : MonoBehaviour
{
    private SpriteRenderer sr;
    private ParticleSystem ps;
    private Vector3 scale;
    public Ease ease;
    // Start is called before the first frame update
    void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
        ps = this.GetComponent<ParticleSystem>();
        scale = this.transform.localScale;
        StartCoroutine(SpawnWarning());
    }

    IEnumerator SpawnWarning()
    {
        sr.DOFade(1, 2f).SetEase(Ease.OutExpo);
        sr.DOColor(Color.white, 2f).SetEase(ease);
        this.transform.DOScale(1.75f * scale,2f).SetEase(ease);
        this.transform.DOShakePosition(2f, 0.3f, 30, 90, false, false).SetEase(ease);
        yield return new WaitForSeconds(2f);
        sr.enabled = false;
        ps.Play();
        float time = ps.main.duration;
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
