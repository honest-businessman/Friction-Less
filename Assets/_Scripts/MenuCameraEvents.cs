using UnityEngine;

public class MenuCameraEvents : MonoBehaviour
{
    public void OnCameraMoveComplete()
    {
        Debug.Log("Animation finished! Triggering UnityEvent...");
        MenuAnimationManager.Instance.OnCameraMoveCompleted?.Invoke();
    }
}
