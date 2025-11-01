using UnityEngine;

public class WallObject : MonoBehaviour
{
    protected Vector3Int tilePos;
    protected WallSpawning spawner;

    public void Initialize(WallSpawning wallSpawner, Vector3Int tilePosition)
    {
        spawner = wallSpawner;
        tilePos = tilePosition;
    }

    protected virtual void OnDestroy()
    {
        // free tile when destroyed
        if (spawner != null)
        {
            spawner.FreeTile(tilePos);
        }
    }
}
