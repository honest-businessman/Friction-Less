using UnityEngine;
using Pathfinding;
using System.Collections;
public class ExplosionAttackSystme : MonoBehaviour
{
    
    [SerializeField]
    private GameObject explosionEffectPrefab;
    [SerializeField]
    private float destroyDelay = 0.3f;
    [SerializeField]
    private float explosionDelay = 1.5f;

    private AIPath aiPath;
    private bool hasExploded = false;
    private FactionController fc;

    private void Start()
    {
        fc = GetComponent<FactionController>();
        aiPath = GetComponent<AIPath>();
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (hasExploded) return;

        Debug.Log($"Triggered by: {other.gameObject.name}");

        if (other.TryGetComponent<FactionController>(out FactionController otherFC))
        {
            if (fc.IsSameFaction(otherFC))
            {
                Debug.Log("Same faction - ignoring.");
                return;
            }

            hasExploded = true;

            if(aiPath != null)
            {
                aiPath.canMove = false;
                aiPath.canSearch = false;
                aiPath.destination = transform.position;
            }

            StartCoroutine(DelayedExplosion());
           
            
        }
    }

    private IEnumerator DelayedExplosion()
    {
        yield return new WaitForSeconds(explosionDelay);

        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }
        Destroy(gameObject, destroyDelay);
    }
}
