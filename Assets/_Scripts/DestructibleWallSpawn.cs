using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class DestructibleWallSpawn : MonoBehaviour
{
    [Header("Setup")]
    public GameObject DestructibleWall;   
    public int maxSpawnGroups = 5;        
    public float spawnInterval = 5f;      
    public float initialDelay = 10f;      

    private Tilemap tilemap;
    private HashSet<Vector3Int> allTiles = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> usedTiles = new HashSet<Vector3Int>();

    private void Start()
    {
        GameObject wallGen = GameObject.FindGameObjectWithTag("WallGen");
        if (wallGen == null)
        {
            Debug.LogError("No Tilemap found with tag 'WallGen'!");
            return;
        }

        tilemap = wallGen.GetComponent<Tilemap>();

        // store all valid tile positions
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
                allTiles.Add(pos);
        }

        StartCoroutine(SpawnOverTime());
    }

    private IEnumerator SpawnOverTime()
    {
        yield return new WaitForSeconds(initialDelay);

        for (int i = 0; i < maxSpawnGroups; i++)
        {
            Vector3Int startTile = GetRandomUnusedTile();
            if (startTile == Vector3Int.zero)
                yield break;

            // Flood fill from this tile to get all connected tiles
            List<Vector3Int> connectedTiles = GetConnectedTiles(startTile);

            foreach (var tilePos in connectedTiles)
            {
                Vector3 worldPos = tilemap.CellToWorld(tilePos) + tilemap.tileAnchor;
                GameObject dWall = Instantiate(DestructibleWall, worldPos, Quaternion.identity);
                dWall.transform.SetParent(transform);
                usedTiles.Add(tilePos);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private Vector3Int GetRandomUnusedTile()
    {
        List<Vector3Int> available = new List<Vector3Int>();
        foreach (var t in allTiles)
            if (!usedTiles.Contains(t))
                available.Add(t);

        if (available.Count == 0) return Vector3Int.zero;
        return available[Random.Range(0, available.Count)];
    }

    // Flood fill to get all connected tiles in all directions
    private List<Vector3Int> GetConnectedTiles(Vector3Int start)
    {
        List<Vector3Int> connected = new List<Vector3Int>();
        Queue<Vector3Int> toCheck = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        toCheck.Enqueue(start);
        visited.Add(start);

        Vector3Int[] directions = new Vector3Int[]
        {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right
        };

        while (toCheck.Count > 0)
        {
            Vector3Int current = toCheck.Dequeue();
            connected.Add(current);

            foreach (var dir in directions)
            {
                Vector3Int next = current + dir;
                if (allTiles.Contains(next) && !usedTiles.Contains(next) && !visited.Contains(next))
                {
                    visited.Add(next);
                    toCheck.Enqueue(next);
                }
            }
        }

        return connected;
    }
}
