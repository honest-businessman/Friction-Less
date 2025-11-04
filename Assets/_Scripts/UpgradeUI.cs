using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Button optionAButton;
    [SerializeField] private Button optionBButton;
    [SerializeField] private Button optionCButton;

    private System.Action optionAAction;
    private System.Action optionBAction;
    private System.Action optionCAction;

    private void OnEnable()
    {
        UpgradeEvents.OnUpgradesAvailable += ShowUpgradeOptions;
    }

    private void OnDisable()
    {
        UpgradeEvents.OnUpgradesAvailable -= ShowUpgradeOptions;
    }

    private void ShowUpgradeOptions(System.Action[] upgradeOptions)
    {
        if (upgradeOptions.Length < 3)
        {
            Debug.LogWarning("Not enough upgrades to show all 3.");
            return;
        }

        // Assign each action
        optionAAction = upgradeOptions[0];
        optionBAction = upgradeOptions[1];
        optionCAction = upgradeOptions[2];

        // Show panel
        upgradePanel.SetActive(true);
        Time.timeScale = 0f;

        // Setup button listeners
        optionAButton.onClick.RemoveAllListeners();
        optionAButton.onClick.AddListener(() => SelectUpgrade(optionAAction));

        optionBButton.onClick.RemoveAllListeners();
        optionBButton.onClick.AddListener(() => SelectUpgrade(optionBAction));

        optionCButton.onClick.RemoveAllListeners();
        optionCButton.onClick.AddListener(() => SelectUpgrade(optionCAction));

        // Make sure all buttons are active
        optionAButton.gameObject.SetActive(true);
        optionBButton.gameObject.SetActive(true);
        optionCButton.gameObject.SetActive(true);
    }

    private void SelectUpgrade(System.Action chosenUpgrade)
    {
        chosenUpgrade?.Invoke();
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
