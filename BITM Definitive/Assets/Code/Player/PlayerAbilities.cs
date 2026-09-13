using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{
    PlayerInputs playerInputs;
    InputAction Shooting;
    PlayerMovement PMScript;
    PlayerHealthScript PHScript;
    LOIndicationScript LOIScript;
    Rigidbody rb;

    [SerializeField] float meterRegain = 3f;

    [SerializeField] float dashStrength = 80f;
    [SerializeField] float dashTime = 0.2f;

    [SerializeField] float GasShootDistance = 4f;
    [SerializeField] float shootDelay = 0.3f;

    [SerializeField] GameObject gasPrefab;

    bool gainingMeter = true;
    bool canShoot = true;

    public float maxPlantMeter = 100;
    public float curPlantMeter = 100;

    void Awake()
    {
        playerInputs = new PlayerInputs();
        PMScript = GetComponent<PlayerMovement>();
        PHScript = GetComponent<PlayerHealthScript>();
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        LOIScript = FindFirstObjectByType<LOIndicationScript>();
    }
    public float MeterFill()
    {
        return curPlantMeter / maxPlantMeter;
    }
    void OnEnable()
    {
        playerInputs.Player.Ability.started += AbilityPressed;
        Shooting = playerInputs.Player.Shoot;

        playerInputs.Player.Ability.Enable();
        Shooting.Enable();
    }
    void OnDisable()
    {
        playerInputs.Player.Ability.Disable();
        Shooting.Disable();
    }
    void Update()
    {
        if (gainingMeter && curPlantMeter < maxPlantMeter)
        {
            curPlantMeter += Time.deltaTime * meterRegain;
        }
        if (Shooting.ReadValue<float>() > 0)
        {
            WhileShooting();
        }
    }
    void AbilityPressed(InputAction.CallbackContext context)
    {
        if (PMScript.canMove && curPlantMeter >= 5)
        {
            PHScript.ShowBars();
            if (LOIScript.LockedOn != LOstates.Off)
            {
                if (Vector3.Dot(PMScript.horMovement, transform.forward) > 0.8f && curPlantMeter >= 10)
                {
                    Debug.Log("ForwardInput");
                    curPlantMeter -= 10;
                    return;
                }
                else if (Vector3.Dot(PMScript.horMovement, transform.forward) < -0.8f && curPlantMeter >= 10)
                {
                    Debug.Log("BackwardsInput");
                    curPlantMeter -= 10;
                    return;
                }
            }
            if (PMScript.horMovement != Vector3.zero)
            {
                Dash(PMScript.horMovement.normalized);
                curPlantMeter -= 5;
                return;
            }
            Dash(transform.forward);
            curPlantMeter -= 5;
        }
    }
    void WhileShooting()
    {
        if (PMScript.canMove && canShoot)
        {
            StartCoroutine(DisableShoot());
            if (LOIScript.LockedOn != LOstates.Off)
            {
                if (Vector3.Dot(PMScript.horMovement, transform.forward) > 0.8f && curPlantMeter >= 10)
                {
                    Debug.Log("ForwardInput");
                    return;
                }
                else if (Vector3.Dot(PMScript.horMovement, transform.forward) < -0.8f && curPlantMeter >= 10)
                {
                    Debug.Log("BackwardsInput");
                    return;
                }
            }
            if (PMScript.horMovement != Vector3.zero)
            {
                Instantiate(gasPrefab, transform.position + (PMScript.horMovement * GasShootDistance), UnityEngine.Random.rotation);
                return;
            }
            Instantiate(gasPrefab, transform.position + (transform.forward.normalized * GasShootDistance), UnityEngine.Random.rotation);
        }
    }
    void Dash(Vector3 direction)
    {
        direction *= dashStrength;
        StartCoroutine(PMScript.DisableMovement(dashTime));
        StartCoroutine(DisableMeterGain(dashTime + 0.2f));
        Vector3 velocity = rb.linearVelocity;
        velocity.x = direction.x;
        velocity.z = direction.z;
        rb.linearVelocity = velocity;
    }
    IEnumerator DisableMeterGain(float f)
    {
        gainingMeter = false;
        yield return new WaitForSeconds(f);
        gainingMeter = true;
    }
    IEnumerator DisableShoot()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootDelay);
        canShoot = true;
    }
}
