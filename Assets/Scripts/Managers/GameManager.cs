using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.SceneManagement;
using System.Collections;

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

    private InputManager inputManager;
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
        StartGame();
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        CleanupPlayer();
        HazardLevel = 0;
        CurrentState = GameState.InGame;
        SpawnPlayer();
        InitializeManagers();
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

    private void InitializeManagers()
    {
        inputManager = GetComponentInChildren<InputManager>();
        inputManager.Player = player.GetComponent<PlayerController>();
        inputManager.GameManager = this;
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
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        while (!asyncLoad.isDone) // Wait until the asynchronous scene fully loads
        {
            yield return null;
        }

        // Once Loaded, restart the game
        StartGame();
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
