using UnityEngine;
using Pathfinding;

public class EnemySeparation : MonoBehaviour
{
    [SerializeField] private float separationDistance = 1.5f;
    [SerializeField] private float separationForce = 2.0f;
    [SerializeField] private string[] enemyTags = { "Enemy", "Enemy2", "Enemy3" };
    [SerializeField] private float updateInterval = 0.05f;

    private AIPath aiPath;
    private Transform player;
    private float updateTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        aiPath = GetComponent<AIPath>();
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (aiPath == null || player == null) return;

        updateTimer += Time.deltaTime;
        if(updateTimer >= updateInterval)
        {
            Vector3 separationVector = ComputeSeparationVector();
            aiPath.destination = player.position + separationVector;
            updateTimer = 0f;
        }
        
    }

    private Vector3 ComputeSeparationVector()
    {
        Collider2D[] others = Physics2D.OverlapCircleAll(transform.position, separationDistance);
         Vector2 separation = Vector2.zero;

        int count = 0;

        foreach (Collider2D other in others)
        {
            if (other.gameObject == gameObject) continue;

            bool isEnemy = false;

            foreach(string tag in enemyTags)
            {
                if(other.CompareTag(tag))
                {
                    isEnemy = true;
                    break;
                }
            }
            if (!isEnemy) continue;

            Vector2 away = (Vector2)(transform.position - other.transform.position);
            float distance = away.magnitude;
            if(distance > 0)
            {
                separation += away.normalized / distance;
                count++;
            }
        }

        if(count > 0)
        {
            separation /= count;
            separation *= separationForce;

            separation *= Time.deltaTime * 50f;
        }

        return separation;
        
    }

    public Vector3 GetSeparationVector()
    {
        return ComputeSeparationVector();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, separationDistance);
    }
}
