using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Unlocker_Tile : MonoBehaviour
{
    private BoxCollider2D box;
    private SpriteRenderer sr;
    [SerializeField]
    private PlayerController player;
    private Color oriColor;
    [HideInInspector]
    public bool isPoping;
    public Vector3 scale;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerController>();
        box.enabled = false;
        oriColor = sr.color;
        scale = sr.gameObject.transform.localScale;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && player.vulnerable == true)
            StartCoroutine(player.GetDamage());
    }

    public IEnumerator Pop(float warnTime, float PopTime)
    {
        isPoping = true;
        sr.DOBlendableColor(new Color32(255,89,89,0),0.1f).SetLoops(-1, LoopType.Yoyo);
        sr.DOFade(0.5f, warnTime);
        yield return new WaitForSeconds(warnTime);
        sr.DOKill();
        sr.color = Color.white;
        box.enabled = true;
        sr.DOColor(new Color32(255, 89, 89, 255), 0.4f);
        sr.gameObject.transform.localScale = 1.4f * scale;
        sr.sortingOrder = 3;
        sr.gameObject.transform.DOScale(scale, 0.4f);
        yield return new WaitForSeconds(PopTime);
        sr.gameObject.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
        box.enabled = false;
        yield return new WaitForSeconds(0.2f);
        sr.sortingOrder = 2;
        sr.color = oriColor;
        sr.gameObject.transform.localScale = scale;
        isPoping = false;
    }
}
