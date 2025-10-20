using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float health = 2;
    public float maxHealth = 2;
    public bool vulnerable = true;
    public delegate void DieAction();
    public event DieAction OnDie;

    [SerializeField] AudioClip DamageSound;
    [SerializeField] AudioClip DeathSound;
    [SerializeField] GameObject DeathVFX;
    [SerializeField] private XPContainer xpContainer;
    [SerializeField] private bool dropXP = false;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        if (vulnerable)
        {
            Debug.Log($"{gameObject.name} taken {damage} damage!");
            health -= damage;

            if (health <= 0)
            {
                health = 0;
                Die();
            }
            else
            {
                if(gameObject.CompareTag("Player") && DamageSound != null)
                {
                    audioSource.PlayOneShot(DamageSound);
                }
            }
        }
    }
    public void GainHealth(int gainedHealth)
    {
        health += gainedHealth;
        if (health >= maxHealth)
            health = maxHealth;
    }

    private void Die()
    {
        OnDie?.Invoke();

        if (DeathVFX != null)
        {
            GameObject explosion = Instantiate(DeathVFX, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }

        AudioClip clipToPlay = DeathSound;

        if(clipToPlay != null)
        {
            AudioSource.PlayClipAtPoint(clipToPlay,transform.position);
        }

        if (!CompareTag("Player"))
        {
            
            if (xpContainer != null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    xpContainer.SpawnXP(transform.position, player.transform);
                }
            }
        }

        Destroy(gameObject);
    }
}
