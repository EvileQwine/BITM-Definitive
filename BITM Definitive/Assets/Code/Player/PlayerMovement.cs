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

    [SerializeField] LayerMask groundLayer;

    PlayerInputs playerInputs;
    InputAction MovementAction;
    PlayerAbilities moveset;
    Rigidbody rb;

    LOIndicationScript LOIScript;

    public bool canMove = true;

    bool isGrounded = false;
    bool jumpPressed;
    float jumpHeldTime;
    float raycastDistance;

    public Vector3 horMovement;
    void Awake()
    {
        playerInputs = new PlayerInputs();
        rb = GetComponent<Rigidbody>();
        moveset = GetComponent<PlayerAbilities>();
    }
    void Start()
    {
        LOIScript = FindFirstObjectByType<LOIndicationScript>();
        raycastDistance = (GetComponent<CapsuleCollider>().height * transform.localScale.y / 2) + 0.2f;
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
        horMovement = (Quaternion.Euler(0, -90, 0) * Camera.main.transform.right * input.y + Camera.main.transform.right * input.x);
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
            targetVelocity = lockOnSpeedReductionMultiplier * moveSpeedMulti * horMovement;
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
            rb.linearVelocity += fallMultiplier * Physics.gravity.y * Time.deltaTime * Vector3.up;
        } 
        else if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += ascendingWeight * Physics.gravity.y * Time.deltaTime * Vector3.up;
        }
    }
    public IEnumerator DisableMovement(float f)
    {
        canMove = false;
        yield return new WaitForSeconds(f);
        canMove = true;
    }
}
