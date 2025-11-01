using UnityEngine;
using UnityEngine.UI;

public class XPContainer : MonoBehaviour
{
    [SerializeField] public GameObject xpPrefab;
    [SerializeField] public int minCount = 1;
    [SerializeField] public int maxCount = 3;
    [SerializeField] public int minValue = 1;
    [SerializeField] public int maxValue = 10;
    [SerializeField] public float launchForce = 3f;
    [SerializeField] public float dropSpread = 0.5f;

    public void SpawnXP(Vector2 spawnOrigin, Transform player)
    {
        int count = Random.Range(minCount, maxCount + 1);

        for(int i = 0; i < count; i++)
        {
            Vector2 spawnPos = spawnOrigin;
            int attempt = 10;

            while(attempt > 0)
            {
                Vector2 offset = Random.insideUnitCircle * dropSpread;
                Vector2 candidatePos = spawnOrigin + offset;

                if(Physics2D.OverlapCircle(candidatePos, 0.2f) == null)
                {
                    spawnPos = candidatePos;
                    break;
                }

                attempt--;
            }

            GameObject xp = Instantiate(xpPrefab, spawnPos, Quaternion.identity);

            int value = Random.Range(minValue, maxValue + 1);
            xp.GetComponent<XPObject>().Initialize(value, player, player.GetComponent<PlayerController>().pickupRadius);

            Rigidbody2D rb = xp.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                rb.AddForce(randomDir * launchForce, ForceMode2D.Impulse);
            }
        }
    }
        
}
