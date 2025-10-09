using Pathfinding;
using UnityEngine;

public class EnemyController3 : MonoBehaviour
{
    public Transform player;
    private AIPath aiPath;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player").transform;
        aiPath = GetComponent<AIPath>();

        if(player != null)
        {
            aiPath.destination = player.position;
            aiPath.canSearch = true;
            aiPath.canMove = true;
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            aiPath.destination = player.position;
        }
    }
}
