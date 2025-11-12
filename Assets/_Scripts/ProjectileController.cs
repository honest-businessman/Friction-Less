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
        if (other.TryGetComponent(out FactionController otherFC))
        {
            if (fc.IsSameFaction(otherFC))
            {
                return;
            }

            if (other.TryGetComponent(out HealthSystem otherHS))
            {
                otherHS.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out FactionController otherFC))
        {
            if (!fc.IsSameFaction(otherFC))
            {
                HandleTrigger(collision.collider);
            }
        }
        if (bounceCount >= maxBounces)
        {
            Destroy(gameObject);
            return;
        }

        if (Time.time - lastBounceTime < bounceCooldown)
        {
            return;
        }

        float speed = rb.linearVelocity.magnitude;
        Vector2 inDirection = rb.linearVelocity.normalized;
        Vector2 normal = collision.contacts[0].normal;
        Vector2 reflected = Vector2.Reflect(inDirection, normal);


        float angleDiff = Vector2.Angle(lastDirection, reflected);

        rb.linearVelocity = reflected * speed;
        lastDirection = reflected;
        lastBounceTime = Time.time;

        bounceCount++;

        if (impactFX != null)
        {
            Instantiate(impactFX, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
        }
    }

}
