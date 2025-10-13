using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

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
    [SerializeField]
    public GameState CurrentState { get; private set; } = GameState.MainMenu;
    public static UnityEvent OnPause = new UnityEvent();
    public static UnityEvent OnUnpause = new UnityEvent();

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
        CurrentState = GameState.InGame;
        // Additional logic to initialize the game
    }


    public void PauseInput()
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
