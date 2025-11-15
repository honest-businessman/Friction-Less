using UnityEngine;
using System;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    private PlayerController player;
    private TurretController turretController;

    // Number of upgrades to offer
    [SerializeField] int upgradesToOffer = 3;

    [SerializeField] List<UpgradeItem> availableUpgrades;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        turretController = player.turretController;
    }

    public void TriggerUpgrades()
    {
        if (upgradesToOffer > availableUpgrades.Count)
        {
            Debug.LogWarning("Cannot offer more unique upgrades than available!");
            return;
        }

        // Instantiate all ScriptableObject items
        List<UpgradeItem> shuffled = new List<UpgradeItem>();
        foreach (UpgradeItem item in availableUpgrades)
        {
            shuffled.Add(Instantiate(item));
        }

        // Shuffle the array and take the first N options
        for (int i = 0; i < shuffled.Count; i++)
        {
            int randIndex = UnityEngine.Random.Range(i, shuffled.Count);
            var temp = shuffled[i];
            shuffled[i] = shuffled[randIndex];
            shuffled[randIndex] = temp;
        }

        UpgradeItem[] chosenUpgrades = shuffled.GetRange(0, upgradesToOffer).ToArray();

        Debug.Log("Chosen upgrades:");
        for (int i = 0; i < chosenUpgrades.Length; i++)
            Debug.Log($"Option {i + 1}: {chosenUpgrades[i].name}");

        // Assign actions to upgrades
        foreach (UpgradeItem item in chosenUpgrades)
        {
            System.Action<float> action;
            switch (item.upgradeType)
            {
                case UpgradeItem.UpgradeType.MoveSpeed: action = UpgradeMoveSpeed; break;
                case UpgradeItem.UpgradeType.ShellSpeed: action = UpgradeShellSpeed; break;
                case UpgradeItem.UpgradeType.FireRate: action = UpgradeFireRate; break;
                case UpgradeItem.UpgradeType.DriveChargeSpeed: action = UpgradeDriveChargeSpeed; break;
                case UpgradeItem.UpgradeType.AmmunitionPower: action = UpgradeAmmunitionPower; break;
                default: action = (value) => Debug.Log($"No Upgrade action for upgrade type {item.upgradeType}"); break;
            }
            item.action = () => action(item.value);
        }

        // Invoke the UpgradesAvailable event with the chosen upgrades
        UpgradeEvents.UpgradesAvailable(chosenUpgrades);
    }

    private void UpgradeMoveSpeed(float value) => player.UpgradeMoveSpeed(value);
    private void UpgradeShellSpeed(float value) => turretController.UpgradeShellSpeed(value);
    private void UpgradeFireRate(float value) => turretController.UpgradeFireRate(value);
    private void UpgradeDriveChargeSpeed(float value) => player.UpgradeDriveChargeSpeed(value);
    private void UpgradeAmmunitionPower(float value) => turretController.UpgradeAmmunitionPower(value);
}
