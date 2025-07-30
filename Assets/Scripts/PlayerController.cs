using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    public float moveSpeed = 25f; // Movement speed
    public float maxSpeed = 5f; // Maximum speed the player can reach
    public float driftSpeed = 5f;
    public float defaultDrag = 8f;
    public float rotationSpeed = 15f; // Rotation speed
    private float moveSpeedMultiplier = 100f; // Adjusted multiplier for movement speed
    public float driftTurnStrength = 2f; // Strength of the drift turn
    public float treadOffset = 0.2f; // Offset for the rotation pivot point
    private float rotationSpeedMultiplier = 10f; // Adjusted multiplier for rotation speed
    private float debugOffset = 2f; // Offset for the debug line length

    private bool isDrifting = false; // Flag to check if the player is drifting
    private Vector2 playerInput;
    private Vector3 velocity;
    private Rigidbody rb;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        defaultDrag = rb.linearDamping;
    }

    private void OnMove(InputValue value)
    {
        playerInput = value.Get<Vector2>();
    }

    private void OnDrift(InputValue value)
    {
        isDrifting = value.isPressed;
    }

    void FixedUpdate()
    {
        if (isDrifting)
        {
            rb.linearDamping = 0f; // Reduce linear damping when drifting
        }
        else
        {
           rb.linearDamping = defaultDrag; // Reset linear damping when not drifting
        }

        PlayerMovement();
    }
    private void PlayerMovement()
    {
        Debug.DrawRay(transform.position, transform.up * 2, Color.green); // Draw ray pointing up from the player
        if (!isDrifting)
            rb.AddForce(transform.up * playerInput.y * (1.5f - Mathf.Abs(playerInput.x)) * moveSpeed * moveSpeedMultiplier * Time.deltaTime, ForceMode.Force);
        else
        {
            rb.AddForce(transform.up * playerInput.y * driftSpeed * moveSpeedMultiplier * Time.deltaTime, ForceMode.Force);
            
            //DriftMovement();
        }
        PlayerRotate();

        // Clamp the velocity to the maximum speed
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }
    private void PlayerRotate()
    {
        Vector3 rotationPoint = transform.position; // Point to rotate around
        Vector3 debugPoint = transform.position; // Point to draw debug line to
        if (Mathf.Abs(playerInput.y) > 0.1)
        {
            if (playerInput.x < 0)
            {
                rotationPoint += -transform.right * treadOffset;
                debugPoint += -transform.right * treadOffset * debugOffset;
            }
            else if (playerInput.x > 0)
            {
                rotationPoint += transform.right * treadOffset;
                debugPoint += transform.right * treadOffset * debugOffset;
            }
        }
        Debug.DrawLine(transform.position, debugPoint, Color.blue); // Draw line roughly pointing to pivot point
        transform.RotateAround(rotationPoint, transform.forward, -playerInput.x * rotationSpeed * rotationSpeedMultiplier * Time.deltaTime);
    }

    /*private void DriftMovement()
    {
        if (Mathf.Abs(playerInput.y) > 0.01f)
        {
            Vector3 currentVelocity = rb.linearVelocity;
            float speed = currentVelocity.magnitude;
            Vector3 desiredDirection = transform.up * Mathf.Sign(playerInput.y);

            // Calculate the max angle to rotate this frame (in radians)
            float maxRadiansDelta = Mathf.Deg2Rad * driftTurnStrength; // Adjust 5f for drift responsiveness

            // Rotate velocity direction toward desired direction
            Vector3 newDirection = Vector3.RotateTowards(
                currentVelocity.normalized,
                desiredDirection,
                maxRadiansDelta,
                0f
            );

            rb.linearVelocity = newDirection * speed;
        }
    }*/
    

}
