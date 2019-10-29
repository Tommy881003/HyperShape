using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TempTitle : MonoBehaviour
{
    private TextMeshProUGUI[] texts;
    private PlayerController player;
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        cam = Camera.main;
        texts = this.GetComponentsInChildren<TextMeshProUGUI>();
        StartCoroutine(startTitle());
    }

    IEnumerator startTitle()
    {
        yield return new WaitUntil(() => Input.anyKey == true);
        foreach (TextMeshProUGUI text in texts)
            text.DOFade(0, 0.5f);
        cam.DOOrthoSize(13.5f, 1.5f);
        yield return new WaitForSeconds(1.5f);
        player.enabled = true;
    }
}
