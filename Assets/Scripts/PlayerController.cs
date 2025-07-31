using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    public float moveSpeed = 25f; // Movement speed
    public float maxSpeed = 5f; // Maximum speed the player can reach
    public float driftSpeed = 5f;
    public float defaultDrag = 8f;
    public float rotationSpeed = 15f; // Rotation speed
    private float moveSpeedMultiplier = 100f; // Adjusted multiplier for movement speed
    public float treadOffset = 0.2f; // Offset for the rotation pivot point
    public float chargePerSecond = 60f;
    public float dischargePerSecond = 70f;
    public float chargeFadeDelay = 2f; // Delay before charge starts fading
    public float chargeAngle = 45f; // degrees
    public float minChargeMoveSpeed= 3f; // degrees
    private float rotationSpeedMultiplier = 10f; // Adjusted multiplier for rotation speed
    public float driveCharge = 0f;

    private enum MoveState
    {
        Idle,
        Moving,
        Drifting
    }
    private MoveState currentState = MoveState.Idle;
    private Vector2 playerInput;
    public bool driftPressed;
    private Rigidbody rb;
    private float timeLastCharge;

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
        driftPressed = value.isPressed;
    }
    private void ChageMoveState()
    {
        if (driftPressed)
            currentState = MoveState.Drifting;
        else if (playerInput.magnitude > 0.1f)
            currentState = MoveState.Moving;
        else
            currentState = MoveState.Idle;
    }

    void FixedUpdate()
    {
        if (currentState == MoveState.Drifting)
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
        ChageMoveState();

        Debug.DrawRay(transform.position, transform.up * 2, Color.green); // Draw ray pointing up from the player
        if (currentState == MoveState.Moving)
        {
            rb.AddForce(transform.up * playerInput.y * (1.5f - Mathf.Abs(playerInput.x)) * moveSpeed * moveSpeedMultiplier * Time.deltaTime, ForceMode.Force);
            FadeDrive();
        }
        else if (currentState == MoveState.Drifting)
        {
            rb.AddForce(transform.up * playerInput.y * driftSpeed * moveSpeedMultiplier * Time.deltaTime, ForceMode.Force);
            ChargeDrive();
        }
        else if (currentState == MoveState.Idle)
            FadeDrive();

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
            }
            else if (playerInput.x > 0)
            {
                rotationPoint += transform.right * treadOffset;
            }
        }
        transform.RotateAround(rotationPoint, transform.forward, -playerInput.x * rotationSpeed * rotationSpeedMultiplier * Time.deltaTime);
    }

    private void ChargeDrive()
    {
        
        Vector3 velocity = rb.linearVelocity;
        float mag = velocity.magnitude;
        Vector3 up = transform.up;

        if (mag > minChargeMoveSpeed)
        {
            float fAngle = Vector3.Angle(up, velocity);
            float bAngle = Vector3.Angle(-up, velocity);
            
            Debug.DrawRay(transform.position, velocity * 2, Color.red);
            if (fAngle > chargeAngle && bAngle > chargeAngle)
            {
                if (driveCharge >= 100f)
                    driveCharge = 100f; // Cap the charge at 100%
                else
                    driveCharge += (chargePerSecond * Time.deltaTime) * (mag/maxSpeed) * Mathf.Abs(playerInput.y);
                timeLastCharge = Time.time; // Reset the charge fade timer
            }
        }
    }

    private void FadeDrive()
    {
        if (Time.time - timeLastCharge > chargeFadeDelay && driveCharge > 0f)
            driveCharge -= (dischargePerSecond * Time.deltaTime) * Mathf.Clamp01((Time.time - timeLastCharge) / chargeFadeDelay);
        else
            driveCharge = Mathf.Max(driveCharge, 0f); // Ensure charge doesn't go below 0
    }

}
