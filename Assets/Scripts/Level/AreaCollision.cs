using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaCollision : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
            collision.gameObject.GetComponent<PlayerBulletInfo>().StartCoroutine(collision.gameObject.GetComponent<PlayerBulletInfo>().DelayDestroy());
    }
}
