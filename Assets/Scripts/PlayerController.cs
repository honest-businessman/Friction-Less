using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    public float pMoveSpeed = 25f; // Movement speed
    private float pMoveSpeedMultiplier = 100f; // Adjusted multiplier for movement speed
    public float pRotSpeed = 10f; // Rotation speed
    private float pRotSpeedMultiplier = 10f; // Adjusted multiplier for rotation speed
    public float treadOffset = 0.2f; // Offset for the rotation pivot point
    private float debugOffset = 2f; // Offset for the debug line length
    private Vector2 playerInput;
    private Vector3 velocity;
    private Rigidbody rb;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnMove(InputValue value)
    {
        playerInput = value.Get<Vector2>();
    }


    void FixedUpdate()
    {
        PlayerMovement();
    }
    private void PlayerMovement()
    {
        Debug.DrawRay(transform.position, transform.up * 2, Color.green); // Draw ray pointing up from the player
        rb.AddForce(transform.up * playerInput.y * (1 - Mathf.Abs(playerInput.x)) * pMoveSpeed * pMoveSpeedMultiplier * Time.deltaTime, ForceMode.Force);
        PlayerRotate();
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
        transform.RotateAround(rotationPoint, transform.forward, -playerInput.x * pRotSpeed * pRotSpeedMultiplier * Time.deltaTime);
    }

}
