using UnityEngine;

[CreateAssetMenu(fileName = "TurretSettings", menuName = "Scriptable Objects/Turret Settings")]
public class TurretSettings : ScriptableObject
{
    public Sprite sprite;
    public Color spriteColor;
    public bool isNormalHitscan = false;
    public float shellSpeed = 9f;
    public float shellSize = 0.3f;
    public int shellBounces = 3;
    public int shellDamage = 1;
    public bool isChargedHitscan = true;
    public float hitscanSize = 1f;
    public int hitscanBounces = 1;
    public int hitscanPenetrations = 0;
    public int hitscanDamage = 1;
    public int hitscanRange = 100;
    public float fireRate = 2; // Projectiles per second
    public float shellLifetime = 10f;
    public float spawnOffset = 1;
    public GameObject shellPrefab;
}
