using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    public UnityEvent<InputValue> OnInputMove = new UnityEvent<InputValue>(); // Event for movement scripts to subscribe to
    [SerializeField]
    public UnityEvent<InputValue> OnInputDrift = new UnityEvent<InputValue>(); // Event for movement scripts to subscribe to
    [SerializeField]
    public UnityEvent<InputValue> OnInputFire = new UnityEvent<InputValue>(); // Event for firing scripts to subscribe to
    [SerializeField]
    public UnityEvent<InputValue> OnInputChangeWeapon = new UnityEvent<InputValue>(); // Event for weapon scripts to subscribe to
    private void OnMove(InputValue value)
    {
        OnInputMove?.Invoke(value); // Invoke event and subsribers when OnMove event is triggered
    }
    private void OnDrift(InputValue value)
    {
        OnInputDrift?.Invoke(value); // Invoke event and subsribers when OnDrift event is triggered
    }
    private void OnFire(InputValue value)
    {
        OnInputFire?.Invoke(value);
    }
    private void OnChangeWeapon(InputValue value)
    {
        OnInputChangeWeapon?.Invoke(value);
    }
}
