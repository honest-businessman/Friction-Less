using Pathfinding;
using UnityEngine;

public class EnemyController1 : MonoBehaviour
{
    public Transform player;
    public float preferredDistance = 8f;
    public float tolerance = 1f;

    private AIPath aiPath;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player").transform;
        aiPath = GetComponent<AIPath>();

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > preferredDistance + tolerance)
        {
            aiPath.destination = player.position;
            aiPath.canMove = true;
        }
        else if(distance < preferredDistance - tolerance)
        {
            Vector2 away = transform.position + (transform.position - player.position).normalized * 2f;
            aiPath.destination = away;
            aiPath.canMove = true;
        }
        else
        {
            aiPath.canMove = false;
        }
    }
}
