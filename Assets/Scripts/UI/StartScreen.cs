using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public static StartScreen instance;
    public GameObject big, small;
    private PlayerController player = null;
    private PauseMenu menu;
    private CanvasGroup group;

    void Start()
    {
        menu = PauseMenu.instance;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        group = GetComponent<CanvasGroup>();
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            group.alpha = 0;
            return;
        }
        if (player != null)
            player.enabled = false;
        group.alpha = 1;
        big.transform.rotation = Quaternion.Euler(0, 0, 0);
        small.transform.rotation = Quaternion.Euler(0, 0, 0);
        big.transform.DORotate(new Vector3(0, 0, 45), 1.5f).SetOptions(false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        small.transform.DORotate(new Vector3(0, 0, -45), 1.5f).SetOptions(false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        StartCoroutine(AnyKey());
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    IEnumerator AnyKey()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return) == true);
        player.enabled = true;
        big.transform.DOKill();
        small.transform.DOKill();
        DOTween.To(() => group.alpha, x => group.alpha = x, 0, 0.5f);
    }

    void OnSceneLoad(Scene e, LoadSceneMode l)
    {
        if (e.buildIndex != 0)
        {
            group.alpha = 0;
            return;
        }      
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (player != null)
            player.enabled = false;
        group.alpha = 1;
        big.transform.rotation = Quaternion.Euler(0, 0, 0);
        small.transform.rotation = Quaternion.Euler(0, 0, 0);
        big.transform.DORotate(new Vector3(0, 0, 45), 1.5f).SetOptions(false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        small.transform.DORotate(new Vector3(0, 0, -45), 1.5f).SetOptions(false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        StartCoroutine(AnyKey());
    }
}
