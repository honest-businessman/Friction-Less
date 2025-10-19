using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Pathfinding.Examples;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    public List<Enemy> enemies = new List<Enemy>();
    public float waveDuration = 20f;
    public float spawningDuration = 10f;

    [Header("Runtime State")]
    public int currentWave = 0;
    public int waveBudget;
    public List<GameObject> enemiesToSpawn = new List<GameObject>();
    public List<GameObject> activeEnemies = new List<GameObject>();

    [Header("Events")]
    public UnityEvent OnWaveStarted;
    public UnityEvent<GameObject> OnEnemySpawned;
    public UnityEvent OnWaveCompleted;

    private SpawnPointSpawning sps;
    private Coroutine waveRoutine;
    private bool waveActive;

    private void OnDisable()
    {
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);
    }

    private void Awake()
    {
        if (!TryGetComponent(out sps))
            sps = gameObject.AddComponent<SpawnPointSpawning>();
        sps.ResetSpawnPoints();

        if (spawningDuration > waveDuration)
        {
            Debug.LogWarning("Spawning duration larger than wave duration, clamping to prevent errors");
            spawningDuration = waveDuration;
        }
    }

    public void NextWave()
    {
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);

        currentWave++;
        waveRoutine = StartCoroutine(RunWave());
    }

    private IEnumerator RunWave()
    {
        Debug.Log("Generating Wave...");
        GenerateWave();
        OnWaveStarted?.Invoke();
        Debug.Log($"Starting Wave {currentWave} with budget {waveBudget} spawning {enemiesToSpawn.Count} enemies.");
        waveActive = true;

        float spawnInterval = enemiesToSpawn.Count > 0 ? (spawningDuration / enemiesToSpawn.Count) : 0f;
        float elapsed = 0f;

        // Spawn enemies gradually
        while (elapsed < spawningDuration && enemiesToSpawn.Count > 0)
        {
            GameObject enemyPrefab = enemiesToSpawn[0];
            GameObject enemySpawned = sps.SpawnAtRandomSpawnPoint(enemyPrefab);
            OnEnemySpawned?.Invoke(enemySpawned);
            activeEnemies.Add(enemySpawned);
            enemiesToSpawn.RemoveAt(0);

            if (enemySpawned.TryGetComponent(out HealthSystem health))
            {
                HealthSystem.DieAction handler = null;
                handler = () =>
                {
                    activeEnemies.Remove(enemySpawned);
                    health.OnDie -= handler; // unsubscribe immediately
                    if (activeEnemies.Count == 0 && enemiesToSpawn.Count == 0)
                        EndWaveEarly();
                };
                health.OnDie += handler;
            }

            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;
        }

        // Wait out remaining wave duration
        yield return new WaitForSeconds(waveDuration - spawningDuration);
        EndWave();
    }

    private void EndWave()
    {
        if (!waveActive) return;
        waveActive = false;

        if (waveRoutine != null)
        {
            StopCoroutine(waveRoutine);
            waveRoutine = null;
        }

        enemiesToSpawn.Clear();
        OnWaveCompleted?.Invoke();
    }

    private void EndWaveEarly()
    {
        EndWave();
        // Possibly add extra logic for early completion player rewards
    }

    private void GenerateWave()
    {
        waveBudget = currentWave * 5;
        enemiesToSpawn = GenerateEnemies();
    }

    private List<GameObject> GenerateEnemies()
    {
        List<GameObject> generated = new List<GameObject>();

        List<Enemy> eligibleEnemies = new List<Enemy>();
        int minCost = int.MaxValue;
        foreach (Enemy e in enemies)
        {
            if (currentWave >= e.minimumWave)
            {
                eligibleEnemies.Add(e);
                if (e.cost < minCost) { minCost = e.cost; }
            }
        }
        if (eligibleEnemies.Count == 0)
        {
            Debug.LogWarning("No eligible enemies for this wave!");
            return generated;
        }


        int safetyCounter = 0;
        while (waveBudget > minCost)
        {
            int randEnemyID = Random.Range(0, eligibleEnemies.Count);
            Enemy enemy = eligibleEnemies[randEnemyID];
            if (waveBudget - enemy.cost >= 0)
            {
                generated.Add(enemy.enemyprefab);
                waveBudget -= enemy.cost;
            }
            safetyCounter++;
            if (safetyCounter > 1000)
            {
                Debug.LogWarning("Wave generation safety counter triggered, breaking loop to prevent infinite loop.");
                break;
            }
        }
        return generated;
    }

    public void CleanWaves()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        activeEnemies.Clear();
        enemiesToSpawn.Clear();
        waveActive = false;
        currentWave = 0;
        waveBudget = 0;
        if (waveRoutine != null)
        {
            StopCoroutine(waveRoutine);
            waveRoutine = null;
        }

        sps.ResetSpawnPoints();
    }
}


[System.Serializable]
public class Enemy
{
    public GameObject enemyprefab;
    public int cost;
    public int minimumWave;
}
