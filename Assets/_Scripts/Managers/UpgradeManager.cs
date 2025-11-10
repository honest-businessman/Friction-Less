using UnityEngine;
using System;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    private PlayerController player;
    private System.Action[] upgradeActions;
    [Header("Base Upgrade Multipliers")]
    [SerializeField] float speedMultiplier = 1.2f;
    [SerializeField] float fireRateMultiplier = 1.2f;
    [SerializeField] float projectileSpeedMultiplier = 1.3f;

    // Number of upgrades to offer
    [SerializeField] int upgradesToOffer = 3;

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();

        upgradeActions = new System.Action[]
        {
            UpgradeSpeed,
            UpgradeProjectileSpeed,
            UpgradeFireRate
        };
    }

    private void UpgradeSpeed()
    {
        float randomMultiplier = UnityEngine.Random.Range(1.1f, 1.3f);
        player.UpgradeSpeed(randomMultiplier);
    }
    private void UpgradeProjectileSpeed()
    {
        float randomMultiplier = UnityEngine.Random.Range(1.1f, 1.3f);
        player.UpgradeProjectileSpeed(randomMultiplier);
    }
    private void UpgradeFireRate()
    {
        float randomMultiplier = UnityEngine.Random.Range(1.1f, 1.3f);
        player.UpgradeFireRate(randomMultiplier);
    }

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
