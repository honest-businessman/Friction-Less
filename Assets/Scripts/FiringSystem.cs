using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class FiringSystem : MonoBehaviour
{
    [SerializeField]
    private InputHandler inputHandler;

    private float firingTimer = 0f;
    private GameObject turret;
    private TurretParameters turretParameters;
    private Vector2 firePosition;
    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        foreach (Transform childTransform in transform)
        {
            // Check if the child's GameObject has the specified tag
            if (childTransform.CompareTag("Turret"))
            {
                turret = childTransform.gameObject; // Return the GameObject if tag matches
                turretParameters = turret.GetComponent<TurretParameters>();
            }
        }
        if (turret == null)
        {
            Debug.LogError("No Turret found on " + gameObject);
        }
    }

    private void OnEnable()
    {
        inputHandler.OnInputFire.AddListener(Fire);
    }
    private void OnDisable()
    {
        inputHandler.OnInputFire.RemoveListener(Fire);
    }

    void Fire(InputValue value)
    {
        if(Time.time - firingTimer >= 1 / turretParameters.fireRate) // Divide fire rate by 1 to convert fire rate to shells per second
        {
            firingTimer = Time.time;
            firePosition = turret.transform.position + (turret.transform.up * turretParameters.spawnOffset);
            GameObject shell = Instantiate(turretParameters.shellPrefab, firePosition, turret.transform.rotation);
            shell.GetComponent<ShellController>().Initialize(turretParameters.shellBounces, turretParameters.shellLifetime);
            shell.transform.localScale = new Vector3(turretParameters.shellSize, turretParameters.shellSize, turretParameters.shellSize);
            shell.GetComponent<Rigidbody>().linearVelocity = shell.transform.up * turretParameters.shellSpeed;
        }
    }

}
