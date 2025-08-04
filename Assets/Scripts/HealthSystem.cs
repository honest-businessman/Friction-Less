using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int health = 2;
    public int maxHealth = 2;
    public bool vulnerable = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Minus))
            TakeDamage(1);
        if (Input.GetKey(KeyCode.Equals))
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

    }
}
