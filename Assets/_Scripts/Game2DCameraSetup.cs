using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteAlways]
public class GameCameraSetup : MonoBehaviour
{
    [Header("Optional Render Texture")]
    public RenderTexture renderTexture;

    [Header("Debug")]
    public bool forceRenderToTextureInEditor = false;

    private void Start()
    {
        ApplyRenderMode();
    }

#if UNITY_EDITOR
    private void Update()
    {
        // Update instantly when changing properties in Editor (for devs)
        if (!Application.isPlaying)
            ApplyRenderMode();
    }
#endif

    private void ApplyRenderMode()
    {
        Camera cam = GetComponent<Camera>();

        // If there’s no assigned RenderTexture, just render normally.
        if (renderTexture == null)
        {
            cam.targetTexture = null;
            return;
        }

        // Decide if we’re in "Menu Mode" or "Scene Dev Mode"
        bool loadedFromMainMenu =
            SceneManager.sceneCount > 1 || forceRenderToTextureInEditor;

        if (loadedFromMainMenu)
        {
            // When loaded additively by main menu, render to texture
            cam.targetTexture = renderTexture;
        }
        else
        {
            // When testing the 2D scene directly, render to Game view
            cam.targetTexture = null;
        }
    }
}
