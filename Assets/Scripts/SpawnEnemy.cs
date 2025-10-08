using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemy1;
    public GameObject enemy2;

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
            int currentEnemy1Count = GameObject.FindGameObjectsWithTag("Enemy").Length;
            int currentEnemy2Count = GameObject.FindGameObjectsWithTag("Enemy2").Length;

            if (currentEnemy1Count < 3)
            {
                SpawnAtCorner(enemy1);

            }

            if (currentEnemy2Count < 3)
            {
                SpawnAtCorner(enemy2);
            }

        }
    }

    void SpawnAtCorner(GameObject enemyPrefab)
    {
        Vector3 bottomLeft  = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        SpriteRenderer sr = enemyPrefab.GetComponent<SpriteRenderer>();
        float enemyWidth = sr.bounds.size.x;
        float enemyHeight = sr.bounds.size.y;

        float offsetX = enemyWidth / 2f;
        float offsetY = enemyHeight / 2f;

        Vector3[] corners = new Vector3[4];
        corners[0] = new Vector3(bottomLeft.x + offsetX, bottomLeft.y + offsetY, 0);
        corners[1] = new Vector3(topRight.x - offsetX, bottomLeft.y = offsetY, 0);
        corners[2] = new Vector3(bottomLeft.x + offsetX, topRight.y - offsetY,0);
        corners[3] = new Vector3(topRight.x - offsetX, topRight.y - offsetY, 0);

        Vector3 spawnPosition = corners[Random.Range(0, corners.Length)];
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
