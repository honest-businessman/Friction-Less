using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemy;
    private int frameCount = 0;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        frameCount++;

        if(frameCount % 200 == 0)
        {
            int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if(currentEnemyCount < 10)
            {
                Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
                Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

                SpriteRenderer sr = enemy.GetComponent<SpriteRenderer>();
                float enemyWidth = sr.bounds.size.x;
                float enemyHeight = sr.bounds.size.y;

                float minX = bottomLeft.x + enemyWidth / 2f;
                float maxX = topRight.x - enemyWidth / 2f;
                float minY = bottomLeft.y + enemyHeight / 2f;
                float maxY = topRight.y - enemyHeight / 2f;

                float x = Random.Range(minX, maxX);
                float y = Random.Range(minY, maxY);

                Vector3 spawnPosition = new Vector3(x, y, 0);
                Instantiate(enemy, spawnPosition, Quaternion.identity);

            }
            
        }
    }
}
