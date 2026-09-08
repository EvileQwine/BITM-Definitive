using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeedMulti = 20f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] GameObject CameraFollow;
    //[SerializeField] LayerMask groundLayer;

    PlayerInputs playerInputs;
    InputAction MovementAction;
    Rigidbody rb;

    LOIndicationScript LOIScript;

    public Vector3 horMovement;
    //bool isGrounded = false;

    void Awake()
    {
        playerInputs = new PlayerInputs();    
        LOIScript = FindFirstObjectByType<LOIndicationScript>();
        rb = GetComponent<Rigidbody>();
        RBFreeze(false);
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
    void Update()
    {
        GenerateMovement();
    }
    void FixedUpdate()
    {
        RotatePlayer();
        MovePlayer();
    }
    void GenerateMovement()
    {
        Vector3 input = MovementAction.ReadValue<Vector2>();
        horMovement = (Quaternion.Euler(0, -90, 0) * Camera.main.transform.right) * input.y + Camera.main.transform.right * input.x;
    }
    void RotatePlayer()
    {
        if (LOIScript.LockedOn == LOstates.On)
        {
            RBFreeze(false);
            Quaternion lookRotation = Quaternion.LookRotation((
                new Vector3(LOIScript.LockedOnEnemy.transform.position.x, transform.position.y, LOIScript.LockedOnEnemy.transform.position.z)
                - transform.position).normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
        else if (LOIScript.LockedOn == LOstates.Off)
        {
            if (horMovement != Vector3.zero)
            {
                RBFreeze(false);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(horMovement), Time.deltaTime * rotationSpeed);
            }
            else
            {
                RBFreeze(true);
            }
        }
    }
    void MovePlayer()
    {
        Vector3 targetVelocity = horMovement * moveSpeedMulti;
        Vector3 velocity = rb.linearVelocity;
        velocity.x = targetVelocity.x;
        velocity.z = targetVelocity.z;
        rb.linearVelocity = velocity;

        //rb.position += moveSpeedMulti * Time.deltaTime * horMovement;
    }
    void RBFreeze(bool okay)
    {
        if (okay)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }
}
