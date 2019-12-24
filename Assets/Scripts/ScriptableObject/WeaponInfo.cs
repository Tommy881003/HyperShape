using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum weapon_type
{
    semi_auto,
    auto,
    burst,
    charge
}

[CreateAssetMenu]
public class WeaponInfo : ScriptableObject
{
    public weapon_type type;
    public GameObject bullet;
    [Range(10f, 150f)]
    public float speed;
    public float damage;
    [Range(0f, 3f)]
    public int level = 0;
    [Range(1f, 15f)]
    public int count = 1;
    public int burst_count;
    public float burst_spacing;
    public float fireRate;
    public int ammoCapacity;
    public float reloadTime;
    public float spread;
    public WeaponInfo[] upgrades;
}
