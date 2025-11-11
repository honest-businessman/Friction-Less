using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Button3D : MonoBehaviour
{
    public UnityEvent onActivate;

    private MeshRenderer meshRenderer;
    private Material originalMaterial;

    private bool isActivated = false;



    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;
        originalMaterial.color = ScreenManager.Instance.colorButton;
    }
    private void OnMouseDown()
    {
        Press();
    }
    public void Press()
    {
        if(isActivated) return;
        originalMaterial.color = ScreenManager.Instance.colorButtonActive;
        StartCoroutine(Activate());
    }
    public void Select()
    {
        originalMaterial.color = ScreenManager.Instance.colorButtonHover;
    }
    public void Deselect()
    {
        originalMaterial.color = ScreenManager.Instance.colorButton;
    }
    private void OnMouseEnter()
    {
        if (isActivated) return;
        Select();
    }
    private void OnMouseExit()
    {
        if (isActivated) return;
        Deselect();
    }

    private IEnumerator Activate()
    {
        isActivated = true;
        onActivate.Invoke();
        yield return new WaitForSeconds(1f);
        originalMaterial.color = ScreenManager.Instance.colorButton;
        isActivated = false;
    }

}
