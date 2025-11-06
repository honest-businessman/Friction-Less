using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Canvas uiCanvas;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject pausePanel;

    private bool isSettingsOpen = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetRenderTextureMode(RenderTexture target)
    {
        uiCamera.targetTexture = target;
        uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        uiCanvas.worldCamera = uiCamera;
    }

    public void SetScreenMode()
    {
        uiCamera.targetTexture = null;
        uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        uiCanvas.worldCamera = uiCamera;
    }


    public void ShowMainMenu()
    {
        HideAll();
        mainMenuPanel.SetActive(true);
    }
    public void ShowSettingsMenu()
    {
        HideAll();
        settingsPanel.SetActive(true);
        isSettingsOpen = true;
    }
    public void ShowPauseMenu()
    {
        HideAll();
        pausePanel.SetActive(true);
    }

    public void HideAll()
    {
        mainMenuPanel?.SetActive(false);
        settingsPanel?.SetActive(false);
        isSettingsOpen = false;
        pausePanel?.SetActive(false);
    }

    public bool IsSettingsOpen() { return isSettingsOpen; }
}
