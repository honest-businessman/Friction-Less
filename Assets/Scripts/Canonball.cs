using UnityEngine;

public class Canonball : MonoBehaviour
{
    [SerializeField]
    public float canonSpeed = 5f;
    private Vector2 direction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * canonSpeed * Time.deltaTime);
        
    }
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }
}
