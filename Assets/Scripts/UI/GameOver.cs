using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameOver : MonoBehaviour
{
    public static GameOver instance = null;
    private CanvasGroup canvas;
    private SceneAudioManager manager;

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
        manager = SceneAudioManager.instance;
        canvas = GetComponent<CanvasGroup>();
        canvas.alpha = 0;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }
    
    public IEnumerator gameOver()
    {
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, 1, 0.5f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(0.5f);
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }

    public void LoadScene()
    {
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, 0, 0.5f).SetUpdate(true);
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
        manager.PlayByName("bgm", manager.sceneClips);
    }
}
