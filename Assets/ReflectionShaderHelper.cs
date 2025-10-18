using UnityEngine;
using UnityEngine.Tilemaps;

public class ReflectionShaderHelper : MonoBehaviour
{
    private void Start()
    {
        var renderer = GetComponent<TilemapRenderer>();
        var bounds = renderer.bounds; // Get the world bounds

        // Access material
        Material floorMaterial = renderer.material;

        // Set the min and max bounds Vector2 to the shader
        Vector2 minBounds = new Vector2(bounds.min.x, bounds.min.y);
        Vector2 maxBounds = new Vector2(bounds.max.x, bounds.max.y);

        floorMaterial.SetVector("_FloorMinBounds", minBounds);
        floorMaterial.SetVector("_FloorMaxBounds", maxBounds);

    }
}
