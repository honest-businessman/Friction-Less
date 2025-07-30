using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Find");
        if (other.gameObject.name == "Player") 
        {
            transform.LookAt(Player);
            transform.Translate(0,0,0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
