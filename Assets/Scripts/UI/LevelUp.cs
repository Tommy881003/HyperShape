using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class LevelUp : MonoBehaviour
{
    public static LevelUp instance = null;
    private CanvasGroup canvas;
    public TextMeshProUGUI text;
    private PlayerController player;
    private Weapon weapon;
    private HeartContainer heart;

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
        canvas = GetComponent<CanvasGroup>();
        canvas.interactable = false;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        weapon = player.gameObject.GetComponentInChildren<Weapon>();
        heart = HeartContainer.instance;
    }

    public IEnumerator Upgrade()
    {
        weapon.canAttack = false;
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0, 1).SetUpdate(true);
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, 1, 1).SetUpdate(true);
        yield return new WaitForSecondsRealtime(1.1f);
        canvas.interactable = true;
    }

    public void Health()
    {
        StartCoroutine(setAtk());
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 1.5f).SetUpdate(true);
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, 0, 0.5f).SetUpdate(true);
        canvas.interactable = false;
        player.maxLife += 2;
        player.life += 2;
        heart.ShowHeart(false, player);
    }

    public void Gun()
    {
        StartCoroutine(setAtk());
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 1.5f).SetUpdate(true);
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, 0, 0.5f).SetUpdate(true);
        canvas.interactable = false;
        weapon.AddWeapon();
    }

    IEnumerator setAtk()
    {
        yield return new WaitForSecondsRealtime(1);
        weapon.canAttack = true;
    }
}
