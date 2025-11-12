using System;
using UnityEngine;

public static class UpgradeEvents
{
    public static event Action<System.Action[]> OnUpgradesAvailable;

    public static void UpgradesAvailable(params System.Action[] upgrades)
    {
        Debug.Log("Upgrades available event triggered.");
        OnUpgradesAvailable?.Invoke(upgrades);
    }
}
