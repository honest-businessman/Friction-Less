using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int health = 2;
    public int maxHealth = 2;
    public bool vulnerable = true;
    public delegate void DieAction();
    public static event DieAction OnDie;
    [SerializeField] GameObject explosionEffectPrefab;
    [SerializeField] AudioClip explosionSoundEnemy;
    [SerializeField] AudioClip explosionSoundPlayer;
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
        if (Input.GetKeyDown(KeyCode.Minus))
            TakeDamage(1);
        if (Input.GetKeyDown(KeyCode.Equals))
            GainHealth(1);
    }

    public void TakeDamage(int damage)
    {
        if (vulnerable)
        {
            Debug.Log($"{gameObject.name} taken {damage} damage!");
            health -= damage;

            if(health <= 0)
            {
                health = 0;
                Die();
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

        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }

        AudioClip clipToPlay = gameObject.CompareTag("Player") ? explosionSoundPlayer : explosionSoundEnemy;

        if(clipToPlay != null)
        {
            AudioSource.PlayClipAtPoint(clipToPlay,transform.position);
        }
        Destroy(gameObject);
    }
}
