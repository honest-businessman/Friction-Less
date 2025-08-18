using UnityEditor;
using UnityEngine;

public class ShellController : MonoBehaviour
{
    [SerializeField]
    private int shellBounces = 0;
    [SerializeField]
    private float shellLifetime = 10f;
    [SerializeField]
    private int shellDamage = 1;

    private Vector2 direction;
    [SerializeField]
    private Rigidbody2D rb;
    private FactionController fc;

    public void Initialize(int shellBounces, int shellDamage, float shellLifetime, FactionController.Factions faction)
    {
        this.shellBounces = shellBounces;
        this.shellDamage = shellDamage;
        this.shellLifetime = shellLifetime;
        fc.SetFaction(faction);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        fc = GetComponent<FactionController>();
    }
    private void Start()
    {
        Destroy(gameObject, shellLifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.TryGetComponent<FactionController>(out FactionController otherFC))
        {
            if (fc.IsSameFaction(otherFC)) { return; }
            if (other.gameObject.TryGetComponent<HealthSystem>(out HealthSystem healthSystem))
            {
                healthSystem.TakeDamage(shellDamage);
            }
            else if (other.gameObject.CompareTag("Projectile"))
            {
                Destroy(other.gameObject);
            }
        }
        Destroy(gameObject);
    }

}
