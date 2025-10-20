using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    public string gameSceneName;
    public Color colorButton;
    public Color colorButtonActive;
    public Color colorButtonHover;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
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

    public void ExitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    public void OpenSettings()
    {
       Debug.Log("Opening Settings");
        // Implement settings menu logic here
    }
}
