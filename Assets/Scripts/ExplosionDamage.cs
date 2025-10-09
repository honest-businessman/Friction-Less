using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 1.14f;
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask damageableLayer;

    void Start()
    {
        ApplyExplosionDamage();


    }

    private void ApplyExplosionDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageableLayer);

        foreach(Collider2D hit in hits)
        {
            if(hit.TryGetComponent<HealthSystem>(out HealthSystem health))
            {
                Debug.Log($"Explosion hit: {hit.gameObject.name}");
                health.TakeDamage(damage);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

}
