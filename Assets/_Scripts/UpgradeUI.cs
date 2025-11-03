using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Button optionAButton;
    [SerializeField] private Button optionBButton;
    [SerializeField] private TextMeshProUGUI optionAText;
    [SerializeField] private TextMeshProUGUI optionBText;


    private System.Action optionAAction;
    private System.Action optionBAction;

    private void OnEnable()
    {
        UpgradeEvents.OnUpgradesAvailable += ShowUpgradeOptions;
    }
    void OnDisable()
    {
        UpgradeEvents.OnUpgradesAvailable -= ShowUpgradeOptions;
    }

    private void ShowUpgradeOptions(System.Action actionA, System.Action actionB)
    {
        optionAAction = actionA;
        optionBAction = actionB;

        
        upgradePanel.SetActive(true);
        Time.timeScale = 0f;

        //refect text to button
        optionAText.text = GetUpgradeDescription(actionA);
        optionBText.text = GetUpgradeDescription(actionB);

        optionAButton.onClick.RemoveAllListeners();
        optionAButton.onClick.AddListener(() => SelectUpgrade(optionAAction));

        optionBButton.onClick.RemoveAllListeners();
        optionBButton.onClick.AddListener(() => SelectUpgrade(optionBAction));
    }
    private void SelectUpgrade(System.Action chosenUpgrade)
    {

        Debug.Log("Upgrade selected, invoking action...");
        chosenUpgrade?.Invoke();
        Debug.Log("Upgrade applied successfully!");


        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private string GetUpgradeDescription(System.Action upgradeAction)
    {
        if (upgradeAction.Method.Name.Contains("UpgradeSpeed")) return "Move Speed Up";

        if(upgradeAction.Method.Name.Contains("UpgradeFireRate")) return "Fire Rate Up";

        if(upgradeAction.Method.Name.Contains("UpgradeProjectileSpeed")) return "Projectile Speed Up";

        return "Unknown upgrade";
    }

   
}
