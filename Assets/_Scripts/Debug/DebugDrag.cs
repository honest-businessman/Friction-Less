using UnityEngine;
using UnityEngine.EventSystems;
public class DebugDrag : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData e)
    {
        Debug.Log($"{name} Drag: {e.position}");
    }
}

