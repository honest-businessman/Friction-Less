using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float eMoveSpeed = 1f;
    

    public Transform player;
    public GameObject canonball;
    private int count = 0;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Find");
        if (other.gameObject.name == "Player") 
        {
            //Vector3 direction = player.position - transform.position;
            
            //if (direction != Vector3.zero)
            //{
            //    Quaternion targetRotation = Quaternion.LookRotation(direction);
            //    Vector3 euler = targetRotation.eulerAngles;
            //    euler.z = 0;
            //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), Time.deltaTime * 5f);
            //}
            //transform.Translate(Vector3.forward * eMoveSpeed * Time.deltaTime);

            transform.LookAt(player);
            transform.Translate(0,0,eMoveSpeed * Time.deltaTime);

            count++;
            if(count % 100 == 0)
            {
                Instantiate(canonball, transform.position, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
