using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    public bool vulnerable = true;
    public int health = 3;
    public int maxHealth = 3;
    public bool regenerateHealth = false;
    public int regenAmount = 3;
    public float regenDelay = 5f;
    public event Action<int, int> OnDamageTaken;
    public event Action OnDie;
    public UnityEvent OnRegenStart;
    public UnityEvent OnRegenFinish;

    [SerializeField] AudioClip DamageSound;
    [SerializeField] AudioClip DeathSound;
    [SerializeField] GameObject DeathVFX;
    [SerializeField] private XPContainer xpContainer;
    [SerializeField] private bool dropXP = false;

    private AudioSource audioSource;
    private Coroutine regenCoroutine;

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

    public void TakeDamage(int damage)
    {
        if (vulnerable)
        {
            Debug.Log($"{gameObject.name} taken {damage} damage!");
            health -= damage;
            OnDamageTaken?.Invoke(health, maxHealth);
            
            if (regenCoroutine != null) { StopCoroutine(regenCoroutine);  }

            if (health <= 0)
            {
                health = 0;
                Die();
            }
            else
            {
                if (regenerateHealth) 
                {
                    regenCoroutine = StartCoroutine(StartRegenerateDelay()); 
                    OnRegenStart?.Invoke();
                }

                //Trigger glitch effect only for the player
                if (gameObject.CompareTag("Player"))
                {
                    // Play damage sound if available
                    if (DamageSound != null)
                    {
                        audioSource.PlayOneShot(DamageSound);
                    }

                    // Trigger glitch visual
                    GlitchFlash glitch = GetComponent<GlitchFlash>();
                    if (glitch != null)
                        glitch.TriggerGlitch();
                }
            }
        }
    }

    public void GainHealth(int gainedHealth)
    {
        health = Mathf.Min(health + gainedHealth, maxHealth);
    }
    public void GainMaxHealth()
    {
        health = maxHealth;
    }

    private void Die()
    {
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

    private IEnumerator StartRegenerateDelay()
    {
        Debug.Log($"{gameObject.name} is regenerating {regenAmount} health in {regenDelay} seconds!");
        yield return new WaitForSeconds(regenDelay);
        
        regenCoroutine = null;
        GainHealth(regenAmount);
        OnRegenFinish?.Invoke();
        Debug.Log($"{gameObject.name} is regenerated {regenAmount} health!");
    }

    private void OnDestroy()
    {
        OnDie?.Invoke();
    }
}
