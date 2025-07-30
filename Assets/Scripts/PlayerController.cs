using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    [SerializeField] private float pMoveSpeed = 3f;
    [SerializeField] private float pRotSpeed = 40f;
    private Vector2 playerInput;
    private Vector3 velocity;
    private Rigidbody rb;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnMove(InputValue value)
    {
        playerInput = value.Get<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        controller.Move(transform.up * playerInput.y * pMoveSpeed * Time.deltaTime);
        transform.Rotate(transform.forward * -playerInput.x * pRotSpeed * Time.deltaTime);
    }
}
