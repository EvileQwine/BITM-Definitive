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

    Vector3 horMovement;

    void Awake()
    {
        playerInputs = new PlayerInputs();    
    }
    void OnEnable()
    {
        MovementAction = playerInputs.Player.Move;
        MovementAction.Enable();
    }
    void OnDisable()
    {
        MovementAction.Disable();    
    }
    void FixedUpdate()
    {
        Vector3 input = MovementAction.ReadValue<Vector2>();
        horMovement = (Quaternion.Euler(0, -90, 0) * Camera.main.transform.right) * input.y + Camera.main.transform.right * input.x;
        if (horMovement != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(horMovement),Time.deltaTime * rotationSpeed);
        }
        transform.position += moveSpeedMulti * Time.deltaTime * horMovement;
    }
}
