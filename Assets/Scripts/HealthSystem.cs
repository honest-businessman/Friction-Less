using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int health = 2;
    public int maxHealth = 2;
    public bool vulnerable = true;
    public delegate void DieAction();
    public event DieAction OnDie;

    [SerializeField] AudioClip DamageSound;
    [SerializeField] AudioClip DeathSound;
    [SerializeField] GameObject DeathVFX;
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

    public void TakeDamage(int damage)
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
        Destroy(gameObject);
    }
}
