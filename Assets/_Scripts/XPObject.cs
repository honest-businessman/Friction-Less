using UnityEngine;

public class XPObject : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] public int xpValue = 1;
    [SerializeField] public float moveSpeed = 3f;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private float pickupRadius = 1f;

    private bool isAttracting = false;


    // Update is called once per frame
    void Update()
    {
        if (playerTarget == null) return;

        float dist = Vector3.Distance(transform.position, playerTarget.position);

        if (!isAttracting && dist <= pickupRadius) isAttracting = true;


        if(isAttracting)
        {
            Vector3 dir = (playerTarget.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;

            if(Vector3.Distance(transform.position,, playerTarget.position) <= 0.1f)
            {
                LevelSystem levelSystem = playerTarget.GetComponent<LevelSystem>();
                if (levelSystem != null)
                {
                    levelSystem.AddXP(xpValue);
                }
                Destroy(gameObject);
            } 
        }
    }

    public void Initialize(int value, Transform player, float radius)
    {
        xpValue = value;
        playerTarget = player;
        pickupRadius = radius;
    }
}
