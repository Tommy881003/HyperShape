using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartContainer : MonoBehaviour
{
    public Image[] hearts;
    public Sprite empty, half, full;
    private PlayerController player;
    public static HeartContainer instance = null;
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
        player = GameObject.Find("DummyPlayer").GetComponent<PlayerController>();
        ShowHeart(true);
    }

    public void ShowHeart(bool start)
    {
        int maxLife = player.maxLife;
        int currentLife = (start ? player.maxLife : player.life);
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
