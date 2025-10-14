using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInputSystem inputSystem;

    [SerializeField]
    private PlayerController player;
    public PlayerController Player { get; set; }
    public GameManager GameManager { get; set; }

    private void Awake()
    {
        inputSystem = new PlayerInputSystem();
    }


    private void OnEnable()
    {
        inputSystem.Enable();

        // Global input
        inputSystem.UI.Pause.performed += ctx => GameManager.PauseRecieve();

        // Player input
        inputSystem.Player.Move.performed += ctx => Player.Move(ctx.ReadValue<Vector2>());
        inputSystem.Player.Move.canceled += ctx => Player.Move(Vector2.zero);
        inputSystem.Player.Drift.performed += ctx => Player.Drift(true);
        inputSystem.Player.Drift.canceled += ctx => Player.Drift(false);
        inputSystem.Player.Aim.performed += ctx => Player.Aim(ctx.ReadValue<Vector2>());
        inputSystem.Player.Aim.canceled += ctx => Player.Aim(Vector2.zero);
        inputSystem.Player.Fire.performed += ctx => Player.Fire();
        inputSystem.Player.ChangeWeapon.performed += ctx => Player.ChangeWeapon(ctx.ReadValue<float>());

        GameManager.OnPause.AddListener(OnUIPause);
        GameManager.OnUnpause.AddListener(OnUIUnpause);

    }

    private void OnDisable()
    {
        // Always clean up to avoid memory leaks
        inputSystem.Disable();

        inputSystem.UI.Pause.performed -= ctx => GameManager.PauseRecieve();

        inputSystem.Player.Move.performed -= ctx => Player.Move(ctx.ReadValue<Vector2>());
        inputSystem.Player.Move.canceled -= ctx => Player.Move(Vector2.zero);
        inputSystem.Player.Drift.performed -= ctx => Player.Drift(true);
        inputSystem.Player.Drift.canceled -= ctx => Player.Drift(false);
        inputSystem.Player.Aim.performed -= ctx => Player.Aim(ctx.ReadValue<Vector2>());
        inputSystem.Player.Aim.canceled -= ctx => Player.Aim(Vector2.zero);
        inputSystem.Player.Fire.performed -= ctx => Player.Fire();
        inputSystem.Player.ChangeWeapon.performed -= ctx => Player.ChangeWeapon(ctx.ReadValue<float>());

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