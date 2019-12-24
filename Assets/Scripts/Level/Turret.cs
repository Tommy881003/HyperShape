using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private SceneAudioManager audios;
    private BulletManager manager;
    private AudioSource shoot;
    private Quaternion rotate;
    public float shootTime;
    private float volume;
    public BulletPattern pattern;
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        audios = SceneAudioManager.instance;
        manager = BulletManager.instance;
        shoot = GetComponent<AudioSource>();
        rotate = transform.rotation;
        volume = shoot.volume;
        player = GameObject.FindGameObjectWithTag("Player").transform.Find("Player");
    }

    void Shoot()
    {
        float value = Mathf.Clamp01((25f - (player.position - transform.position).magnitude) / 25f);
        shoot.volume = volume * value * audios.fxAmp;
        shoot.Play();
        Vector3 lookAt = new Vector3(Mathf.Cos(rotate.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(rotate.eulerAngles.z * Mathf.Deg2Rad));
        manager.StartCoroutine(manager.SpawnPattern(pattern, transform.position + 0.5f * lookAt, rotate));
    }
}
