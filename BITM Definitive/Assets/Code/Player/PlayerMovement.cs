using System;
using System.Collections;
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

    [SerializeField] float lockOnSpeedReductionMultiplier = 0.7f;

    [SerializeField] float dashStrength = 80f;
    [SerializeField] float dashTime = 0.2f;

    [SerializeField] LayerMask groundLayer;

    PlayerInputs playerInputs;
    InputAction MovementAction;
    Rigidbody rb;

    LOIndicationScript LOIScript;

    [SerializeField] bool canMove = true;

    bool isGrounded = false;
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
    }
    void OnEnable()
    {
        MovementAction = playerInputs.Player.Move;
        playerInputs.Player.Jump.started += JumpStarted;
        playerInputs.Player.Jump.canceled += JumpEnded;
        playerInputs.Player.Ability.performed += AbilityPressed;
        playerInputs.Player.LockOn.started += OnLockOn;
        playerInputs.Player.LockOn.canceled += OffLockOn;

        playerInputs.Player.LockOn.Enable();
        playerInputs.Player.Jump.Enable();
        playerInputs.Player.Ability.Enable();
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
    private void AbilityPressed(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            if (LOIScript.LockedOn != LOstates.Off)
            {
                if (Vector3.Dot(horMovement, transform.forward) > 0.8f)
                {
                    //teleport or something
                    return;
                }
                else
                {
                    if (horMovement != Vector3.zero)
                    {
                        Dash(horMovement);
                        return;
                    }
                }
            }
            Dash(transform.forward);
        }
    }
    void OnLockOn(InputAction.CallbackContext context) { LOIScript.JustLockedOn(); }
    void OffLockOn(InputAction.CallbackContext context) { LOIScript.JustLockedOff(); }
    void OnDisable()
    {
        MovementAction.Disable();
        playerInputs.Player.LockOn.Disable();
        playerInputs.Player.Jump.Disable();
        playerInputs.Player.Ability.Disable();
    }
    void Update()
    {
        GenerateMovement();
        CheckGrounded();
        CalculateJumpStuff();
    }
    void FixedUpdate()
    {
        if (canMove)
        {
            RotatePlayer();
            MovePlayer();
            ApplyJumpPhysics();
        }
    }
    void GenerateMovement()
    {
        Vector3 input = MovementAction.ReadValue<Vector2>();
        horMovement = (Quaternion.Euler(0, -90, 0) * Camera.main.transform.right * input.y + Camera.main.transform.right * input.x).normalized;
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
            Quaternion lookRotation = Quaternion.LookRotation((
                new Vector3(LOIScript.LockedOnEnemy.transform.position.x, transform.position.y, LOIScript.LockedOnEnemy.transform.position.z)
                - transform.position).normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
        else if (LOIScript.LockedOn == LOstates.Off)
        {
            if (horMovement != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(horMovement), Time.deltaTime * rotationSpeed);
            }
        }
    }
    void MovePlayer()
    {
        Vector3 targetVelocity;
        if (LOIScript.LockedOn == LOstates.Off)
        {
            targetVelocity = horMovement * moveSpeedMulti;
        }
        else
        {
            targetVelocity = horMovement * moveSpeedMulti * lockOnSpeedReductionMultiplier;
        }
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
    IEnumerator DisableMovement(float f)
    {
        canMove = false;
        yield return new WaitForSeconds(f);
        canMove = true;
    }
    void Dash(Vector3 direction)
    {
        direction *= dashStrength;
        StartCoroutine(DisableMovement(dashTime));
        Vector3 velocity = rb.linearVelocity;
        velocity.x = direction.x;
        velocity.z = direction.z;
        rb.linearVelocity = velocity;
    }
}
