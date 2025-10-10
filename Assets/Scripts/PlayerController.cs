using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : CharacterBase
{
    private CharacterController controller;
    public bool mouseAiming = false;
    [SerializeField]
    private InputHandler inputHandler;
    public float moveSpeed = 25f; // Movement speed
    public float maxSpeed = 5f; // Maximum speed the player can reach
    public float driftSpeed = 5f;
    public float defaultDrag = 8f;
    public float turnSpeed = 20f; // Rotation speed
    public float driftTurnSpeed = 20f;
    private float moveSpeedMultiplier = 100f; // Adjusted multiplier for movement speed
    public float treadOffset = 0.2f; // Offset for the rotation pivot point
    public float chargePerSecond = 60f;
    public float dischargePerSecond = 100f;
    public float chargeFadeDelay = 1f; // Delay before charge starts fading
    public float chargeAngle = 45f; // degrees
    public float minChargeMoveSpeed= 3f; // degrees
    private float turnSpeedMultiplier = 10f; // Adjusted multiplier for rotation speed
    
    [HideInInspector] public bool driftPressed;

    [SerializeField]
    private List<TurretSettings> equippedTurrets;
    private int turretIndex = 0;
    public GameObject turret;
    private TurretController turretController;


    private enum MoveState
    {
        Idle,
        Moving,
        Drifting
    }
    private MoveState currentState = MoveState.Idle;
    private Vector2 playerInput;
    private Rigidbody2D rb;
    private float timeLastCharge;
    private Vector2 aimInput;

    void Awake()
    {
        DriveCharge = 0f;
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody2D>();
        defaultDrag = rb.linearDamping;
    }

    private void OnEnable()
    {
        inputHandler.OnInputMove.AddListener(ProcessMove);
        inputHandler.OnInputDrift.AddListener(ProcessDrift);
        inputHandler.OnInputChangeWeapon.AddListener(ChangeTurret);
    }
    private void OnDisable()
    {
        inputHandler.OnInputMove.RemoveListener(ProcessMove);
        inputHandler.OnInputDrift.RemoveListener(ProcessDrift);
        inputHandler.OnInputChangeWeapon.RemoveListener(ChangeTurret);
    }

    private void ProcessMove(InputValue value)
    {
        playerInput = value.Get<Vector2>();
        
    }

    private void ProcessDrift(InputValue value)
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
    private void Update()
    {
        TurretRotate();
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
            rb.AddForce(transform.up * playerInput.y * (1.5f - Mathf.Abs(playerInput.x)) * moveSpeed * moveSpeedMultiplier * Time.deltaTime, ForceMode2D.Force);
            FadeDrive();
        }
        else if (currentState == MoveState.Drifting)
        {
            rb.AddForce(transform.up * playerInput.y * driftSpeed * moveSpeedMultiplier * Time.deltaTime, ForceMode2D.Force);
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
        float rotateSpeed;
        if (currentState == MoveState.Drifting)
            rotateSpeed = driftTurnSpeed;
        else
            rotateSpeed = turnSpeed;

        Vector2 rotationPoint = transform.position; // Point to rotate around
        Vector2 debugPoint = transform.position; // Point to draw debug line to
        if (Mathf.Abs(playerInput.y) > 0.1)
        {
            if (playerInput.x < 0)
            {
                rotationPoint += (Vector2)(-transform.right * treadOffset);
            }
            else if (playerInput.x > 0)
            {
                rotationPoint += (Vector2)transform.right * treadOffset;
            }
        }
        transform.RotateAround(rotationPoint, transform.forward, -playerInput.x * rotateSpeed * turnSpeedMultiplier * Time.deltaTime);
        rb.angularVelocity = 0f;
    }

    private void ChargeDrive()
    {
        
        Vector2 velocity = rb.linearVelocity;
        float mag = velocity.magnitude;
        Vector2 up = transform.up;

        if (mag > minChargeMoveSpeed)
        {
            float fAngle = Vector2.Angle(up, velocity);
            float bAngle = Vector2.Angle(-up, velocity);
            
            Debug.DrawRay(transform.position, velocity * 2, Color.red);
            if (fAngle > chargeAngle && bAngle > chargeAngle)
            {
                if (DriveCharge >= 100f)
                    DriveCharge = 100f; // Cap the charge at 100%
                else
                    DriveCharge += (chargePerSecond * Time.deltaTime) * (mag/maxSpeed) * Mathf.Abs(playerInput.y);
                timeLastCharge = Time.time; // Reset the charge fade timer
            }
        }
    }

    private void FadeDrive()
    {
        if (Time.time - timeLastCharge > chargeFadeDelay && DriveCharge > 0f)
            DriveCharge -= (dischargePerSecond * Time.deltaTime) * Mathf.Clamp01((Time.time - timeLastCharge) / chargeFadeDelay);
        else
            DriveCharge = Mathf.Max(DriveCharge, 0f); // Ensure charge doesn't go below 0
    }

    void OnAim(InputValue value)
    {
        aimInput = value.Get<Vector2>();
    }

    void TurretRotate()
    {
        Vector2 direction;
        if (mouseAiming)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = mousePos - (Vector2)transform.position;
        }
        else
        {
            if (aimInput.magnitude < 0.1f)
                direction = transform.up;
            else
                direction = new Vector2(aimInput.x, aimInput.y);
        }
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        turret.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle - 90));
    }

    void ChangeTurret(InputValue value)
    {
        if(equippedTurrets.Count <= 1) { return; }

        float input = value.Get<float>();
        if(input > 0)
        {
            Debug.Log("Next Turret!");
            // Uses modulo operator to get remainder after division. This is to wrap the index within the count.
            turretIndex = (turretIndex + 1) % equippedTurrets.Count; 
        }
        else if(input < 0)
        {
            Debug.Log("Previous Turret!");
            // Adds count to index to prevent negative index result when wrapping with modulo.
            turretIndex = (turretIndex - 1 + equippedTurrets.Count) % equippedTurrets.Count;
        }

        TurretSettings newTurretSettings = equippedTurrets[turretIndex];
        setNewTurret(newTurretSettings);
    }

    private void setNewTurret(TurretSettings newSettings)
    {
        SpriteRenderer sprender = turret.GetComponent<SpriteRenderer>();
        sprender.sprite = newSettings.sprite;
        sprender.color = newSettings.spriteColor;
        TurretController tController = turret.GetComponent<TurretController>();
        tController.settings = newSettings;
    }
}
