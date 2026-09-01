using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeedMulti = 20f;

    PlayerInputs playerInputs;
    InputAction Movement;

    void Awake()
    {
        playerInputs = new PlayerInputs();    
    }
    void OnEnable()
    {
        Movement = playerInputs.Player.Move;
        Movement.Enable();
    }
    void OnDisable()
    {
        Movement.Disable();    
    }
    void FixedUpdate()
    {
        Vector3 input = Movement.ReadValue<Vector2>();
        Vector3 movement = transform.forward * input.y + transform.right * input.x;
        transform.position += moveSpeedMulti * Time.deltaTime * movement;
    }
}
