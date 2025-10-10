using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using System.Threading.Tasks;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemy1;
    public GameObject enemy2;
    public GameObject enemy3;
    public int spawnPointCooldown = 5000;

    public List<GameObject> spawnPoints;

    private int frameCount = 0;
    

    void Start()
    {

    }

    void Update()
    {
        
        frameCount++;

        if(frameCount % 200 == 0)
        {
            int currentEnemy1Count = GameObject.FindGameObjectsWithTag("Enemy").Length;
            int currentEnemy2Count = GameObject.FindGameObjectsWithTag("Enemy2").Length;
            int currentEnemy3Count = GameObject.FindGameObjectsWithTag("Enemy3").Length;

            if (currentEnemy1Count < 2)
            {
                SpawnAtRandomSpawnPoint(enemy1);

            }

            if (currentEnemy2Count < 2)
            {
                SpawnAtRandomSpawnPoint(enemy2);
            }

            if(currentEnemy3Count < 2)
            {
                SpawnAtRandomSpawnPoint(enemy3);
            }

        }
    }

    void SpawnAtRandomSpawnPoint(GameObject enemyPrefab)
    {
        List<GameObject> activeSpawnPoints = spawnPoints.FindAll(sp => sp.activeInHierarchy);
        GameObject spawnPoint = activeSpawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
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
        await Task.Delay(spawnPointCooldown);
        spawnPoint.SetActive(true);
        Debug.Log($"Point {spawnPoint.name} Reactivated");
    }
}
