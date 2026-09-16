using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{
    PlayerInputs playerInputs;
    PlayerMovement PMScript;
    PlayerHealthScript PHScript;
    LOIndicationScript LOIScript;
    Rigidbody rb;

    [SerializeField] float meterRegain = 3f;

    [SerializeField] float dashStrength = 80f;
    [SerializeField] float dashTime = 0.2f;

    [SerializeField] float GasShootDistance = 4f;
    [SerializeField] float shootDelay = 0.1f;

    [SerializeField] GameObject gasPrefab;
    [SerializeField] GameObject matchPrefab;
    [SerializeField] GameObject canPrefab;

    bool gainingMeter = true;
    bool canShoot = true;
    bool isShooting = false;

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
        playerInputs.Player.Shoot.started += ShootPressed;
        playerInputs.Player.Shoot.canceled += ShootCancelled;

        playerInputs.Player.Ability.Enable();
        playerInputs.Player.Shoot.Enable();
    }
    void ShootPressed(InputAction.CallbackContext context)
    {
        isShooting = true;
        if (PMScript.canMove && canShoot)
        {
            if (LOIScript.LockedOn != LOstates.Off)
            {
                if (Vector3.Dot(PMScript.horMovement, transform.forward) > 0.8f)
                {
                    StartCoroutine(PMScript.DisableMovement(0.4f));
                    StartCoroutine(DisableShoot(shootDelay + 0.8f));
                    GameObject can = Instantiate(canPrefab, transform.position, transform.rotation);
                    can.GetComponent<CanScript>().Launch((transform.forward * 20) + (transform.up * 5));
                    isShooting = false;
                    return;
                }
                else if (Vector3.Dot(PMScript.horMovement, transform.forward) < -0.8f)
                {
                    StartCoroutine(PMScript.DisableMovement(0.3f));
                    StartCoroutine(DisableShoot(shootDelay + 0.4f));
                    GameObject match = Instantiate(matchPrefab, transform.position, transform.rotation);
                    match.GetComponent<MatchScript>().Launch((transform.forward * 15) + (transform.up * 5));
                    isShooting = false;
                    return;
                }
            }
            if (PMScript.horMovement != Vector3.zero)
            {
                Instantiate(gasPrefab, transform.position + (PMScript.horMovement * GasShootDistance), UnityEngine.Random.rotation);
                StartCoroutine(DisableShoot(shootDelay));
                return;
            }
            Instantiate(gasPrefab, transform.position + (transform.forward.normalized * GasShootDistance), UnityEngine.Random.rotation);
            StartCoroutine(DisableShoot(shootDelay));
        }
    }
    void ShootCancelled(InputAction.CallbackContext context) { isShooting = false; }
    void OnDisable()
    {
        playerInputs.Player.Ability.Disable();
        playerInputs.Player.Shoot.Disable();
    }
    void Update()
    {
        if (gainingMeter && curPlantMeter < maxPlantMeter)
        {
            curPlantMeter += Time.deltaTime * meterRegain;
        }
        if (isShooting)
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
            if (PMScript.horMovement != Vector3.zero)
            {
                Instantiate(gasPrefab, transform.position + (PMScript.horMovement * GasShootDistance), UnityEngine.Random.rotation);
                StartCoroutine(DisableShoot(shootDelay));
                return;
            }
            Instantiate(gasPrefab, transform.position + (transform.forward.normalized * GasShootDistance), UnityEngine.Random.rotation);
            StartCoroutine(DisableShoot(shootDelay));
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
    IEnumerator DisableShoot(float f)
    {
        canShoot = false;
        yield return new WaitForSeconds(f);
        canShoot = true;
    }
}
