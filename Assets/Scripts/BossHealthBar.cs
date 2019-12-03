using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossHealthBar : MonoBehaviour
{
    public static BossHealthBar instance = null;
    private CanvasGroup group;
    private Enemy boss;
    private Scrollbar healthBar;
    private float health;
    private void Awake()
    {
        healthBar = this.GetComponentInChildren<Scrollbar>();
        if (instance == null)
        {
            instance = this;
            enabled = false;
        }
        else
            Destroy(this);
        group = GetComponent<CanvasGroup>();
    }

    public IEnumerator StartBoss(Enemy B)
    {
        boss = B;
        DOTween.To(() => group.alpha, x => group.alpha = x, 1, 1);
        DOTween.To(() => healthBar.size, x => healthBar.size = x, 1, 1);
        yield return new WaitForSeconds(1);
        enabled = true;
    }

    public IEnumerator EndBoss()
    {
        boss = null;
        enabled = false;
        DOTween.To(() => group.alpha, x => group.alpha = x, 0, 1);
        yield return new WaitForSeconds(1);
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.size = (float)boss.hp / (float)boss.maxHp;
    }
}
