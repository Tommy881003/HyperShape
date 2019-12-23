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

    // Start is called before the first frame update
    void Start()
    {
        loading = Loading.instance;
        Vector3 scale = transform.localScale;
        sr = GetComponent<SpriteRenderer>();
        sr.DOBlendableColor(pColor, 0.75f).SetLoops(-1, LoopType.Yoyo);
        transform.DOScale(0.75f*scale,0.5f).SetEase(Ease.OutQuad).SetLoops(-1, LoopType.Restart);
        transform.DOShakePosition(1, 0.5f, 15, 90, false,false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            trigger.Invoke();
    }

    public void NextScene()
    {
        loading.StartCoroutine(loading.SceneTransition(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void StartScene()
    {
        loading.StartCoroutine(loading.SceneTransition(0));
    }
}
