using Pathfinding;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;

public class FiringSystem : MonoBehaviour
{
    [SerializeField]
    LayerMask chargedHitList;
    [SerializeField]
    GameObject hitscanTrailPrefab;
    [SerializeField]
    private float trailSpeed = 300f;
    [SerializeField]
    private float bounceDelay = 0.5f;

    private float firingTimer = 0f;
    private bool isPlayer = false;
    private bool canCharge = false;
    private GameObject turret;
    private TurretController turretController;
    private TurretSettings settings;
    private Vector2 firePosition;
    private Rigidbody2D rb;
    private FactionController fc;
    private CharacterBase cb;
    public delegate void ImpactAction();
    public static event ImpactAction OnImpact;
    private TrailRenderer tr;

    private void Awake()
    {
        cb = GetComponent<CharacterBase>();
        if (hitscanTrailPrefab == null) { Debug.Log("No Hitscan Prefab found."); }

        foreach (Transform childTransform in transform) // To do: Check if for loop is nessecary
        {
            turret = FindTagInChildren("Turret").gameObject; // Return the turret child object if tag matches game object
        }
        if (turret == null)
        {
            Debug.LogError("No Turret found on " + gameObject);
        }
        if (gameObject.CompareTag("Player")) // checks if parent GameObject is the player object
        {
            isPlayer = true;
        }

        rb = GetComponent<Rigidbody2D>();
        fc = GetComponent<FactionController>();
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

    // For AI or non-input based firing
    public void FireCommand() { TryFire(); }

    // For player input system
    public void FireCommand(InputValue value) { TryFire(); }

    // Attempts to fire a shot if the fire rate cooldown has elapsed.
    // Will fire a charged shot if drive is fully charged.
    void TryFire()
    {
        // Get current turret parameters, need to implement only getting new parameters on weapon change
        turretController = turret.GetComponent<TurretController>();
        settings = turretController.settings;

        if (Time.time - firingTimer >= 1 / settings.fireRate) // Divide fire rate by 1 to convert fire rate to shells per second
        {
            firingTimer = Time.time;
            firePosition = turret.transform.position + (turret.transform.up * settings.spawnOffset);
            if (cb.DriveCharge >= 100f)
            {
                Debug.Log("Fire Charged!");
                FireCharged();
                cb.DrainDrive();
                return;
            }
            FireNormal();
        }
    }

    // Fires a normal shot
    public void FireNormal()
    {
        if (settings.isNormalHitscan)
            StartCoroutine(FireHitscan(firePosition, firePosition, Vector2.zero, 0, 0, 0));
        else
            FireShell();
    }

    // Fires a charged shot
    public void FireCharged()
    {
        if (settings.isChargedHitscan)
            StartCoroutine(FireHitscan(firePosition, firePosition, Vector2.zero, 0, 0, 0));
        else
            FireShell();
    }

    // Fires a physical projectile that can bounce.
    void FireShell()
    {
        GameObject shell = Instantiate(settings.shellPrefab, firePosition, turret.transform.rotation);
        shell.GetComponent<ProjectileController>().Initialize(settings.shellBounces, settings.shellDamage, settings.shellLifetime, fc.Faction);
        shell.transform.localScale = new Vector2(settings.shellSize, settings.shellSize);
        shell.GetComponent<Rigidbody2D>().linearVelocity = shell.transform.up * settings.shellSpeed;
    }

    // A recursive coroutine that handles hitscan firing, penetration, and bouncing.
    System.Collections.IEnumerator FireHitscan(Vector3 oldOrigin, Vector3 targetPosition, Vector2 hitNormal, int shotType, int currentBounces, int currentPens)
    {
        // Shot types: 0 - First shot, 1 - Penetration, 2 - Bounce
        Debug.Log("DEBUG1");
        Vector2 rayOrigin;
        Vector3 rayDirection;
        Quaternion trailRotation;
        int maxBounces = settings.hitscanBounces;

        if (shotType == 0) // Ready raycast for first shot
        {
            Debug.Log("FIRST SHOT");
            rayOrigin = firePosition;
            rayDirection = turret.transform.up;
            trailRotation = turret.transform.rotation;
            Vector2 debugRayEndpoint = rayOrigin + (Vector2)rayDirection * settings.hitscanRange;
            Debug.DrawLine(rayOrigin, debugRayEndpoint, Color.green, 3f);
        }
        else if (shotType == 1) // Ready next raycast for penetration
        {
            Debug.Log("PENETRATION");
            rayDirection = (targetPosition - oldOrigin).normalized; // Use previous ray direction, unsure how this works.
            rayOrigin = oldOrigin; // Start from previous hit point

            Vector2 debugRayEndpoint = rayOrigin + (Vector2)rayDirection * settings.hitscanRange;
            Debug.DrawLine(rayOrigin, debugRayEndpoint, Color.yellow, 3f);
            trailRotation = Quaternion.FromToRotation(Vector3.up, rayDirection); // Keep same rotation as previous trail, unsure how necessary this is
        }
        else // Ready next raycast for bounce
        {
            Debug.Log("BOUNCE");
            yield return new WaitForSeconds(bounceDelay);

            // Use previous hit point and normal to calculate bounce direction
            Vector2 incomingDirection = (targetPosition - oldOrigin).normalized;
            Vector2 bounceDirection = Vector2.Reflect(incomingDirection, hitNormal);

            // Offset origin slightly to avoid starting inside collider
            Vector2 bounceOrigin = (Vector2)targetPosition + bounceDirection * 0.01f;

            // Visualize the bounce ray in the editor (full range)
            Debug.DrawLine(bounceOrigin, bounceOrigin + bounceDirection * settings.hitscanRange, Color.red, 3f);

            rayOrigin = bounceOrigin;
            rayDirection = bounceDirection;

            // Calculate the angle from the bounce direction
            float angle = Mathf.Atan2(bounceDirection.y, bounceDirection.x) * Mathf.Rad2Deg;
            trailRotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        }

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, settings.hitscanRange, chargedHitList);


        if (hit.collider != null)
        {
            Debug.Log("Hit: " + hit.transform.name);

            // Trail spawn and movement
            GameObject trail = Instantiate(hitscanTrailPrefab, rayOrigin, trailRotation);
            tr = trail.GetComponent<TrailRenderer>();
            Vector3 startPosition = tr.transform.position;
            float startingDistance = Vector3.Distance(startPosition, hit.point);
            float distance = startingDistance;
            while (distance > 0)
            {
                tr.transform.position = Vector3.Lerp(startPosition, hit.point, 1 - (distance / startingDistance));
                distance -= Time.deltaTime * trailSpeed;

                yield return null;
            }
            tr.transform.position = hit.point;
            OnImpact?.Invoke(); // Should activate a particle effect through this event

            hit.transform.TryGetComponent(out FactionController hitFc);
            hit.transform.gameObject.TryGetComponent(out HealthSystem hitHealthSystem);
            if (currentPens < settings.hitscanPenetrations)
            {
                tryDamage();
                int pensSpent;
                if (hitFc.Faction == FactionController.Factions.Neutral)
                    pensSpent = 0;
                else
                    pensSpent = 1;

                Vector2 penetrationOrigin = (Vector2)hit.point + (Vector2)rayDirection * 0.01f;
                Vector2 penetrationEnd = penetrationOrigin + (Vector2)rayDirection * settings.hitscanRange;
                yield return StartCoroutine(FireHitscan(penetrationOrigin, penetrationEnd, hit.normal, pensSpent, currentBounces, currentPens + 1));
                yield break;
            }
            
            if (maxBounces > currentBounces) // Start new hitscan for bounce
            {
                tryDamage();
                Debug.Log("DEBUG4");
                yield return new WaitForSeconds(bounceDelay);
                yield return StartCoroutine(FireHitscan(rayOrigin, hit.point, hit.normal, 2, currentBounces + 1, currentPens));
            }

            void tryDamage()
            {
                if (hitFc != null && hitHealthSystem != null && !(fc.IsSameFaction(hitFc)))
                {
                    hitHealthSystem.TakeDamage(settings.hitscanDamage);
                }
            }
        }
    }
}
