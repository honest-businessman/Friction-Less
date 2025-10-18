using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class WaveSpawning : MonoBehaviour
{
    public List<Enemy> enemies = new List<Enemy>();
    public List<GameObject> enemiesToSpawn = new List<GameObject>();
    public int currentWave = 0;
    public int waveBudget;

    SpawnPointSpawning sps;
    public float waveDuration;
    private float waveTimer;
    public float spawningDuration;
    private float spawningTimer;
    private float spawnInterval;
    private float spawnTimer;

    private void Awake()
    {
        currentWave = 0;
        if (!TryGetComponent(out sps))
        {
            sps = gameObject.AddComponent<SpawnPointSpawning>();
        }
        if(spawningDuration > waveDuration)
        {
            Debug.Log("WARNING: Spawning duration larger than wave duration, equalling to prevent errors");
            spawningDuration = waveDuration;
        }
    }

    private void Update()
    {
        // To Do: Implement starting of waves and spawning of enemies
    }

    public void GenerateWave()
    {
        waveBudget = currentWave * 10;
        GenerateEnemies();

        // Sets a fixed, equal time inteval for each generated enemy to spawn within the spawning duration.
        spawnInterval = spawningDuration / enemiesToSpawn.Count;
        waveTimer = waveDuration;
        spawnTimer = spawningDuration;
    }
    private void GenerateEnemies()
    {
        List<GameObject> generatedEnemies = new List<GameObject>();
        while (waveBudget > 0)
        {
            int randEnemyID = Random.Range(0, enemies.Count);
            int randEnemyCost = enemies[randEnemyID].cost;

            if(waveBudget - randEnemyCost >= 0 && currentWave >= enemies[randEnemyID].minimumWave)
            {
                generatedEnemies.Add(enemies[randEnemyID].enemyprefab);
                waveBudget -= randEnemyCost;
            }
        }
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }

    void Spawn(Enemy enemy)
    {
        sps.SpawnAtRandomSpawnPoint(enemy.enemyprefab);
    }
}

[System.Serializable]
public class Enemy
{
    public GameObject enemyprefab;
    public int cost;
    public int minimumWave;
}
