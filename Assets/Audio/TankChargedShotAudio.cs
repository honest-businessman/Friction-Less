using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class TankChargedShotAudio : MonoBehaviour
{
    [SerializeField] private EventReference tankTrailShotEvent;

    public void PlayTrailShot(Vector3 pos)
    {
        FMOD.Studio.EventInstance e = RuntimeManager.CreateInstance(tankTrailShotEvent);
        e.set3DAttributes(RuntimeUtils.To3DAttributes(pos));
        e.start();
        e.release();
    }
}
