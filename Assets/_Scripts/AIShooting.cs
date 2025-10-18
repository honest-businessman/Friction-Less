using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

// Class to give AI the functionality to aim at, and shoot a target using the modular FiringSystem
public class AIShooting : MonoBehaviour
{
    public float tryInterval = 0.5f;

    private Transform player; //Currently only supports shooting at the player
    private GameObject turret;
    private FiringSystem fireSystem;
    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        turret = FindTagInChildren("Turret").gameObject; // Return the GameObject if tag matches
        player = GameObject.FindWithTag("Player").transform;
        fireSystem = GetComponent<FiringSystem>();
        timer = tryInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            Vector2 direction = player.position - transform.position;
            float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            turret.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, rotationAngle - 90));

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                Shoot();
                timer = tryInterval;
            }
        }
    }

    void Shoot()
    {
        
        fireSystem.FireCommand();
    }

    private Transform FindTagInChildren(string tag)
    {
        foreach (Transform childTransform in transform)
        {
            // Check if the child's GameObject has the specified tag
            if (childTransform.CompareTag(tag))
            {
                return childTransform;
            }
        }
        return null;
    }
}
