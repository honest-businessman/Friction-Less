using JetBrains.Annotations;
using Pathfinding;
using UnityEngine;

public class AiTankController : CharacterBase
{
    public Transform player;
    public float preferredDistance = 8f;
    public float tolerance = 1f;
    private AIPath aiPath;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DriveCharge = 0f;
        player = GameObject.Find("Player").transform;
        aiPath = GetComponent<AIPath>();

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Vector3 targetPos;

        if (distance > preferredDistance + tolerance)
        {
            targetPos = player.position;
            //aiPath.canMove = true;
        }
        else if(distance < preferredDistance - tolerance)
        {
            targetPos = transform.position + (transform.position - player.position).normalized * 2f;
            //aiPath.canMove = true;
        }
        else
        {
            targetPos = transform.position;
            //aiPath.canMove = false;
        }
        var separation = GetComponent<EnemySeparation>()?.GetSeparationVector() ?? Vector3.zero;
        aiPath.destination = targetPos + separation * 0.8f;
    }
}
