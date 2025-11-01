using System;
using UnityEngine;

public static class UpgradeEvents 
{
    public static event Action<System.Action, System.Action> OnUpgradesAvailable;

    public static void UpgradesAvailable(System.Action upgrade1, System.Action upgrade2)
    {
        OnUpgradesAvailable?.Invoke(upgrade1, upgrade2);
    }
}
