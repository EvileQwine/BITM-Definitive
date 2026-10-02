using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    static bool gamePaused = false;

    CameraSystem CSScript;
    ManagerInputs ManagerInputs;

    void Awake()
    {
        ManagerInputs = new ManagerInputs();
    }
    void Start()
    {
        CSScript = FindAnyObjectByType<CameraSystem>();
    }
    void OnEnable()
    {
        ManagerInputs.MgmtActions.Pause.Enable();
        ManagerInputs.MgmtActions.Pause.performed += PausePerformed;
    }
    void OnDisable()
    {
        ManagerInputs.MgmtActions.Pause.Disable();
    }
    public static bool IsGamePaused()
    {
        return gamePaused;
    }
    void PausePerformed(InputAction.CallbackContext context)
    {
        if (!gamePaused)
        {
            Time.timeScale = 0f;
            gamePaused = true;
            FindAnyObjectByType<PlayerMovement>().enabled = false;
            FindAnyObjectByType<PlayerAbilities>().enabled = false;
            CSScript.CameraBlurred(true);
            CSScript.enabled = false;
        }
        else
        {
            Time.timeScale = 1f;
            gamePaused = false;
            FindAnyObjectByType<PlayerMovement>().enabled = true;
            FindAnyObjectByType<PlayerAbilities>().enabled = true;
            CSScript.enabled = true;
            CSScript.CameraBlurred(false);
        }
    }
}
