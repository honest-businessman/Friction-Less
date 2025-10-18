using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.XR;

public abstract class CharacterBase : MonoBehaviour
{
    [SerializeField] protected float driveCharge;
    public float DriveCharge { get => driveCharge; set => driveCharge = value; }

    public void DrainDrive()
    {
        DriveCharge = 0;
    }
    
}
