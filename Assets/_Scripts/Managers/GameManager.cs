using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEditor.SearchService;

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
    public int HazardLevel { get; private set; } = 0;
    public static UnityEvent OnPause = new UnityEvent();
    public static UnityEvent OnUnpause = new UnityEvent();

    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private float restartDelay = 3f;
    [SerializeField]
    private float waveDelay = 3f;

    private InputManager inputManager;
    private UIManager uiManager;
    private WaveManager waveManager;
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
        StartMainMenu();
    }

    public void StartMainMenu()
    {
        Time.timeScale = 1f;
        CleanupPlayer();
        CurrentState = GameState.MainMenu;
        MenuAnimationManager.Instance.OnCameraMoveCompleted?.AddListener(OnCameraMoveCompleted);

        InputManager.Instance.EnableUIInput();
    }

    private void OnCameraMoveCompleted()
    {
        StartCoroutine(EnterGame());
    }

    public IEnumerator EnterGame()
    {
        yield return LoadSceneAsync(1);
        StartGame();
    }
    public IEnumerator EnterMainMenu()
    {
        yield return LoadSceneAsync(0);
    }

    public void StartGame()
    {
        MenuAnimationManager.Instance.OnCameraMoveCompleted?.RemoveListener(OnCameraMoveCompleted);
        Time.timeScale = 1f;
        CleanupPlayer();
        HazardLevel = 0;
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
            Vector3 spawnposition = spawnPositionArray[Random.Range(0, spawnPositionArray.Length)];
            player = Instantiate(playerPrefab, spawnposition, Quaternion.identity);
            Debug.Log("Player spawned.");
        }

        HealthSystem playerHealthSys = player.GetComponent<HealthSystem>();
        playerHealthSys.OnDie += HandlePlayerDeath;
    }


    private void InitializeInGameManagers()
    {
        inputManager = GetComponentInChildren<InputManager>();
        inputManager.GameManager = this;

        uiManager = GetComponentInChildren<UIManager>();
        uiManager.playerController = player.GetComponent<PlayerController>();
        uiManager.meterText = GameObject.FindGameObjectWithTag("UI Drive Charge").GetComponent<TextMeshProUGUI>(); ;

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
        yield return LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        // Once Loaded, restart the game
        StartGame();
    }

    private IEnumerator LoadSceneAsync(int buildIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
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
            OnPause.Invoke();
        }
    }
    private void UnpauseGame()
    {
        if (CurrentState == GameState.Paused)
        {
            CurrentState = GameState.InGame;
            Time.timeScale = 1f;
            OnUnpause.Invoke();
        }
    }
}
