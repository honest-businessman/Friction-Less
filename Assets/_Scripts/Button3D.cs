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
        originalMaterial.color = MenuManager.Instance.colorButton;
    }

    private void OnMouseDown()
    {
        if(isActivated) return;
        Debug.Log("Mouse Down");
        originalMaterial.color = MenuManager.Instance.colorButtonActive;
        StartCoroutine(Activate());
    }
    private void OnMouseEnter()
    {
        if (isActivated) return;
        Debug.Log("Mouse Enter");
        originalMaterial.color = MenuManager.Instance.colorButtonHover;
    }
    private void OnMouseExit()
    {
        if (isActivated) return;
        Debug.Log("Mouse Exit");
        originalMaterial.color = MenuManager.Instance.colorButton;
    }

    private IEnumerator Activate()
    {
        isActivated = true;
        onActivate.Invoke();
        yield return new WaitForSeconds(1f);
        originalMaterial.color = MenuManager.Instance.colorButton;
        isActivated = false;
    }

}
