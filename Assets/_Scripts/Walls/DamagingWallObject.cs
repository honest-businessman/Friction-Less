using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageWallObject : WallObject
{
    public int damage = 2;
    public float damageCooldown = 1f;

    private Dictionary<GameObject, float> lastDamageTime = new Dictionary<GameObject, float>();

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!lastDamageTime.ContainsKey(other.gameObject))
                lastDamageTime[other.gameObject] = 0f;

            if (Time.time - lastDamageTime[other.gameObject] >= damageCooldown)
            {
                lastDamageTime[other.gameObject] = Time.time;

                var health = other.GetComponent<HealthSystem>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                }
            }
        }
    }
}
