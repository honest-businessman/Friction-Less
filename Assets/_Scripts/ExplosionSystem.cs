using System.Collections;
using UnityEngine;

public class ExplosionSystem : MonoBehaviour
{
    public void Explode(ExplosionSettings settings)
    {
        ParticleSystem particles = Instantiate(settings.explosionVFX, gameObject.transform);
        ParticleSystem.ShapeModule shape = particles.shape;
        shape.radius = settings.radius;
        particles.Play();

        Debug.Log($"Explosion at {transform.position} with radius {settings.radius} and damage {settings.damage}");
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, settings.radius, settings.affectedLayers);

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<HealthSystem>(out HealthSystem health))
            {
                Debug.Log($"Explosion hit: {hit.gameObject.name}");
                health.TakeDamage(settings.damage);
            }
        }
        StartCoroutine(DestroyAfterParticles(particles));
    }

    private IEnumerator DestroyAfterParticles(ParticleSystem particles)
    {
        yield return new WaitUntil(() => !particles.IsAlive(true));
        Destroy(gameObject);
    }
}
