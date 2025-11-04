using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using System.Collections; // ✅ Added for coroutine

public class PlayerController : CharacterBase
{
    [SerializeField] private ParticleSystem fullChargeParticles;
    private bool particlesPlaying = false;

    private CharacterController controller;
    public bool mouseAiming = false;
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
    public float minChargeMoveSpeed = 3f; // degrees
    private float turnSpeedMultiplier = 10f; // Adjusted multiplier for rotation speed

    // Reference to the TrackRight Animator
    [SerializeField] private Animator trackRightAnimator;
    [SerializeField] private Animator trackLeftAnimator;

    // ✅ Added Trail support
    [SerializeField] private TrailRenderer trailLeft;
    [SerializeField] private TrailRenderer trailRight;
    [SerializeField] private float trailFadeOutTime = 0.5f;
    private Coroutine fadeRoutine;

    [HideInInspector] public bool driftPressed;

    [SerializeField]
    private List<TurretSettings> equippedTurrets;
    private int turretIndex = 0;
    public GameObject turret;
    private TurretController turretController;

    public float pickupRadius = 1.5f; //distance of XP starting homing


    private enum MoveState
    {
        Idle,
        Moving,
        Drifting
    }
    private MoveState currentState = MoveState.Idle;
    private Vector2 playerInput;
    private Rigidbody2D rb;
    private FiringSystem firingSystem;
    private float timeLastCharge;
    private Vector2 aimInput;

    private void Start()
    {
        // Ensure trails are hidden when the game starts
        if (trailLeft != null) trailLeft.emitting = false;
        if (trailRight != null) trailRight.emitting = false;
    }


    void Awake()
    {
        // Find TrackRight and TrackLeft children by name and get their Animators
        if (trackRightAnimator == null)
        {
            Transform trackRightTransform = transform.Find("TrackRight");
            if (trackRightTransform != null)
                trackRightAnimator = trackRightTransform.GetComponent<Animator>();
        }

        if (trackLeftAnimator == null)
        {
            Transform trackLeftTransform = transform.Find("TrackLeft");
            if (trackLeftTransform != null)
                trackLeftAnimator = trackLeftTransform.GetComponent<Animator>();
        }

        DriveCharge = 0f;
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody2D>();
        firingSystem = GetComponent<FiringSystem>();
        defaultDrag = rb.linearDamping;

        firingSystem = GetComponent<FiringSystem>();
        if (turret != null)
        {
            turretController = turret.GetComponent<TurretController>();
        }
    }

    public void Move(Vector2 moveVector)
    {
        playerInput = moveVector;

    }

    // Modified to include trail control
    public void Drift(bool isPressed)
    {
        driftPressed = isPressed;
        HandleDriftTrails(isPressed); // Added
    }

    public void Aim(Vector2 aimVector)
    {
        aimInput = aimVector;
    }

    private void ChangeMoveState()
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
        HandleChargeParticles();
    }

    private void PlayerMovement()
    {
        ChangeMoveState();

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

        bool isMoving = (currentState == MoveState.Moving);

        if (trackRightAnimator != null)
            trackRightAnimator.SetBool("isMoving", isMoving);

        if (trackLeftAnimator != null)
            trackLeftAnimator.SetBool("isMoving", isMoving);
    }

    //particle effects for full charge
    private void HandleChargeParticles()
    {
        if (DriveCharge >= 100f && !particlesPlaying)
        {
            fullChargeParticles.Play();
            particlesPlaying = true;
        }
        else if (DriveCharge < 100f && particlesPlaying)
        {
            fullChargeParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // instantly clear particles
            particlesPlaying = false;
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
                    DriveCharge += (chargePerSecond * Time.deltaTime) * (mag / maxSpeed) * Mathf.Abs(playerInput.y);
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

    public void ChangeWeapon(float changeInput)
    {
        if (equippedTurrets.Count <= 1) { return; }

        if (changeInput > 0)
        {
            Debug.Log("Next Turret!");
            // Uses modulo operator to get remainder after division. This is to wrap the index within the count.
            turretIndex = (turretIndex + 1) % equippedTurrets.Count;
        }
        else if (changeInput < 0)
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

    public void Fire()
    {
        firingSystem.FireCommand();
    }

    //upgrade functions
    public void UpgradeSpeed(float multiplier)
    {
        moveSpeed *= multiplier;
        Debug.Log($"Speed upgraded! New speed: {moveSpeed}");
    }

    public void UpgradeProjectileSpeed(float multiplier)
    {
        firingSystem.UpgradeTrailSpeed(multiplier);
        if (turretController != null && turretController.settings != null)
        {
            turretController.settings.shellSpeed *= multiplier;
            Debug.Log($"Projectile speed upgraded! New shell speed: {turretController.settings.shellSpeed}");
        }
        else
        {
            Debug.LogWarning("turretController or settings is null!");
        }
    }
    public void UpgradeFireRate(float multiplier)
    {
        firingSystem.UpgradeFireRate(multiplier);
        Debug.Log($"Fire rate upgraded! New fire rate: {firingSystem.Settings.fireRate}");
    }

    // ✅ Added trail fade handling
    private void HandleDriftTrails(bool drifting)
    {
        if (drifting)
        {
            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);

            trailLeft.emitting = true;
            trailRight.emitting = true;
        }
        else
        {
            fadeRoutine = StartCoroutine(FadeOutTrails());
        }
    }

    private IEnumerator FadeOutTrails()
    {
        float startTime = Time.time;
        float initialTimeLeft = trailLeft.time;
        float initialTimeRight = trailRight.time;

        while (Time.time < startTime + trailFadeOutTime)
        {
            float t = 1 - ((Time.time - startTime) / trailFadeOutTime);
            trailLeft.time = Mathf.Lerp(0, initialTimeLeft, t);
            trailRight.time = Mathf.Lerp(0, initialTimeRight, t);
            yield return null;
        }

        trailLeft.time = initialTimeLeft;
        trailRight.time = initialTimeRight;
        trailLeft.emitting = false;
        trailRight.emitting = false;
    }
}
