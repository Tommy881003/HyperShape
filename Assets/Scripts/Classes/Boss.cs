using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Boss : Enemy
{
    public virtual IEnumerator StartCutScene()
    {
        yield break;
    }

    public virtual IEnumerator EndCutScene()
    {
        yield break;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            PlayerBulletInfo info = collision.gameObject.GetComponent<PlayerBulletInfo>();
            info.StartCoroutine(info.DelayDestroy());
            hp -= info.damage;
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Contact");
            player.StartCoroutine(player.GetDamage());
        }
    }

    public override IEnumerator Hurt()
    {
        yield break;
    }
}
