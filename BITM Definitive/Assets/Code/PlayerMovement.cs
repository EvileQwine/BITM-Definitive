using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeedMulti = 20f;
    [SerializeField] float rotationSpeed = 25f;
    [SerializeField] float jumpForce = 35f;

    [SerializeField] float fallMultiplier = 8f;
    [SerializeField] float ascendingWeight = 6f;

    [SerializeField] float fullJumpTime = 0.3f;
    [SerializeField] float minJump = 0.2f;

    [SerializeField] LayerMask groundLayer;

    PlayerInputs playerInputs;
    InputAction MovementAction;
    Rigidbody rb;

    LOIndicationScript LOIScript;

    public bool isGrounded = false;
    bool jumpPressed;
    float jumpHeldTime;
    float raycastDistance;

    public Vector3 horMovement;
    void Awake()
    {
        playerInputs = new PlayerInputs();    
        LOIScript = FindFirstObjectByType<LOIndicationScript>();
        rb = GetComponent<Rigidbody>();
        raycastDistance = (GetComponent<CapsuleCollider>().height * transform.localScale.y / 2) + 0.2f;
        //RBFreeze(false);
    }
    void OnEnable()
    {
        MovementAction = playerInputs.Player.Move;
        playerInputs.Player.Jump.started += JumpStarted;
        playerInputs.Player.Jump.canceled += JumpEnded;
        playerInputs.Player.LockOn.started += OnLockOn;
        playerInputs.Player.LockOn.canceled += OffLockOn;

        playerInputs.Player.LockOn.Enable();
        playerInputs.Player.Jump.Enable();
        MovementAction.Enable();
    }
    void JumpStarted(InputAction.CallbackContext context)
    {
        jumpHeldTime = 0f;
        jumpPressed = true;
        if (isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
    }
    void JumpEnded(InputAction.CallbackContext context)
    {
        jumpPressed = false;
    }
    void OnLockOn(InputAction.CallbackContext context) { LOIScript.JustLockedOn(); }
    void OffLockOn(InputAction.CallbackContext context) { LOIScript.JustLockedOff(); }
    void OnDisable()
    {
        MovementAction.Disable();
        playerInputs.Player.LockOn.Disable();
        playerInputs.Player.Jump.Disable();
    }
    void Update()
    {
        GenerateMovement();
        CheckGrounded();
        CalculateJumpStuff();
    }
    void FixedUpdate()
    {
        RotatePlayer();
        MovePlayer();
        ApplyJumpPhysics();
    }
    void GenerateMovement()
    {
        Vector3 input = MovementAction.ReadValue<Vector2>();
        horMovement = (Quaternion.Euler(0, -90, 0) * Camera.main.transform.right) * input.y + Camera.main.transform.right * input.x;
    }
    void CheckGrounded()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, raycastDistance, groundLayer);
    }
    void CalculateJumpStuff()
    {
        if (jumpPressed)
        {
            jumpHeldTime += Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && jumpHeldTime < fullJumpTime && jumpHeldTime > minJump)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        }
        else if (jumpHeldTime < fullJumpTime)
        {
            jumpHeldTime += Time.deltaTime;
        }
    }
    void RotatePlayer()
    {
        if (LOIScript.LockedOn == LOstates.On)
        {
            //RBFreeze(false);
            Quaternion lookRotation = Quaternion.LookRotation((
                new Vector3(LOIScript.LockedOnEnemy.transform.position.x, transform.position.y, LOIScript.LockedOnEnemy.transform.position.z)
                - transform.position).normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
        else if (LOIScript.LockedOn == LOstates.Off)
        {
            if (horMovement != Vector3.zero)
            {
                //RBFreeze(false);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(horMovement), Time.deltaTime * rotationSpeed);
            }
            else
            {
                //RBFreeze(true);
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
    }
    void ApplyJumpPhysics()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.deltaTime;
        } 
        else if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * ascendingWeight * Time.deltaTime;
        }
    }
    /*void RBFreeze(bool okay)
    {
        if (okay)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.(FreezeRotationZ;
        }
    }*/
}
