using UnityEngine;

public class ExplosionAttackSystme : MonoBehaviour
{
    //[SerializeField]
    //private int damage = 1;
    [SerializeField]
    private GameObject explosionEffectPrefab;

    private FactionController fc;

    private void Start()
    {
        fc = GetComponent<FactionController>();
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Triggered by: {other.gameObject.name}");

        if (other.TryGetComponent<FactionController>(out FactionController otherFC))
        {
            if (fc.IsSameFaction(otherFC))
            {
                Debug.Log("Same faction - ignoring.");
                return;
            }

           
            if(explosionEffectPrefab != null)
            {
                GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
                Destroy(explosion, 2f);
            }
            Destroy(gameObject);
        }
    }
}
