using UnityEngine;

public class TrailFader : MonoBehaviour
{
    public float delay = 0.1f;

    private TrailRenderer tr;
    private Material mat;
    private Color initialColor;
    private float elapsedTime = 0f;
    private float fadeDuration = 2f;
    private bool isFading = false;

    void Awake()
    {
        tr = GetComponent<TrailRenderer>();
        mat = tr.material;
        initialColor = mat.color;
        fadeDuration = tr.time - delay;
    }

    void OnEnable()
    {
        StartCoroutine(FadeTrail());
    }

    private System.Collections.IEnumerator FadeTrail()
    {
        yield return new WaitForSeconds(delay);

        isFading = true;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            Color fadedColor = initialColor;
            fadedColor.a = alpha;
            mat.color = fadedColor;

            yield return null;
        }
        Destroy(gameObject);  // Clean up the trail after fade
    }
}
