using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public GameObject canonball;
    public Transform firePoint;
    public float fireInterval = 2f;

    private Transform player;
    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player").transform;
        timer = fireInterval;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Shoot();
            timer = fireInterval;
        }
    }

    void Shoot()
    {
        if (player == null) return;
        Vector2 dir = player.position - transform.position;
        GameObject bullet = Instantiate(canonball,firePoint.position, Quaternion.identity);
        bullet.GetComponent<Canonball>().SetDirection(dir);
    }
}
