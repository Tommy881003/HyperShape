using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeBug : MonoBehaviour
{
    public CanvasGroup group;
    public TextMeshProUGUI player;
    public TextMeshProUGUI enemy;
    public TextMeshProUGUI fps;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            group.alpha = (group.alpha > 0.1f) ? 0 : 1;
        player.text = "Player : " + PlayerBulletInfo.currentBulletAmount;
        enemy.text = "Enemy : " + BulletManager.currentBulletAmount;
        fps.text = "FPS : " + (int)(1.0f / Time.deltaTime);
    }
}
