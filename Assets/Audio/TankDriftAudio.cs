using UnityEngine;
using FMODUnity;

public class TankDriftAudio : MonoBehaviour
{
    [Header("FMOD Events")]
    [SerializeField] private EventReference driftStartEvent;  // Assign your DriftStart event in inspector

    public void PlayDriftStart(Vector3 position)
    {
        // Check if the EventReference has a valid GUID
        if (driftStartEvent.Guid == System.Guid.Empty)
        {
            Debug.LogWarning("DriftStart Event is not assigned!");
            return;
        }

        RuntimeManager.PlayOneShot(driftStartEvent, position);
    }

}
