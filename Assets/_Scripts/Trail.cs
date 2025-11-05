using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputHandler))]
public class Trail : MonoBehaviour
{
    [Header("Trail Renderers")]
    public TrailRenderer trailLeft;
    public TrailRenderer trailRight;

    [Header("Trail Settings")]
    [Tooltip("TrailRenderer.time should be set in inspector and not changed at runtime.")]
    public float trailLifetime = 0.5f;

    [Header("Linger & Fade Settings")]
    [Tooltip("Time the trail stays fully visible after stopping.")]
    public float lingerTime = 1f;
    [Tooltip("Time it takes for the detached trail to fade out.")]
    public float fadeOutTime = 0.5f;

    private InputHandler inputHandler;
    private bool isDrifting = false;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
    }

    private void OnEnable()
    {
        inputHandler.OnInputDrift.AddListener(OnDriftInput);
    }

    private void OnDisable()
    {
        inputHandler.OnInputDrift.RemoveListener(OnDriftInput);
    }

    private void Start()
    {
        // Initialize trails
        trailLeft.emitting = false;
        trailRight.emitting = false;
        trailLeft.time = trailLifetime;
        trailRight.time = trailLifetime;
    }

    private void OnDriftInput(InputValue value)
    {
        bool drifting = value.isPressed;

        if (drifting && !isDrifting)
            StartDrift();
        else if (!drifting && isDrifting)
            StopDrift();
    }

    private void StartDrift()
    {
        if (isDrifting) return;
        isDrifting = true;

        // Clear old segments to prevent tiny trail artifacts
        trailLeft.Clear();
        trailRight.Clear();

        // Enable emitting for new trail
        trailLeft.emitting = true;
        trailRight.emitting = true;
    }

    private void StopDrift()
    {
        if (!isDrifting) return;
        isDrifting = false;

        // Stop emitting new segments
        trailLeft.emitting = false;
        trailRight.emitting = false;

        // Detach the trails into independent GameObjects
        DetachTrail(trailLeft);
        DetachTrail(trailRight);
    }

    private void DetachTrail(TrailRenderer trail)
    {
        if (trail == null) return;

        // Create a detached copy at current position and rotation
        GameObject detachedGO = new GameObject("DetachedTrail");
        detachedGO.transform.position = trail.transform.position;
        detachedGO.transform.rotation = trail.transform.rotation;

        TrailRenderer detachedTrail = detachedGO.AddComponent<TrailRenderer>();

        // Copy the main trail's settings
        detachedTrail.material = trail.material;
        detachedTrail.startWidth = trail.startWidth;
        detachedTrail.endWidth = trail.endWidth;
        detachedTrail.time = trail.time;
        detachedTrail.minVertexDistance = trail.minVertexDistance;
        detachedTrail.numCapVertices = trail.numCapVertices;
        detachedTrail.numCornerVertices = trail.numCornerVertices;
        detachedTrail.autodestruct = true; // optional, destroys when finished

        // Keep it static in world space
        detachedGO.transform.parent = null;

        // Let the trail linger fully for lingerTime, then fade
        StartCoroutine(FadeDetachedTrail(detachedTrail));
    }

    private IEnumerator FadeDetachedTrail(TrailRenderer detached)
    {
        if (detached == null) yield break;

        Material mat = detached.material;
        Color originalColor = mat.HasProperty("_Color") ? mat.color : Color.white;

        // 1) Linger fully visible
        yield return new WaitForSeconds(lingerTime);

        // 2) Fade alpha over fadeOutTime
        float startTime = Time.time;
        float endTime = startTime + fadeOutTime;

        while (Time.time < endTime)
        {
            float t = 1f - ((Time.time - startTime) / fadeOutTime);
            Color c = originalColor;
            c.a = t;
            mat.color = c;
            yield return null;
        }

        // Ensure fully transparent
        Color final = originalColor;
        final.a = 0f;
        mat.color = final;

        // Destroy the detached trail object
        if (detached != null)
            Destroy(detached.gameObject);
    }
}
