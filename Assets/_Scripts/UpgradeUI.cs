using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class UpgradeUI : MonoBehaviour
{
    [Header("Upgrade Buttons")]
    [SerializeField] private Button optionAButton;
    [SerializeField] private Button optionBButton;
    [SerializeField] private Button optionCButton;
    [Header("Upgrade Borders")]
    [SerializeField] private GameObject optionABorder;
    [SerializeField] private GameObject optionBBorder;
    [SerializeField] private GameObject optionCBorder;

    private Action optionAAction;
    private Action optionBAction;
    private Action optionCAction;

    [Header("Navigation Settings")]
    [SerializeField] private float navigateThreshold = 0.5f;
    [SerializeField] private float deadzone = 0.2f;
    [SerializeField] private bool inputLocked = false;

    private int selectedIndex = 0;
    private bool navigateHeld = false;

    private List<Button> buttons = new List<Button>();
    private List<GameObject> borders = new List<GameObject>();

    private void Awake()
    {
        buttons.Add(optionAButton);
        buttons.Add(optionBButton);
        buttons.Add(optionCButton);
        borders.Add(optionABorder);
        borders.Add(optionBBorder);
        borders.Add(optionCBorder);
        gameObject.SetActive(false);
    }

    // Called when upgrades are offered
    public void ShowUpgradeOptions(Action[] upgradeOptions)
    {
        if (upgradeOptions.Length < 3)
        {
            Debug.LogWarning("Not enough upgrades to show all 3.");
            gameObject.SetActive(false);
            return;
        }

        // Assign actions
        optionAAction = upgradeOptions[0];
        optionBAction = upgradeOptions[1];
        optionCAction = upgradeOptions[2];

        // Setup button listeners
        optionAButton.onClick.RemoveAllListeners();
        optionAButton.onClick.AddListener(() => ChooseUpgrade(optionAAction));

        optionBButton.onClick.RemoveAllListeners();
        optionBButton.onClick.AddListener(() => ChooseUpgrade(optionBAction));

        optionCButton.onClick.RemoveAllListeners();
        optionCButton.onClick.AddListener(() => ChooseUpgrade(optionCAction));

        // Activate buttons
        foreach (var btn in buttons)
            btn.gameObject.SetActive(true);

        gameObject.SetActive(true);
        InputManager.Instance.EnableUIInput();
        Time.timeScale = 0f;

        // Reset selection
        selectedIndex = 0;
        UpdateButtonSelection();
    }

    private void ChooseUpgrade(Action chosenUpgrade)
    {
        chosenUpgrade?.Invoke();
        gameObject.SetActive(false);
        InputManager.Instance.EnablePlayerInput(GameManager.Instance.player.GetComponent<PlayerController>());
        Time.timeScale = 1f;
    }

    // === Input Handlers ===
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

        if (x < -navigateThreshold)
            SelectPreviousButton();
        else if (x > navigateThreshold)
            SelectNextButton();

        navigateHeld = true;
    }

    public void HandleSubmit()
    {
        if (inputLocked || buttons.Count == 0) return;

        Button selectedButton = buttons[selectedIndex];
        if (selectedButton != null)
        {
            selectedButton.onClick.Invoke(); // simulate click
        }
    }

    private void SelectNextButton()
    {
        selectedIndex = (selectedIndex + 1) % buttons.Count;
        UpdateButtonSelection();
    }

    private void SelectPreviousButton()
    {
        selectedIndex = (selectedIndex - 1 + buttons.Count) % buttons.Count;
        UpdateButtonSelection();
    }

    private void UpdateButtonSelection()
    {
        // Deselect all, then highlight the current one
        for (int i = 0; i < borders.Count; i++)
        {
            borders[i].SetActive(false);
        }

        borders[selectedIndex].SetActive(true); // ensures proper UI focus
    }
}
