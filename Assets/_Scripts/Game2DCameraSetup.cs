using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class GameCameraSetup : MonoBehaviour
{
    static GameObject Instance;

    [Header("Optional Render Texture")]
    public RenderTexture renderTexture;

    [Header("Debug")]
    public bool forceRenderToTextureInEditor = false;

    private void Awake()
    {
        if (!Application.isPlaying)
            return;
        // Ensure singleton instance
        if (Instance == null)
        {
            Instance = this.gameObject;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

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
        if (renderTexture == null)
        {
            cam.targetTexture = null;
            return;
        }

        // In editor, use forceRenderToTextureInEditor; otherwise use GameManager
        bool loadedFromMainMenu = false;

#if UNITY_EDITOR
        if (!Application.isPlaying && forceRenderToTextureInEditor)
        {
            loadedFromMainMenu = true;
        }
        else
#endif
        {
            loadedFromMainMenu = GameManager.Instance != null && GameManager.Instance.isMainScene;
        }

        if (loadedFromMainMenu)
        {
            cam.targetTexture = renderTexture;
            //GetComponent<AudioListener>().enabled = false; // Disable audio listener to avoid conflicts with main camera
        }
        else
        {
            cam.targetTexture = null;
        }
    }


}
