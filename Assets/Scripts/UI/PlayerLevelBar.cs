﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerLevelBar : MonoBehaviour
{
    public static PlayerLevelBar instance = null;
    public PlayerController player = null;
    private LevelUp up;
    [SerializeField]
    private Scrollbar levelBar = null,novaBar = null;
    [SerializeField]
    private TextMeshProUGUI levelText = null,novaText = null;
    private int level = 1;
    [Range(50,200)]
    public float threshold = 150;
    private float levelExp;
    private float previousExp;
    private float nowExp = 0;
    private void Awake()
    {
        levelExp = threshold;
        previousExp = 0;
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        up = LevelUp.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.exp >= levelExp)
        {
            level++;
            previousExp = levelExp;
            levelExp += threshold * Mathf.Pow(1.25f, level - 1);
            levelText.text = "Lv : " + level.ToString();
            up.StartCoroutine(up.Upgrade());
        }    
        nowExp = player.exp - previousExp;
        levelBar.size = nowExp / (threshold * Mathf.Pow(1.25f, level - 1));
        novaBar.size = player.currentNovaTime / player.novaCoolDown;
        if (player.currentNovaTime == player.novaCoolDown)
            novaText.alpha = 1;
        else
            novaText.alpha = 0;
    }
}
