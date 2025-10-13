using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInputSystem inputSystem;

    [SerializeField] private PlayerController player;
    [SerializeField] private GameManager gameManager;

    private void Awake()
    {
        inputSystem = new PlayerInputSystem();
    }

    private void OnEnable()
    {
        inputSystem.Enable();

        // Global input
        inputSystem.UI.Pause.performed += ctx => gameManager.PauseInput();

        // Player input
        inputSystem.Player.Move.performed += ctx => player.Move(ctx.ReadValue<Vector2>());
        inputSystem.Player.Move.canceled += ctx => player.Move(Vector2.zero);
        inputSystem.Player.Drift.performed += ctx => player.Drift(true);
        inputSystem.Player.Drift.canceled += ctx => player.Drift(false);
        inputSystem.Player.Aim.performed += ctx => player.Aim(ctx.ReadValue<Vector2>());
        inputSystem.Player.Aim.canceled += ctx => player.Aim(Vector2.zero);
        inputSystem.Player.Fire.performed += ctx => player.Fire();
        inputSystem.Player.ChangeWeapon.performed += ctx => player.ChangeWeapon(ctx.ReadValue<float>());

        GameManager.OnPause.AddListener(OnUIPause);
        GameManager.OnUnpause.AddListener(OnUIUnpause);

    }

    private void OnDisable()
    {
        // Always clean up to avoid memory leaks
        inputSystem.Disable();

        inputSystem.UI.Pause.performed -= ctx => gameManager.PauseInput();

        inputSystem.Player.Move.performed -= ctx => player.Move(ctx.ReadValue<Vector2>());
        inputSystem.Player.Move.canceled -= ctx => player.Move(Vector2.zero);
        inputSystem.Player.Drift.performed -= ctx => player.Drift(true);
        inputSystem.Player.Drift.canceled -= ctx => player.Drift(false);
        inputSystem.Player.Aim.performed -= ctx => player.Aim(ctx.ReadValue<Vector2>());
        inputSystem.Player.Aim.canceled -= ctx => player.Aim(Vector2.zero);
        inputSystem.Player.Fire.performed -= ctx => player.Fire();
        inputSystem.Player.ChangeWeapon.performed -= ctx => player.ChangeWeapon(ctx.ReadValue<float>());

        GameManager.OnPause.RemoveListener(OnUIPause);
        GameManager.OnUnpause.RemoveListener(OnUIUnpause);
    }

    private void OnUIPause()
    {
        inputSystem.Player.Disable();
    }

    private void OnUIUnpause()
    {
        inputSystem.Player.Enable();
    }
}