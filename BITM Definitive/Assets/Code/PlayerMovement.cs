using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeedMulti = 20f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] GameObject CameraFollow;

    PlayerInputs playerInputs;
    InputAction MovementAction;

    LOIndicationScript LOIScript;

    public Vector3 horMovement;

    void Awake()
    {
        playerInputs = new PlayerInputs();    
        LOIScript = FindFirstObjectByType<LOIndicationScript>();
    }
    void OnEnable()
    {
        MovementAction = playerInputs.Player.Move;
        playerInputs.Player.LockOn.started += OnLockOn;
        playerInputs.Player.LockOn.canceled += OffLockOn;

        playerInputs.Player.LockOn.Enable();
        MovementAction.Enable();
    }
    private void OnLockOn(InputAction.CallbackContext context)
    {
        LOIScript.JustLockedOn();
    }
    private void OffLockOn(InputAction.CallbackContext context)
    {
        LOIScript.JustLockedOff();
    }
    void OnDisable()
    {
        MovementAction.Disable();
        playerInputs.Player.LockOn.Disable();
    }
    void FixedUpdate()
    {
        MovementAndRotation();
    }
    void MovementAndRotation()
    {
        Vector3 input = MovementAction.ReadValue<Vector2>();
        horMovement = (Quaternion.Euler(0, -90, 0) * Camera.main.transform.right) * input.y + Camera.main.transform.right * input.x;
        if (LOIScript.LockedOn == LOstates.On)
        {
            Quaternion lookRotation = Quaternion.LookRotation((LOIScript.LockedOnEnemy.transform.position - transform.position).normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
        else if (LOIScript.LockedOn == LOstates.Off)
        {
            if (horMovement != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(horMovement), Time.deltaTime * rotationSpeed);
            }
        }
        transform.position += moveSpeedMulti * Time.deltaTime * horMovement;
    }
}
