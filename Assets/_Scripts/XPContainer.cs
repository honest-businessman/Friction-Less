using UnityEngine;

public class XPContainer : MonoBehaviour
{
    [SerializeField] public GameObject xpPrefab;
    [SerializeField] public int minXP = 1;
    [SerializeField] public int maxXP = 3;
    [SerializeField] public int minValue = 1;
    [SerializeField] public int maxValue = 10;
    [SerializeField] public float launchForce = 3f;

   public void SpawnXP(Transform player)
    {
        int count = Random.Range(minXP, maxXP + 1);

        for(int i = 0; i < count; i++)
        {
            GameObject xp = Instantiate(xpPrefab, transform.position, Quaternion.identity);

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
