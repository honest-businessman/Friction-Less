using Pathfinding;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player;
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player").transform;
       
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetPosition = new Vector3(player.position.x, player.position.y, -10);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            
        
    }
}
