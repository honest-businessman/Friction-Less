using UnityEditor.Rendering;
using UnityEngine;

public class TrailFader : MonoBehaviour
{
    [SerializeField]
    private Material mat;

    TrailRenderer tr;
    float timer = 0;
    float startAlpha = 1f;
    float endAlpha = 0f;


    private void Awake()
    {
        tr = GetComponent<TrailRenderer>();
        timer = Time.time;
    }


    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / tr.time;
        t = Mathf.Clamp01(t);
        float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, t);
        Color editedColor = tr.material.color;
        editedColor.a = currentAlpha;
    }
}
