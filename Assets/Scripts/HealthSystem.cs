using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int health = 2;
    public int maxHealth = 2;
    public bool vulnerable = true;
    public delegate void DieAction();
    public static event DieAction OnDie;

    void Start()
    {
        
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
        Destroy(gameObject);
    }
}
