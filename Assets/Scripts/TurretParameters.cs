using UnityEngine;

public class TurretParameters : MonoBehaviour
{
    public float shellSpeed = 9f;
    public float shellSize = 0.3f;
    public int shellBounces = 1;
    public int shellDamage = 1;
    public float fireRate = 2; // How many shells per second
    public float shellLifetime = 10f;
    public float spawnOffset = 1;
    public GameObject shellPrefab;
}
