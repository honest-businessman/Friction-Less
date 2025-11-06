using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState
    {
        MainMenu,
        InGame,
        Paused,
        GameOver
    }
    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private float restartDelay = 3f;
    [SerializeField]
    private float waveDelay = 3f;

    [SerializeField]
    private Vector3 gameSceneOffset = new Vector3(10, 0, 0);
    [SerializeField] private RenderTexture uiRenderTexture;
 

    private InputManager inputManager;
    private bool isUiSceneLoaded = false;
    private WaveManager waveManager;
    private string mainMenuSceneName = "MainMenu";
    private string gameSceneName = "FrictionLess";
    private string uiSceneName = "UIScene";
    GameObject player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            StartMainMenu();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            StartGame();
        }
    }

    public void StartMainMenu()
    {
        Time.timeScale = 1f;
        CleanupPlayer();
        CurrentState = GameState.MainMenu;
        if (!isUiSceneLoaded)  // Loads UI Scene
        { 
            isUiSceneLoaded = true;
            StartCoroutine(LoadSceneAsync(uiSceneName, LoadSceneMode.Additive, () =>
            {
                UIManager.Instance.SetRenderTextureMode(uiRenderTexture);
                UIManager.Instance.ShowMainMenu();
                
            }));
            Debug.Log("UI Scene Loaded.");
        } 
        InputManager.Instance.EnableUIInput();
    }

    public IEnumerator EnterGame()
    {
        yield return LoadSceneAsync(gameSceneName, LoadSceneMode.Additive);
        GameEvents.OnGameStarted?.Invoke();
        StartGame();
    }

    public IEnumerator LoadGame()
    {
        yield return LoadSceneAsync(gameSceneName, LoadSceneMode.Additive);
    }

    public IEnumerator EnterMainMenu()
    {
        yield return LoadSceneAsync(mainMenuSceneName, LoadSceneMode.Single);
    }

    public void StartGame()
    {
        if (!isUiSceneLoaded)  // Loads UI Scene
        {
            isUiSceneLoaded = true;
            StartCoroutine(LoadSceneAsync(uiSceneName, LoadSceneMode.Additive, () =>
            {
                UIManager.Instance.HideAll();
            }));
            Debug.Log("UI Scene Loaded.");
        }
        
        Time.timeScale = 1f;
        CleanupPlayer();
        CurrentState = GameState.InGame;
        SpawnPlayer();
        InitializeInGameManagers();

        InputManager.Instance.EnablePlayerInput(player.GetComponent<PlayerController>());
        StartCoroutine(WaveLoop());
    }

    private void CleanupPlayer()
    {
        if (player != null)
        {
            HealthSystem healthSys = player.GetComponent<HealthSystem>();
            if (healthSys != null)
                healthSys.OnDie -= HandlePlayerDeath;

            Destroy(player);
            player = null;
        }
    }

    private void SpawnPlayer()
    {
        if (player = GameObject.FindWithTag("Player"))
        {
            Debug.Log("Player already exists in the scene.");
            
        }
        else
        {
            Vector3[] spawnPositionArray = GameObject.FindGameObjectsWithTag("Spawnpoint Player")
             .Select(sp => sp.transform.position)
             .ToArray();
            Vector3 spawnposition = spawnPositionArray[UnityEngine.Random.Range(0, spawnPositionArray.Length)];
            player = Instantiate(playerPrefab, spawnposition, Quaternion.identity);
            Debug.Log("Player spawned.");
        }

        PlayerEvents.OnPlayerSpawned?.Invoke(player);
        HealthSystem playerHealthSys = player.GetComponent<HealthSystem>();
        playerHealthSys.OnDie += HandlePlayerDeath;
    }

    private void InitializeInGameManagers()
    {
        inputManager = GetComponentInChildren<InputManager>();

        waveManager = GetComponentInChildren<WaveManager>();
        waveManager.CleanWaves();


    }

    private IEnumerator WaveLoop()
    {
        Debug.Log("Starting Wave Loop...");
        bool waveDone = false;
        waveManager.OnWaveCompleted.AddListener(() => waveDone = true);
        while (true)
        {
            waveDone = false;
            yield return new WaitForSeconds(waveDelay);
            waveManager.NextWave();
            Debug.Log("Wave Loop Waiting for Next Wave...");
            yield return new WaitUntil(() => waveDone);
            Debug.Log($"Wave {waveManager.currentWave} completed.");
        }
    }

    private void HandlePlayerDeath()
    {
        Debug.Log("Player Died. Game Over!");
        EndGame();
    }

    private void EndGame()
    {
        CurrentState = GameState.GameOver;
        StartCoroutine(RestartAfterDelay(restartDelay));
    }

    private IEnumerator RestartAfterDelay(float restartDelay)
    {
        Debug.Log($"Restarting in {restartDelay} seconds...");
        yield return new WaitForSeconds(restartDelay);
        // asynchronously reload the current scene
        waveManager.CleanWaves();
        yield return SceneManager.UnloadSceneAsync(1);
        yield return LoadSceneAsync("FrictionLess", LoadSceneMode.Additive);

        // Once Loaded, restart the game
        GameEvents.OnGameRestarted?.Invoke();
        StartGame();
    }

    private IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode, Action onComplete = null)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);

        // Wait until scene is loaded to 90% (internal ready)
        while (!asyncLoad.isDone)
        {
            yield return null; // animations continue running
        }

        onComplete?.Invoke();
    }

    public void PauseRecieve()
    {
        if(CurrentState == GameState.InGame)
        {
            PauseGame();
        }
        else if(CurrentState == GameState.Paused)
        {
            UnpauseGame();
        }
    }
    private void PauseGame()
    {
        if (CurrentState == GameState.InGame)
        {
            CurrentState = GameState.Paused;
            Time.timeScale = 0f;
            GameEvents.OnGamePaused.Invoke();
        }
    }
    private void UnpauseGame()
    {
        if (CurrentState == GameState.Paused)
        {
            CurrentState = GameState.InGame;
            Time.timeScale = 1f;
            GameEvents.OnGameResumed.Invoke();
        }
    }
}
