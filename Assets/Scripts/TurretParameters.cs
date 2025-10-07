using UnityEngine;

public class TurretParameters : MonoBehaviour
{
    public bool normalHitscan = false;
    public float shellSpeed = 9f;
    public float shellSize = 0.3f;
    public int shellBounces = 3;
    public int shellDamage = 1;
    public bool chargedHitscan = true;
    public float hitscanSize = 1f;
    public int hitscanBounces = 1;
    public int hitscanPenetration = 0;
    public int hitscanDamage = 1;
    public int hitscanRange = 100;
    public float fireRate = 2; // How many shells per second
    public float shellLifetime = 10f;
    public float spawnOffset = 1;
    public GameObject shellPrefab;
}
