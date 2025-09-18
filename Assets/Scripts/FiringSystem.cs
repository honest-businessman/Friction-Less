using Pathfinding;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class FiringSystem : MonoBehaviour
{
    [SerializeField]
    private InputHandler inputHandler;
    [SerializeField]
    LayerMask chargedHitList;
    [SerializeField]
    GameObject hitscanTrailPrefab;
    [SerializeField]
    private float trailSpeed = 300f;
    [SerializeField]
    private float bounceDelay = 0.5f;

    private float firingTimer = 0f;
    private bool canCharge = false;
    private GameObject turret;
    private TurretParameters turretParameters;
    private Vector2 firePosition;
    private Rigidbody2D rb;
    private FactionController fc;
    private PlayerController pc;
    public delegate void ImpactAction();
    public static event ImpactAction OnImpact;

    private void Awake()
    {
        foreach (Transform childTransform in transform)
        {
            turret = FindTagInChildren("Turret").gameObject; // Return the GameObject if tag matches
            turretParameters = turret.GetComponent<TurretParameters>();
            
        }
        if (turret == null)
        {
            Debug.LogError("No Turret found on " + gameObject);
        }
        if (gameObject.CompareTag("Player")) 
        {
            canCharge = true;
            pc = GetComponent<PlayerController>();
            if(hitscanTrailPrefab == null) { Debug.Log("No Hitscan Prefab found."); }
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
            if (canCharge)
            {
                if(pc.driveCharge >= 100f) 
                {
                    Debug.Log("Fire Charged!");
                    FireCharged();
                    pc.DrainDrive();
                    return;
                }
            }
            FireNormal();
        }
    }

    void FireNormal()
    {
        GameObject shell = Instantiate(turretParameters.shellPrefab, firePosition, turret.transform.rotation);
        shell.GetComponent<ProjectileController>().Initialize(turretParameters.shellBounces, turretParameters.shellDamage, turretParameters.shellLifetime, fc.Faction);
        shell.transform.localScale = new Vector2(turretParameters.shellSize, turretParameters.shellSize);
        shell.GetComponent<Rigidbody2D>().linearVelocity = shell.transform.up * turretParameters.shellSpeed;
    }
    void FireCharged()
    {
        HealthSystem otherHealthSystem;

        // Visualize the bounce ray in the editor
        Debug.DrawLine(firePosition, turret.transform.up * turretParameters.chargedRange, Color.red, 2f);
        RaycastHit2D hit = Physics2D.Raycast(firePosition, turret.transform.up, turretParameters.chargedRange, chargedHitList);
        GameObject trail = Instantiate(hitscanTrailPrefab, firePosition, turret.transform.rotation);
        TrailRenderer tr = trail.GetComponent<TrailRenderer>();
        if (hit.collider != null)
        {
            Debug.Log("Hit: " + hit.transform.name);
            StartCoroutine(SpawnTrail(tr, hit.point, hit.normal, 0, true));
            if (hit.transform.gameObject.TryGetComponent<HealthSystem>(out otherHealthSystem))
            {
                otherHealthSystem.TakeDamage(turretParameters.chargedDamage);
            }
        }
        else
        {
            StartCoroutine(SpawnTrail(tr, firePosition + (Vector2)turret.transform.up * turretParameters.chargedRange, Vector2.zero, 0, false));
        }
    }

    System.Collections.IEnumerator SpawnTrail(TrailRenderer tr, Vector3 targetPosition, Vector2 hitNormal, int currentBounces, bool impacted)
    {
        Vector3 startPosition = tr.transform.position;
        Vector3 direction = (targetPosition - startPosition).normalized; 
        
        float startingDistance = Vector3.Distance(startPosition, targetPosition);
        float distance = startingDistance;

        while (distance > 0)
        {
            tr.transform.position = Vector3.Lerp(startPosition, targetPosition, 1 - (distance / startingDistance));
            distance -= Time.deltaTime * trailSpeed;

            yield return null;
        }
        tr.transform.position = targetPosition;

        if (impacted)
        {
            OnImpact?.Invoke();
            yield return new WaitForSeconds(bounceDelay);

            int maxBounces = turretParameters.chargedBounces;
            if (maxBounces >= currentBounces)
            {
                Vector3 bounceDirection = Vector2.Reflect(direction, hitNormal);

                // Visualize the bounce ray in the editor
                Debug.DrawLine(targetPosition, targetPosition + bounceDirection * turretParameters.chargedRange, Color.red, 2f);
                Vector2 bounceOrigin = targetPosition + bounceDirection * 0.01f; // Small offset for following raycast
                RaycastHit2D hit = Physics2D.Raycast(bounceOrigin, bounceDirection, turretParameters.chargedRange, chargedHitList);
                
                // Calculate the angle from the bounce direction
                float angle = Mathf.Atan2(bounceDirection.y, bounceDirection.x) * Mathf.Rad2Deg;
                // Create a rotation that points the trail along the bounce direction
                Quaternion bounceRotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
                GameObject trail = Instantiate(hitscanTrailPrefab, targetPosition, bounceRotation);

                tr = trail.GetComponent<TrailRenderer>();
                if (hit.collider != null)
                {
                    HealthSystem otherHealthSystem;
                    Debug.Log("Hit: " + hit.transform.name);

                    yield return StartCoroutine(SpawnTrail(tr, hit.point, hit.normal, currentBounces + 1, true));
                    if (hit.transform.gameObject.TryGetComponent<HealthSystem>(out otherHealthSystem))
                    {
                        otherHealthSystem.TakeDamage(turretParameters.chargedDamage);
                    }
                }
                else
                {
                    yield return StartCoroutine(SpawnTrail(tr, targetPosition + (bounceDirection * turretParameters.chargedRange), Vector2.zero, currentBounces + 1, false));
                }
            }
        }
    }

}
