using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public enum RangedEquipped
{
    Axes,
    Gas,
}
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
    [SerializeField] GameObject axePrefab;

    bool gainingMeter = true;
    bool canShoot = true;
    bool isShooting = false;

    public RangedEquipped rEquipped = RangedEquipped.Gas;

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
        playerInputs.Player.SwitchRanged.performed += SwitchedRanged;

        playerInputs.Player.Ability.Enable();
        playerInputs.Player.Shoot.Enable();
        playerInputs.Player.SwitchRanged.Enable();
    }
    void OnDisable()
    {
        playerInputs.Player.Ability.Disable();
        playerInputs.Player.Shoot.Disable();
        playerInputs.Player.SwitchRanged.Disable();
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
    void SwitchedRanged(InputAction.CallbackContext context)
    {
        PHScript.SwapRanged();
        PHScript.ShowBars();
        if (rEquipped == RangedEquipped.Axes)
        {
            rEquipped = RangedEquipped.Gas;
            return;
        }
        rEquipped = RangedEquipped.Axes;
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
    void ShootPressed(InputAction.CallbackContext context)
    {
        if (rEquipped == RangedEquipped.Gas)
        {
            ShootGas();
        }
        else
        {
            ThrowAxes();
        }
    }
    void ShootGas()
    {
        isShooting = true;
        if (PMScript.canMove && PMScript.canAttack && canShoot)
        {
            if (LOIScript.LockedOn != LOstates.Off)
            {
                if (Vector3.Dot(PMScript.horMovement, transform.forward) > 0.8f && canShoot)
                {
                    StartCoroutine(PMScript.DisableAttacks(0.4f));
                    StartCoroutine(PMScript.DisableMovement(0.2f));
                    StartCoroutine(DisableShoot(shootDelay + 0.2f));
                    GameObject can = Instantiate(canPrefab, transform.position, transform.rotation);
                    can.GetComponent<CanScript>().Launch((transform.forward * 20) + (transform.up * 5));
                    isShooting = false;
                    return;
                }
                else if (Vector3.Dot(PMScript.horMovement, transform.forward) < -0.8f)
                {
                    StartCoroutine(PMScript.DisableAttacks(0.3f));
                    StartCoroutine(PMScript.DisableMovement(0.2f));
                    StartCoroutine(DisableShoot(shootDelay + 0.4f));
                    GameObject match = Instantiate(matchPrefab, transform.position, transform.rotation);
                    match.GetComponent<MatchScript>().Launch((transform.forward * 15) + (transform.up * 5));
                    isShooting = false;
                    return;
                }
                CreateGasCloud(transform.forward.normalized);
                return;
            }
            if (PMScript.horMovement != Vector3.zero)
            {
                CreateGasCloud(PMScript.horMovement);
                return;
            }
            CreateGasCloud(transform.forward.normalized);
        }
    }
    void ShootCancelled(InputAction.CallbackContext context) { isShooting = false; }
    void ThrowAxes()
    {
        if (PMScript.canMove && PMScript.canAttack && canShoot)
        {
            if (LOIScript.LockedOn == LOstates.On)
            {
                if (Vector3.Dot(PMScript.horMovement, transform.forward) > 0.8f && canShoot)
                {
                    //forward
                    return;
                }
                else if (Vector3.Dot(PMScript.horMovement, transform.forward) < -0.8f)
                {
                    //backward
                    return;
                }
                //direction based
                return;
            }
            else if (LOIScript.LockedOn == LOstates.None)
            {
                //don't use enemy position
                if (Vector3.Dot(PMScript.horMovement, transform.forward) > 0.8f && canShoot)
                {
                    //forward
                    return;
                }
                else if (Vector3.Dot(PMScript.horMovement, transform.forward) < -0.8f)
                {
                    //backward
                    return;
                }
                return;
            }
            StartCoroutine(PMScript.DisableAttacks(0.3f));
            StartCoroutine(PMScript.DisableMovement(0.2f));
            StartCoroutine(DisableShoot(shootDelay + 0.3f));
            GameObject axe = Instantiate(axePrefab, transform.position, transform.rotation);
            axe.GetComponent<ThrownHatchet>().player = gameObject;
            //axe.GetComponent<ThrownHatchet>().FrontSpin(LOIScript.FindClosest(gameObject.transform).transform.position );
        }
    }
    void WhileShooting()
    {
        if (PMScript.canMove && PMScript.canAttack && canShoot && rEquipped == RangedEquipped.Gas)
        {
            if (LOIScript.LockedOn != LOstates.Off)
            {
                if (Vector3.Dot(PMScript.horMovement, transform.forward) > 0.8f) { return; }
                else if (Vector3.Dot(PMScript.horMovement, transform.forward) < -0.8f) { return; }
            }
            if (PMScript.horMovement != Vector3.zero)
            {
                CreateGasCloud(PMScript.horMovement);
                return;
            }
            CreateGasCloud(transform.forward.normalized);
        }
    }
    void CreateGasCloud(Vector3 direction)
    {
        Quaternion q = UnityEngine.Random.rotation;
        q.x = 0;
        q.z = 0;
        Instantiate(gasPrefab, transform.position + (direction * GasShootDistance), q);
        StartCoroutine(DisableShoot(shootDelay));
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
