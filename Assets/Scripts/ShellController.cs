using UnityEditor;
using UnityEngine;

public class ShellController : MonoBehaviour
{
    [SerializeField]
    private int shellBounces = 0;
    [SerializeField]
    private float shellLifetime = 10f;

    private Vector3 direction;
    [SerializeField]
    private Rigidbody rb;

    public void Initialize(int shellBounces, float shellLifetime)
    {
        this.shellBounces = shellBounces;
        this.shellLifetime = shellLifetime;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        Destroy(gameObject, shellLifetime);
    }


}
