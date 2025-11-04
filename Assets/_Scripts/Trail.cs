using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputHandler))]
public class Trail : MonoBehaviour
{
    [Header("Trail Renderers")]
    public TrailRenderer trailLeft;
    public TrailRenderer trailRight;

    [Header("Fade Settings")]
    public float fadeOutTime = 0.5f;

    private InputHandler inputHandler;
    private bool isDrifting = false;
    private Coroutine fadeRoutine;

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
        // Make sure trails start off
        trailLeft.emitting = false;
        trailRight.emitting = false;
    }

    private void OnDriftInput(InputValue value)
    {
        bool drifting = value.isPressed;
        Debug.Log($"Drift pressed: {drifting}"); // ✅ ADD THIS
        if (drifting && !isDrifting)
        {
            StartDrift();
        }
        else if (!drifting && isDrifting)
        {
            StopDrift();
        }
    }


    private void StartDrift()
    {
        isDrifting = true;
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        trailLeft.emitting = true;
        trailRight.emitting = true;
    }

    private void StopDrift()
    {
        isDrifting = false;
        fadeRoutine = StartCoroutine(FadeOutTrails());
    }

    private IEnumerator FadeOutTrails()
    {
        float startTime = Time.time;
        float initialTimeLeft = trailLeft.time;
        float initialTimeRight = trailRight.time;

        while (Time.time < startTime + fadeOutTime)
        {
            float t = 1 - ((Time.time - startTime) / fadeOutTime);
            trailLeft.time = Mathf.Lerp(0, initialTimeLeft, t);
            trailRight.time = Mathf.Lerp(0, initialTimeRight, t);
            yield return null;
        }

        // Reset and turn off
        trailLeft.time = initialTimeLeft;
        trailRight.time = initialTimeRight;
        trailLeft.emitting = false;
        trailRight.emitting = false;
    }


}
