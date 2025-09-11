using UnityEditor;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField]
    public GameObject impactFX;
    private int maxBounces;
    private int bounceCount = 0;
    [SerializeField]
    private float lifetime = 10f;
    [SerializeField]
    private int damage = 1;

    public float minBounceAngle = 1f;
    public float bounceCooldown = 0.01f;
    private float lastBounceTime = 0f;
    private Vector2 direction;
    private Vector2 lastDirection;
    private Rigidbody2D rb;
    private FactionController fc;

    public void Initialize(int maxBounces, int damage, float lifetime, FactionController.Factions faction)
    {
        this.maxBounces = maxBounces;
        this.damage = damage;
        this.lifetime = lifetime;
        fc.SetFaction(faction);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        fc = GetComponent<FactionController>();
        bounceCount = 0;
    }
    private void Start()
    {
        lastDirection = rb.linearVelocity.normalized;
        Destroy(gameObject, lifetime);
    }

    public void HandleTrigger(Collider2D other)
    {
        Debug.Log($"Triggered by: {other.gameObject.name}");

        if (other.TryGetComponent<FactionController>(out FactionController otherFC))
        {
            if (fc.IsSameFaction(otherFC))
            {
                Debug.Log("Same faction - ignoring.");
                return;
            }

            if (other.TryGetComponent<HealthSystem>(out HealthSystem otherHS))
            {
                Debug.Log("Damaging target.");
                otherHS.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Collided with: {collision.gameObject.name}");
        if (bounceCount >= maxBounces)
        {
            Debug.Log("Destroying projectile due to max bounces.");
            Destroy(gameObject);
            return;
        }

        if (Time.time - lastBounceTime < bounceCooldown)
        {
            Debug.Log("Bounce ignored due to time");
            return;
        }

        float speed = rb.linearVelocity.magnitude;
        Vector2 inDirection = rb.linearVelocity.normalized;
        Vector2 normal = collision.contacts[0].normal;
        Vector2 reflected = Vector2.Reflect(inDirection, normal);


        float angleDiff = Vector2.Angle(lastDirection, reflected);

        // If the angle change is too small, ignore the bounce (prevents parallel jitter)
        /*if (angleDiff < minBounceAngle)
        {
            Debug.Log("Bounce ignored due to angle");
            return;
        }*/

        rb.linearVelocity = reflected * speed;
        lastDirection = reflected;
        lastBounceTime = Time.time;

        bounceCount++;
        Debug.Log($"BOUNCES: {bounceCount}/{maxBounces}");

        if (impactFX != null)
        {
            Instantiate(impactFX, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
        }
    }

}
