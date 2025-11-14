using UnityEngine;
using System;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    private PlayerController player;
    private TurretController turretController;
    private System.Action[] upgradeActions;

    [SerializeField] float speedMultiplier = 1.1f;
    [SerializeField] float fireRateMultiplier = 1.2f;
    [SerializeField] float projectileSpeedMultiplier = 1.3f;

    // Number of upgrades to offer
    [SerializeField] int upgradesToOffer = 3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        turretController = player.turretController;
    }

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();

        upgradeActions = new System.Action[]
        {
            UpgradeMoveSpeed,
            UpgradeProjectileSpeed,
            UpgradeFireRate
        };
    }

    private void UpgradeMoveSpeed() => player.UpgradeMoveSpeed(speedMultiplier);
    private void UpgradeProjectileSpeed() => turretController.UpgradeShellSpeed(projectileSpeedMultiplier);
    private void UpgradeFireRate() => turretController.UpgradeFireRate(fireRateMultiplier);

    public void TriggerUpgrades()
    {
        if (upgradesToOffer > upgradeActions.Length)
        {
            Debug.LogWarning("Cannot offer more unique upgrades than available!");
            return;
        }

        // Shuffle the array and take the first N options
        List<System.Action> shuffled = new List<System.Action>(upgradeActions);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int randIndex = UnityEngine.Random.Range(i, shuffled.Count);
            var temp = shuffled[i];
            shuffled[i] = shuffled[randIndex];
            shuffled[randIndex] = temp;
        }

        System.Action[] chosenUpgrades = shuffled.GetRange(0, upgradesToOffer).ToArray();

        Debug.Log("Chosen upgrades:");
        for (int i = 0; i < chosenUpgrades.Length; i++)
            Debug.Log($"Option {i + 1}: {chosenUpgrades[i].Method.Name}");

        // Invoke the event with the chosen upgrades
        // You can modify UpgradeEvents to accept a variable number of upgrades using params
        UpgradeEvents.UpgradesAvailable(chosenUpgrades);
    }
}
