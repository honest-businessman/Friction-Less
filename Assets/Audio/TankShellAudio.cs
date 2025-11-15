using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class TankShellAudio : MonoBehaviour
{
    [SerializeField] private HealthSystem playerHealth;

    // Event paths for the 3 shells
    [SerializeField] private string shell1Event = "event:/TankShot1";
    [SerializeField] private string shell2Event = "event:/TankShot2";
    [SerializeField] private string shell3Event = "event:/TankShot3";

    public void PlayShell(Vector3 position)
    {
        if (playerHealth == null) return;

        string eventToPlay;

        // Map health ranges to shell events
        if (playerHealth.health <= 1)
        {
            eventToPlay = shell3Event; // 0-1 HP → Shell 3
        }
        else if (playerHealth.health <= 2)
        {
            eventToPlay = shell2Event; // 1-2 HP → Shell 2
        }
        else
        {
            eventToPlay = shell1Event; // 2-3 HP → Shell 1
        }

        // Play the FMOD event as 3D sound
        EventInstance instance = RuntimeManager.CreateInstance(eventToPlay);
        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(position));
        instance.start();
        instance.release(); // clean up
    }
}
