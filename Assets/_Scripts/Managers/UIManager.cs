using System;
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
    public GameObject upgradePanel;

    private UpgradeUI upgradeUI;
    private bool isSettingsOpen = false;
    private Action<Action[]> upgradeHandler;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        uiCamera = GameObject.FindWithTag("2D Camera").GetComponent<Camera>();
        upgradeUI = upgradePanel.GetComponent<UpgradeUI>();
    }

    private void OnEnable()
    {
        upgradeHandler = (actions) =>
        {
            upgradeUI.gameObject.SetActive(true);
            upgradeUI.ShowUpgradeOptions(actions);
        };

        UpgradeEvents.OnUpgradesAvailable += upgradeHandler;
    }

    private void OnDisable()
    {
        UpgradeEvents.OnUpgradesAvailable -= upgradeHandler;
    }

    public void HandleNavigate(Vector2 direction)
    {
        if (upgradeUI.isActiveAndEnabled) { upgradeUI.HandleNavigate(direction); }
    }

    public void HandleSubmit()
    {
        if (upgradeUI.isActiveAndEnabled) { upgradeUI.HandleSubmit(); }
        
    }

    public void SetRenderTextureMode(RenderTexture target)
    {
        uiCamera.targetTexture = target;
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
    public Canvas GetUICanvas() { return uiCanvas; }
}
