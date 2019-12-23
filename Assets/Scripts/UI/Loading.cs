using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public static Loading instance = null;
    public CanvasGroup group;
    public GameObject big, small;
    public bool isLoading = false;

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
        group.blocksRaycasts = false;
    }

    public IEnumerator SceneTransition(int sceneIndex)
    {
        isLoading = true;
        group.blocksRaycasts = true;
        big.transform.rotation = Quaternion.Euler(0, 0, 0);
        small.transform.rotation = Quaternion.Euler(0, 0, 0);
        DOTween.To(() => group.alpha, x => group.alpha = x, 1, 0.5f);
        big.transform.DORotate(new Vector3(0, 0, 360), 1.5f).SetOptions(false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        small.transform.DORotate(new Vector3(0, 0, -360), 1.5f).SetOptions(false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
        while (async.isDone == false)
            yield return null;
        yield return new WaitForSeconds(1);
        DOTween.To(() => group.alpha, x => group.alpha = x, 0, 0.5f);
        group.blocksRaycasts = false;
        isLoading = false;
    }
}
