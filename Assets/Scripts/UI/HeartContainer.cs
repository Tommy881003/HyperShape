using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HeartContainer : MonoBehaviour
{
    private Image lowHp;
    public Image[] hearts;
    public Sprite empty, half, full;
    public PlayerController player = null;
    public static HeartContainer instance = null;
    private float alpha = 0;
    private bool found = false;
    private SceneAudioManager sceneAudio;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        lowHp = GetComponent<Image>();
        player = GameObject.Find("DummyPlayer").GetComponent<PlayerController>();
        DOTween.To(() => alpha, x => alpha = x, 1f, 0.5f).SetLoops(-1,LoopType.Yoyo);
        sceneAudio = SceneAudioManager.instance;
    }

    public void Update()
    {
        if(player == null)
            player = GameObject.Find("DummyPlayer").GetComponent<PlayerController>();
        if (player != null && player.life <= 2)
        {
            if(player.life > 0)
                sceneAudio.PlayByName("low", sceneAudio.sceneClips);
            Color newColor = lowHp.color;
            newColor.a = alpha;
            lowHp.color = newColor;
        }
        else
        {
            Color newColor = lowHp.color;
            newColor.a = 0;
            lowHp.color = newColor;
            sceneAudio.StopByName("low", sceneAudio.sceneClips);
        }
    }

    public void ShowHeart(bool start, PlayerController backUp)
    {
        int maxLife;
        int currentLife;
        if (player == null)
        {
            maxLife = backUp.maxLife;
            currentLife = (start ? backUp.maxLife : backUp.life);
        }
        else
        {
            maxLife = player.maxLife;
            currentLife = (start ? player.maxLife : player.life);
        }
        int heartCount = maxLife / 2;
        int fillHeart = Mathf.CeilToInt((float)currentLife / 2f) - 1;
        int lastHeart = currentLife % 2;
        for(int i = 0; i < hearts.Length; i ++)
        {
            if (i < fillHeart)
            {
                hearts[i].sprite = full;
                hearts[i].color = new Color32(255, 255, 255, 150);
            }
            else if (i == fillHeart)
            {
                hearts[i].sprite = (lastHeart == 1 ? half : full);
                hearts[i].color = new Color32(255, 255, 255, 150);
            }
            else if (i > fillHeart && i < heartCount)
            {
                hearts[i].sprite = empty;
                hearts[i].color = new Color32(255, 0, 0, 150);
            }
            else
                hearts[i].color = Color.clear;
        }
    }
}
