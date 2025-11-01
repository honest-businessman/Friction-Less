using UnityEngine;

public class UpgradeManager : MonoBehaviour
{

    private PlayerController player;
    private System.Action[] upgradeActions;

    [SerializeField] float speedMultiplier = 1.2f;
    [SerializeField] float fireRateMultiplier = 1.2f;
    [SerializeField] float projecttileSpeedMultiplier = 1.3f;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();

        upgradeActions = new System.Action[]
        {
            () => player.UpgradeSpeed(speedMultiplier),
            () => player.UpgradeProjectileSpeed(projecttileSpeedMultiplier),
            () => player.UpgradeFireRate(fireRateMultiplier)

        };
    }

    public void TriggerUpgrades()
    {
        int optionA = Random.Range(0, upgradeActions.Length);
        int optionB;
        do { optionB = Random.Range(0, upgradeActions.Length); } while (optionB == optionA);

        Debug.Log($"Upgrade options triggered: A={optionA}, B={optionB}");
        UpgradeEvents.UpgradesAvailable(upgradeActions[optionA], upgradeActions[optionB]);
    }

}
