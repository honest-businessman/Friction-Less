using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    public string gameSceneName;
    public Color colorButton;
    public Color colorButtonActive;
    public Color colorButtonHover;

    public UnityEvent OnStartSequence = new UnityEvent();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
}

    private void Start()
    {
        MenuAnimationManager.Instance.OnCameraMoveCompleted.AddListener(StartGame);
    }
    private void OnDisable()
    {
        MenuAnimationManager.Instance.OnCameraMoveCompleted.RemoveListener(StartGame);
    }


    public void StartPressed()
    {
        OnStartSequence?.Invoke();
    }

    public void StartGame()
    {
        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void ExitPressed()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    public void SettingsPressed()
    {
       Debug.Log("Opening Settings");

    }
}
