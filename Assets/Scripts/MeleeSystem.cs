using UnityEngine;

public class MeleeSystem : MonoBehaviour
{
    [SerializeField]
    private int damage = 1;

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

            if (other.TryGetComponent<HealthSystem>(out HealthSystem otherHS))
            {
                Debug.Log($"Dealing {damage} damage to target.");
                otherHS.TakeDamage(damage);
                //Destroy(gameObject);
            }
        }
    }
}
