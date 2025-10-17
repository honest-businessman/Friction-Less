using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using Pathfinding;

public class SpawnPointSpawning : MonoBehaviour
{
    [SerializeField]
    private float spawnPointCooldownInSeconds = 5f;

    private List<GameObject> spawnPoints;

    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint Enemy").ToList();
    }

    public void SpawnAtRandomSpawnPoint(GameObject enemyPrefab)
    {
        List<GameObject> activeSpawnPoints = spawnPoints.FindAll(sp => sp.activeInHierarchy);
        GameObject spawnPoint = activeSpawnPoints[UnityEngine.Random.Range(0, activeSpawnPoints.Count)];
        Vector3 spawnPosition = spawnPoint.transform.position;
        spawnPosition.z = 0; // Ensure z is 0 for 2D game
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        
        SetupEnemy(enemy);
        spawnPoint.SetActive(false);
        ManagePointPostSpawn(spawnPoint);
    }

    void SetupEnemy(GameObject enemy)
    {
        enemy.transform.parent = transform;
        // Applies AiManager's WallErosionPenalty to the spawned enemy's Seeker component
        Seeker seeker = enemy.GetComponent<Seeker>();
        int[] penalties = new int[32];
        penalties[1] = AIManager.Instance.WallErosionPenalty; // Tag 1 is "Wall Erosion" under the pathfinding settings
        seeker.tagPenalties = penalties;
        Debug.Log(seeker.tagPenalties);
        // Applies AiManager's PathClaimPenalty to the spawned enemy's AlternativePath component
        AlternativePath altPath = enemy.AddComponent<AlternativePath>();
        altPath.penalty = AIManager.Instance.PathClaimPenalty;
        altPath.randomStep = 1;

        /*SimpleSmoothModifier ssm = enemy.AddComponent<SimpleSmoothModifier>();
        ssm.smoothType = SimpleSmoothModifier.SmoothType.Simple;
        ssm.iterations = 2;
        ssm.strength = 0.5f;
        ssm.uniformLength = true;*/
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
