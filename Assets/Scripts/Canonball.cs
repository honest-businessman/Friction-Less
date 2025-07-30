using UnityEngine;

public class Canonball : MonoBehaviour
{
    [SerializeField]
    private float canonSpeed = 1f;
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
        transform.LookAt(player.transform);
        Destroy(gameObject, 10);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * canonSpeed * Time.deltaTime);
        
    }
}
