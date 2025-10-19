using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlitchFlash : MonoBehaviour
{
    [Header("Glitch Flash Settings")]
    public float flashDuration = 0.2f;
    private bool flashing = false;

    private List<GameObject> glitchParts = new List<GameObject>();

    void Start()
    {
        glitchParts.Add(transform.Find("TrackLeftGlitch")?.gameObject);
        glitchParts.Add(transform.Find("TrackRightGlitch")?.gameObject);
        glitchParts.Add(transform.Find("BatteryGlitch")?.gameObject);
        glitchParts.Add(transform.Find("BodyGlitch")?.gameObject);

        // Hide all glitch parts at the start
        SetGlitchPartsActive(false);
    }

    public void TriggerGlitch()
    {
        if (!flashing)
            StartCoroutine(FlashGlitch());
    }

    private IEnumerator FlashGlitch()
    {
        flashing = true;

        SetGlitchPartsActive(true);
        yield return new WaitForSeconds(flashDuration);
        SetGlitchPartsActive(false);

        flashing = false;
    }

    private void SetGlitchPartsActive(bool active)
    {
        foreach (var part in glitchParts)
        {
            if (part != null)
                part.SetActive(active);
        }
    }
}
