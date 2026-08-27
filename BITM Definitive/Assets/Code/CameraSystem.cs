using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] int moveSpeed = 20;
    [SerializeField] int rotateSpeed = 100;
    [SerializeField] int zoomSpeed = 1000;
    [SerializeField] int edgeScroll = 20;
    [SerializeField] float dragSpeed = 2f;
    [SerializeField] float RightDragSpeed = 2f;
    [SerializeField] float downRotateSpeed = 5f;

    [SerializeField] int[] FOVconstraints = new int[2] { 5, 100 };
    [SerializeField] int[] YConstraints = new int[2] { 1, 60 };

    [SerializeField] bool useEdgeScrolling = false;
    [SerializeField] bool useDragging = true;

    [SerializeField] CinemachineFollow followCam;
    Vector2 lastMousePos = Vector2.zero;
    Vector3 targetOffset;
    float targetFov;
    bool middleMouseDown = false;
    bool rightMouseDown = false;
    void Awake()
    {
        targetOffset = followCam.GetComponent<CinemachineFollow>().FollowOffset;
        targetFov = followCam.GetComponent<CinemachineCamera>().Lens.FieldOfView;
        
    }
    void Update()
    {
        Movement();
        if (useDragging)
        {
            Dragging();
        }
        if (useEdgeScrolling)
        {
            EdgeScrolling();
        }
        Rotation();
        //CameraZoomFOV();
        CameraZoomMovement();
        RightRotation();
    }
    void EdgeScrolling()
    {
        Vector3 input = Vector3.zero;
        if (useEdgeScrolling)
        {
            if (Input.mousePosition.x < edgeScroll) input.x -= 1f;
            if (Input.mousePosition.y < edgeScroll) input.z -= 1f;
            if (Input.mousePosition.x > Screen.width - edgeScroll) input.x += 1f;
            if (Input.mousePosition.y > Screen.height - edgeScroll) input.z += 1f;
        }
        Vector3 movement = transform.forward * input.z + transform.right * input.x;
        transform.position += moveSpeed * Time.deltaTime * movement;
    }
    void Dragging()
    {
        Vector3 input = Vector3.zero;
        if (Input.GetMouseButtonDown(2))
        {
            middleMouseDown = true;
            lastMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(2))
        {
            middleMouseDown = false;
        }
        if (middleMouseDown)
        {
            Vector2 mouseMovement = (Vector2)Input.mousePosition - lastMousePos;
            input.x = mouseMovement.x / -dragSpeed;
            input.z = mouseMovement.y / -dragSpeed;
            lastMousePos = Input.mousePosition;
        }
        Vector3 movement = transform.forward * input.z + transform.right * input.x;
        transform.position += moveSpeed * Time.deltaTime * movement;
    }
    void Movement()
    {
        Vector3 input = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) input.z += 1f;
        if (Input.GetKey(KeyCode.S)) input.z -= 1f;
        if (Input.GetKey(KeyCode.A)) input.x -= 1f;
        if (Input.GetKey(KeyCode.D)) input.x += 1f;
        if (Input.GetKey(KeyCode.LeftShift)) input.y += 1f;
        if (Input.GetKey(KeyCode.LeftControl)) input.y -= 1f;
        Vector3 movement = transform.forward * input.z + transform.right * input.x + transform.up * input.y;
        transform.position += moveSpeed * Time.deltaTime * movement;
    }
    void Rotation()
    {
        float rotateInput = 0f;
        if (Input.GetKey(KeyCode.Q)) rotateInput += 1f;
        if (Input.GetKey(KeyCode.E)) rotateInput -= 1f;
        transform.eulerAngles += new Vector3(0, rotateInput * Time.deltaTime * rotateSpeed, 0);
    }
    void CameraZoomFOV()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            targetFov -= 5;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            targetFov += 5;
        }
        targetFov = Mathf.Clamp(targetFov, FOVconstraints[0], FOVconstraints[1]);
        followCam.GetComponent<CinemachineCamera>().Lens.FieldOfView =
            Mathf.Lerp(followCam.GetComponent<CinemachineCamera>().Lens.FieldOfView, targetFov, Time.deltaTime * zoomSpeed);
    }
    void CameraZoomMovement()
    {
        int input = 0;
        if (Input.mouseScrollDelta.y > 0) input -= 1;
        if (Input.mouseScrollDelta.y < 0) input += 1;
        Vector3 movement = (followCam.transform.position - transform.position).normalized;
        transform.position += zoomSpeed * Time.deltaTime * movement * input;
    }
    void RightRotation()
    {
        Vector3 input = Vector3.zero;
        float rotateInput = 0f;
        if (Input.GetMouseButtonDown(1))
        {
            rightMouseDown = true;
            lastMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(1))
        {
            rightMouseDown = false;
        }
        if (rightMouseDown)
        {
            Vector2 mouseMovement = (Vector2)Input.mousePosition - lastMousePos;
            rotateInput = mouseMovement.x;
            targetOffset.y += mouseMovement.y / -RightDragSpeed;
            lastMousePos = Input.mousePosition;
            if (targetOffset.y != 0)
            {
                targetOffset.y = Mathf.Clamp(targetOffset.y, YConstraints[0], YConstraints[1]);
                followCam.GetComponent<CinemachineFollow>().FollowOffset =
                    Vector3.Lerp(followCam.GetComponent<CinemachineFollow>().FollowOffset, targetOffset, Time.deltaTime * downRotateSpeed);
            }
            transform.eulerAngles += new Vector3(0, rotateInput * Time.deltaTime * rotateSpeed, 0);
        }
    }
}
