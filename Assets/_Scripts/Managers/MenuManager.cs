using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("UI Setup")]
    [SerializeField] 
    private List<Button3D> buttons = new List<Button3D>();
    public string gameSceneName;

    [Header("Button Colors")]
    public Color colorButton = Color.white;
    public Color colorButtonActive = Color.yellow;
    public Color colorButtonHover = Color.cyan;

    [Header("Navigation Settings")]
    [SerializeField] private float navigateThreshold = 0.5f;
    [SerializeField] private float deadzone = 0.2f;

    [Header("Events")]
    public UnityEvent OnStartSequence = new UnityEvent();

    private int selectedIndex = 0;
    private bool inputLocked = false;
    private bool navigateHeld = false;

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

    public void Initialize()
    {
        if (buttons.Count > 0 && GameManager.Instance.CurrentState == GameManager.GameState.MainMenu)
        {
            selectedIndex = 0;
            buttons[selectedIndex].Select();
        }
    }


    public void StartPressed()
    {
        if (inputLocked) return;
        inputLocked = true;

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
        if (inputLocked) return;
        inputLocked = true;

        Debug.Log("Quitting Game");
        Application.Quit();
    }

    public void SettingsPressed()
    {
        if (inputLocked) return;

        Debug.Log("Opening Settings");
    }

    public void HandleNavigate(Vector2 direction)
    {
        if (inputLocked || buttons.Count == 0) return;

        float x = direction.x;

        if (Mathf.Abs(x) < deadzone)
        {
            navigateHeld = false; // reset when stick returns to center
            return;
        }

        // Only act once per stick movement
        if (navigateHeld) return;

        if (x < navigateThreshold)
            SelectPreviousButton();
        else if (x > -navigateThreshold)
            SelectNextButton();

        navigateHeld = true;
    }
    public void HandleSubmit()
    {
        if (inputLocked || buttons.Count == 0) return;

        var selectedButton = buttons[selectedIndex];
        if (selectedButton != null)
        {
            selectedButton.Press();
        }
    }
    private void SelectNextButton()
    {
        buttons[selectedIndex].Deselect();
        selectedIndex = (selectedIndex + 1) % buttons.Count;
        buttons[selectedIndex].Select();
    }

    private void SelectPreviousButton()
    {
        buttons[selectedIndex].Deselect();
        selectedIndex = (selectedIndex - 1 + buttons.Count) % buttons.Count;
        buttons[selectedIndex].Select();
    }

}
