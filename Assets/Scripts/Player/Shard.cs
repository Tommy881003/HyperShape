using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shard : MonoBehaviour
{
    private SpriteRenderer sr;
    public ParticleSystem ps,spark;
    private CircleCollider2D cc;
    private GameObject player = null;
    private PlayerController controller = null;
    private Rigidbody2D rb;
    private int rand;
    private bool follow = false;
    private AudioSource audios;
    private SceneAudioManager manager;
    private HeartContainer container;
    public bool health;

    void Start()
    {
        rand = Random.Range(0, 3);
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();
        rb.angularVelocity = Random.Range(-60, 60);
        player = GameObject.FindGameObjectWithTag("Player").transform.Find("Player").gameObject;
        controller = player.GetComponentInParent<PlayerController>();
        audios = GetComponent<AudioSource>();
        manager = SceneAudioManager.instance;
        container = HeartContainer.instance;
    }

    private void Update()
    {
        if (health && controller.life == controller.maxLife)
            follow = false;
        if(follow == true)
        {
            float distance = (transform.position - player.transform.position).magnitude;
            float angle = Vector2.SignedAngle(Vector2.right, player.transform.position - transform.position) * Mathf.Deg2Rad;
            rb.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Mathf.Lerp(17.5f, 8, distance / 5);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Collect"))
            if ((health && controller.life < controller.maxLife) || !health)
                follow = true;
        if (collision.CompareTag("Player"))
            if((health && controller.life < controller.maxLife) || !health)
                StartCoroutine(AddExp());
    }

    IEnumerator AddExp()
    {
        audios.volume *= manager.fxAmp;
        audios.Play();
        cc.enabled = false;
        sr.enabled = false;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        if (health)
        {
            controller.life = Mathf.Min(controller.life + 1, controller.maxLife);
            container.ShowHeart(false, controller);
        }
        else
            controller.exp += 5;
        ps.Stop();
        spark.Play();
        yield return new WaitForSeconds(ps.main.duration);
        Destroy(gameObject);
    }
}
