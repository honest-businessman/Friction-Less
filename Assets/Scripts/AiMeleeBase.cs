using Pathfinding;
using UnityEngine;

public abstract class AiMeleeBase : MonoBehaviour
{
    protected Transform Player { get; set; }
    protected AIPath AiPath { get; set; }
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        Player = GameObject.Find("Player").transform;
        AiPath = GetComponent<AIPath>();

        if(Player != null)
        {
            AiPath.destination = Player.position;
            AiPath.canSearch = true;
            AiPath.canMove = true;
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        if(Player != null)
        {
            AiPath.destination = Player.position;
        }
    }
}
