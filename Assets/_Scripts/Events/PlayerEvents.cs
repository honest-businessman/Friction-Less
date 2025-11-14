using System;
using UnityEngine;

public static class PlayerEvents
{
    public static Action<GameObject> OnPlayerSpawned;
    public static Action<float> OnPlayerFireCharged; // float = chargeStartDelay
}
