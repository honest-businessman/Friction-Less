using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> enemyCharacters;

    public float spawnCooldownInSeconds = 3f;
    public float spawnPointCooldownInSeconds = 5f;

    private List<GameObject> spawnPoints;
    private float spawnTimer;


    void Start()
    {
        spawnTimer = Time.time;
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint Enemy").ToList();
    }
    void Update()
    {
        if(Time.time - spawnTimer >= spawnCooldownInSeconds)
        {
            spawnTimer = Time.time;
            // Find total number of enemies in the scene based on tag
            int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

            if (currentEnemyCount < 5)
            {
                // Selects a random enemy to spawn
                int rand = UnityEngine.Random.Range(0, enemyCharacters.Count); 
                SpawnAtRandomSpawnPoint(enemyCharacters[rand]);
            }
        }
    }

    void SpawnAtRandomSpawnPoint(GameObject enemyPrefab)
    {
        List<GameObject> activeSpawnPoints = spawnPoints.FindAll(sp => sp.activeInHierarchy);
        GameObject spawnPoint = activeSpawnPoints[UnityEngine.Random.Range(0, activeSpawnPoints.Count)];
        Vector3 spawnPosition = spawnPoint.transform.position;
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        spawnPoint.SetActive(false);
        ManagePointPostSpawn(spawnPoint);
    }

    private async void ManagePointPostSpawn(GameObject spawnPoint)
    {
        await SetActiveAfterDelay(spawnPoint);
    }
    async Task SetActiveAfterDelay(GameObject spawnPoint)
    {
        await Task.Delay((int)(spawnPointCooldownInSeconds * 1000)); // Times by 1000 to convert to milliseconds.
        spawnPoint.SetActive(true);
        Debug.Log($"Point {spawnPoint.name} Reactivated");
    }
}
