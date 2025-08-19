using Pathfinding;
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
    GameObject hiscanTrailPrefab;

    private float firingTimer = 0f;
    float trailSpeed = 0.01f;
    private bool canCharge = false;
    private GameObject turret;
    private TurretParameters turretParameters;
    private Vector2 firePosition;
    private Rigidbody2D rb;
    private FactionController fc;
    private PlayerController pc;

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
            if(hiscanTrailPrefab == null) { Debug.Log("No Hitscan Prefab found."); }
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
                if(pc.driveCharge >= 1f) 
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
        shell.GetComponent<ShellController>().Initialize(turretParameters.shellBounces, turretParameters.shellDamage, turretParameters.shellLifetime, fc.Faction);
        shell.transform.localScale = new Vector2(turretParameters.shellSize, turretParameters.shellSize);
        shell.GetComponent<Rigidbody2D>().linearVelocity = shell.transform.up * turretParameters.shellSpeed;
    }
    void FireCharged()
    {
        HealthSystem otherHealthSystem;
        RaycastHit2D hit = Physics2D.Raycast(firePosition, turret.transform.up, turretParameters.chargedRange, chargedHitList);
        Debug.Log($"Hit {hit.transform.gameObject}");
        GameObject trail = Instantiate(hiscanTrailPrefab, firePosition, turret.transform.rotation);
        TrailRenderer tr = trail.GetComponent<TrailRenderer>();
        if (hit.collider == null || hit == null)
        {
            StartCoroutine(MoveTrail(tr, firePosition + (Vector2)turret.transform.up * turretParameters.chargedRange));
        }
        else
        {
            StartCoroutine(MoveTrail(tr, hit.point));
        }
        Destroy(trail, tr.time);
        if (hit.transform.gameObject.TryGetComponent<HealthSystem>(out otherHealthSystem))
        {
            otherHealthSystem.TakeDamage(turretParameters.chargedDamage);
        }

    }

    System.Collections.IEnumerator MoveTrail(TrailRenderer tr, Vector3 targetPosition)
    {
        float timer = 0f;
        Vector3 startPosition = tr.transform.position;

        while (timer < trailSpeed)
        {
            tr.transform.position = Vector3.Lerp(startPosition, targetPosition, timer / trailSpeed);
            timer += Time.deltaTime;
            yield return null;
        }
        tr.transform.position = targetPosition;
    }

}
