using Pathfinding;
using UnityEngine;

public abstract class AiMeleeBase : MonoBehaviour
{
    [SerializeField] private float updateInterval = 0.05f;

    protected Transform Player { get; set; }
    protected AIPath AiPath { get; set; }

    private float updateTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        Player = GameObject.FindWithTag("Player").transform;
        AiPath = GetComponent<AIPath>();

        if(AiPath != null)
        {
            AiPath.canSearch = true;
            AiPath.canMove = true;
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        if (Player == null || AiPath == null) return;

        updateTimer += Time.deltaTime;
        if(updateTimer >= updateInterval)
        {
            AiPath.destination = Player.position;

            updateTimer = 0f;
        }
        
    }
}
