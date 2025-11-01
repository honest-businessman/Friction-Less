using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class FloorReflectionSetup : MonoBehaviour
{
    [Header("References")]
    public Tilemap floorTilemap;
    public Camera reflectionCamera;       
    public Material floorMaterial;        

    [Header("Render Texture Settings")]
    public int targetTextureHeight = 512; // Desired vertical resolution of RenderTexture

    private RenderTexture reflectionRT;

    void Start()
    {
        reflectionCamera = GameObject.FindGameObjectWithTag("ReflectionCamera").GetComponent<Camera>();
        reflectionCamera.clearFlags = CameraClearFlags.SolidColor;
        reflectionCamera.backgroundColor = Color.black;
        SetupReflection();
    }

    void SetupReflection()
    {
        if (floorTilemap == null || reflectionCamera == null || floorMaterial == null)
        {
            Debug.LogError("Assign floorTilemap, reflectionCamera, and floorMaterial in inspector.");
            return;
        }

        // Get bounds of the floor tilemap renderer
        Bounds floorBounds = floorTilemap.GetComponent<TilemapRenderer>().bounds;

        float width = floorBounds.size.x;
        float height = floorBounds.size.y;

        if (height <= 0 || width <= 0)
        {
            Debug.LogError("Floor bounds size invalid.");
            return;
        }

        // Calculate aspect ratio
        float aspectRatio = width / height;

        // Calculate target width based on desired height and aspect ratio
        int targetTextureWidth = Mathf.RoundToInt(targetTextureHeight * aspectRatio);

        // Create Render Texture
        if (reflectionRT != null)
        {
            reflectionRT.Release();
            Destroy(reflectionRT);
        }

        reflectionRT = new RenderTexture(targetTextureWidth, targetTextureHeight, 16, RenderTextureFormat.ARGB32);
        reflectionRT.name = "ReflectionRenderTexture";
        reflectionRT.Create();

        floorMaterial.SetVector("_FloorMinBounds", new Vector4(floorBounds.min.x, floorBounds.min.y, 0, 0));
        floorMaterial.SetVector("_FloorMaxBounds", new Vector4(floorBounds.max.x, floorBounds.max.y, 0, 0));
        Vector3 center = floorBounds.center;
        floorMaterial.SetVector("_FloorCenter", new Vector4(center.x, center.y, 0f, 0f));


        // Assign Render Texture to reflection camera
        reflectionCamera.targetTexture = reflectionRT;


        // Configure reflection camera
        reflectionCamera.orthographic = true;
        reflectionCamera.orthographicSize = height / 2f;
        reflectionCamera.aspect = aspectRatio;

        // Position camera to center of floor bounds (keep Z as is)
        Vector3 camPos = reflectionCamera.transform.position;
        reflectionCamera.transform.position = new Vector3(floorBounds.center.x, floorBounds.center.y, camPos.z);

        // Assign RenderTexture to floor material
        floorMaterial.SetTexture("_ReflectionTex", reflectionRT);

        // Pass floor bounds to material shader (_FloorMinBounds, _FloorMaxBounds expected as Vector4)
        floorMaterial.SetVector("_FloorMinBounds", new Vector4(floorBounds.min.x, floorBounds.min.y, 0f, 0f));
        floorMaterial.SetVector("_FloorMaxBounds", new Vector4(floorBounds.max.x, floorBounds.max.y, 0f, 0f));

        Debug.Log($"Reflection setup done: RT size {targetTextureWidth}x{targetTextureHeight}, aspect {aspectRatio}");
    }

    void OnDestroy()
    {
        if (reflectionRT != null)
        {
            reflectionRT.Release();
            DestroyImmediate(reflectionRT);
        }
    }
}