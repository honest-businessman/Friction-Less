using UnityEngine;

public class MeleeSystem : MonoBehaviour
{
    [SerializeField]
    private MeleeSettings settings;

    private float meleeTimer = 0f;
    private FactionController fc;

    private void Start()
    {
        fc = GetComponent<FactionController>();
    }
    public void OnTriggerStay2D(Collider2D other)
    {
        if (Time.time - meleeTimer >= 1 / settings.fireRate) // Divide fire rate by 1 to convert fire rate to shells per second
        {
            if (other.TryGetComponent<FactionController>(out FactionController otherFC))
            {
                Debug.Log($"Triggered by: {other.gameObject.name}");
                if (!fc.IsSameFaction(otherFC))
                {
                    if (other.TryGetComponent<HealthSystem>(out HealthSystem otherHS))
                    {
                        meleeTimer = Time.time;
                        Debug.Log($"Dealing {settings.damage} damage to target.");
                        otherHS.TakeDamage(settings.damage);
                        if (settings.destroyAfterAttack) { Destroy(gameObject); }
                    }
                }
                else
                {
                    Debug.Log("Same faction - ignoring.");
                    return;
                }
            }
        }
    }
}
