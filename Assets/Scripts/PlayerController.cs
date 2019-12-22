using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using BulletCreator;

public enum WeaponType
{
    pistol = 0,
    rifle = 1,
    shotgun = 2,
    laser = 3
}

public class PlayerController : MonoBehaviour
{
    private GameObject player, carrier;
    public GameObject Nova;
    private Rigidbody2D rb;
    private CircleCollider2D circle;
    private Camera cam;
    private CameraFollower follower;
    private SpriteRenderer sr;
    private HeartContainer heart;
    [SerializeField]
    private ParticleSystem die = null,nova = null;
    [HideInInspector]
    public bool isShifting = false, canShift = true, canShoot = true;
    [HideInInspector]
    public bool isBattling = false;
    [HideInInspector]
    public bool vulnerable = true;
    [HideInInspector]
    public Vector2 direction = Vector2.zero;
    [Header("走路速度"), Range(1f,10f)]
    public float speed = 5;
    [Header("突刺最大距離"), Range(5f,15f)]
    public float shiftDis = 8;
    [Header("突刺冷卻時間"), Range(0.1f, 1)]
    public float shiftCoolDown = 0.3f;
    [Header("最大生命"), Range(4, 10)]
    public int maxLife = 8;
    [Header("傷後無敵時間"), Range(1, 2)]
    public float invincibleTime = 1.5f;
    [HideInInspector]
    public int life;
    [HideInInspector]
    public int currentNova = 3;
    public float exp = 0;
    private Weapon selectedWeapon;
    private ObjAudioManager audios;
    private PauseMenu menu = null;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        follower = cam.GetComponent<CameraFollower>();
        player = transform.Find("Player").gameObject;
        rb = player.GetComponent<Rigidbody2D>();
        circle = player.GetComponent<CircleCollider2D>();
        sr = player.GetComponent<SpriteRenderer>();
        selectedWeapon = this.GetComponentInChildren<Weapon>();
        life = maxLife;
        heart = HeartContainer.instance;
        heart.ShowHeart(true,this);
        audios = GetComponent<ObjAudioManager>();
        menu = PauseMenu.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && menu != null)
            menu.enabled = !menu.enabled;
        if (currentNova > 0 && Input.GetKeyDown(KeyCode.Q))
        {
            audios.PlayByName("nova");
            nova.Play();
            follower.Nova();
            currentNova--;
            GameObject newNova = Instantiate(Nova, player.transform.position, Quaternion.identity);
            DOTween.To(() => newNova.GetComponent<CircleCollider2D>().radius, x => newNova.GetComponent<CircleCollider2D>().radius = x, 50f, 0.5f);
            Destroy(newNova, 2);
        }
        if (follower.isCutScene)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) && canShift)
            StartCoroutine(Shift());
        if (isShifting == false)
            rb.velocity = Moving();
        player.transform.localScale = new Vector3(1 + 0.05f * (rb.velocity.magnitude / speed), 1 - 0.05f * (rb.velocity.magnitude / speed), 1);
        player.transform.rotation = Quaternion.Euler(0, 0, (rb.velocity.y < 0 ? -Vector2.Angle(Vector2.right, rb.velocity) : Vector2.Angle(Vector2.right, rb.velocity)));
    }

    Vector2 Moving()
    {
        direction = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
            direction += Vector2.up;
        if (Input.GetKey(KeyCode.S))
            direction += Vector2.down;
        if (Input.GetKey(KeyCode.A))
            direction += Vector2.left;
        if (Input.GetKey(KeyCode.D))
            direction += Vector2.right;
        return direction.normalized * speed;
    }

    IEnumerator Shift()
    {
        isShifting = true;
        vulnerable = false;
        canShift = false;
        audios.PlayByName("dash");
        float moveDis = shiftDis;
        Vector3 moveVect = Moving().normalized;
        RaycastHit2D ray = Physics2D.Raycast(player.transform.position, moveVect, shiftDis + circle.radius, 1 << 8);
        Debug.DrawRay(player.transform.position, moveVect * shiftDis,Color.white,2);
        if (ray.collider != null)
            moveDis = ray.distance - 0.5f * circle.radius;
        Vector3 moveTo = player.transform.position + moveVect * moveDis;
        player.transform.localScale = new Vector3(1.4f, 0.6f, 1);
        player.transform.rotation = Quaternion.Euler(0, 0, (moveVect.y < 0? -Vector2.Angle(Vector2.right, moveVect) : Vector2.Angle(Vector2.right, moveVect)));
        player.transform.DOMove(moveTo, 0.15f * moveDis / shiftDis);
        yield return new WaitForSeconds(0.15f);
        isShifting = false;
        vulnerable = true;
        yield return new WaitForSeconds(shiftCoolDown);
        canShift = true;
    }

    public IEnumerator GetDamage()
    {
        if (vulnerable == false)
            yield break;
        vulnerable = false;
        life--;
        if(life > 0)
        {
            audios.PlayByName("damage");
            follower.GetDamage();
        }
        else
        {
            circle.enabled = false;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            audios.PlayByName("over");
            follower.Die();
            die.Play();
            enabled = false;
        }
        heart.ShowHeart(false,this);
        float timer = 0;
        float flashTime = 0.1f;
        int i = 0;
        while(timer <= invincibleTime)
        {
            if(timer >= flashTime * i)
            {
                sr.color = (i % 2 == 0)? Color.clear : Color.white;
                i++;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        sr.color = Color.white;
        vulnerable = true;
    }
}
