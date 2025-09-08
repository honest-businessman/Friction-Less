using NUnit.Framework;
using Pathfinding;
using UnityEngine;

using System.Collections.Generic;

public class EnemyController1 : MonoBehaviour
{
    public Transform player;
    public float preferredDistance = 8f;
    public float tolerance = 1f;

    public float minSeparation = 1.0f;
    public float separationForce = 1.5f;

    private static List<EnemyController1> allEnemies = new List<EnemyController1>();
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

        Vector3 target = player.position;

        foreach (var other in allEnemies)
        {
            if (other == this || other == null) continue;

            //float dist = Vector2.Distance(transform.position - other.transform.position).normalized;
            
            
        }

       
    }


    private void Awake()
    {
        allEnemies.Add(this);
    }
    void OnDestory()
    {
        allEnemies.Remove(this);
    }
}
