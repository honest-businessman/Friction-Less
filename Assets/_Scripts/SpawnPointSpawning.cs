using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using Pathfinding;
using System.Collections;

public class SpawnPointSpawning : MonoBehaviour
{
    [SerializeField]
    private float spawnPointCooldownInSeconds = 5f;

    private List<GameObject> spawnPoints;
    private Dictionary<GameObject, Coroutine> spawnPointCoroutines = new Dictionary<GameObject, Coroutine>();

    public GameObject SpawnAtRandomSpawnPoint(GameObject enemyPrefab)
    {
        List<GameObject> activeSpawnPoints = spawnPoints.FindAll(sp => sp.activeInHierarchy);
        GameObject spawnPoint = activeSpawnPoints[UnityEngine.Random.Range(0, activeSpawnPoints.Count)];
        Vector3 spawnPosition = spawnPoint.transform.position;
        spawnPosition.z = 0; // Ensure z is 0 for 2D game
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        
        SetupEnemy(enemy);
        spawnPoint.SetActive(false);
        ManagePointPostSpawn(spawnPoint);
        return enemy;
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

    private void ManagePointPostSpawn(GameObject spawnPoint)
    {
        if (spawnPointCoroutines.TryGetValue(spawnPoint, out Coroutine existing))
        {
            StopCoroutine(existing);
        }
        spawnPointCoroutines[spawnPoint] = StartCoroutine(SetActiveAfterDelay(spawnPoint));
    }
    private IEnumerator SetActiveAfterDelay(GameObject spawnPoint)
    {
        yield return new WaitForSeconds(spawnPointCooldownInSeconds);
        spawnPoint.SetActive(true);
        spawnPointCoroutines.Remove(spawnPoint);
        Debug.Log($"Point {spawnPoint.name} Reactivated");
    }

    public void ResetSpawnPoints()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint Enemy").ToList();
        foreach (GameObject sp in spawnPoints)
            sp.SetActive(true);

        foreach (Coroutine c in spawnPointCoroutines.Values)
            StopCoroutine(c);
        spawnPointCoroutines.Clear();
    }
}
