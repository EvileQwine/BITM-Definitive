using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float maxHealth;
    public float curHealth;
    public bool canDie = true;
    public float comboCount = 0;
    public bool isGrounded;

    [SerializeField] LayerMask groundLayer;

    float raycastDistance;
    bool justExploded = false;
    bool checkingGrounded = true;

    Collider col;
    Rigidbody rb;

    MMRScript mmrscript;

    bool insideGas = false;
    void Awake()
    {
        col = GetComponent<Collider>();    
        rb = GetComponent<Rigidbody>();
        mmrscript = GetComponent<MMRScript>();
    }
    void Start()
    {
        raycastDistance = (GetComponent<BoxCollider>().size.y * transform.localScale.y / 2) + 0.3f;
    }
    void Update()
    {
        if (curHealth <= 0 && canDie)
        {
            FindAnyObjectByType<LOIndicationScript>().UpdateLockOn(gameObject);
        }
        if (checkingGrounded)
        {
            Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
            if (!Physics.Raycast(rayOrigin, Vector3.down, raycastDistance, groundLayer))
            {
                StartCoroutine(CoyoteTime());
                StartCoroutine(DontCheckGrounded());
            }
            else { isGrounded = true; }
            if (isGrounded && rb.linearVelocity.y == 0)
            {
                comboCount = 0;
            }
        }
    }
    private void FixedUpdate()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += 3 * Physics.gravity.y * Time.deltaTime * Vector3.up;
        }
        else if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += 3 * Physics.gravity.y * Time.deltaTime * Vector3.up;
        }
        if (insideGas)
        {
            curHealth -= 0.1f;
        }
    }
    public void RemoveHealth(float f)
    {
        comboCount += 1;
        curHealth -= f;
    }
    public float HealthPercent()
    {
        if (!canDie)
        {
            return 1;
        }
        return curHealth / maxHealth;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Matchstick"))
        {
            WasHit("Match");
        }
        if (other.gameObject.CompareTag("GasCloud") && !justExploded)
        {
            insideGas = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("GasCloud"))
        {
            insideGas = false;
        }
    }
    public void ShouldExplode()
    {
        if (insideGas && !justExploded)
        {
            insideGas = false;
            WasHit("GasExplosion");
            StartCoroutine(NoGas());
        }
    }
    IEnumerator NoGas()
    {
        justExploded = true;
        yield return new WaitForSeconds(0.2f);
        justExploded = false;
    }
    public void WasHit(string attackname)
    {
        //Debug.Log(attackname);
        //transform.LookAt(new Vector3(transform.position.x, FindAnyObjectByType<PlayerMovement>().gameObject.transform.position.y, transform.position.z));
        Quaternion lookRotation = Quaternion.LookRotation((new Vector3(FindAnyObjectByType<PlayerMovement>().gameObject.transform.position.x,
            transform.position.y, FindAnyObjectByType<PlayerMovement>().gameObject.transform.position.z) - transform.position).normalized);
        transform.rotation = lookRotation;

        RemoveHealth(mmrscript.DamageReturn(attackname));
        Knockback(mmrscript.KnockbackReturn(attackname));
    }
    public void Knockback(Vector3 direction)
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.x = direction.x;
        velocity.y = direction.y;
        velocity.z = direction.z;
        rb.linearVelocity = velocity;
    }
    IEnumerator DontCheckGrounded()
    {
        checkingGrounded = false;
        yield return new WaitForSeconds(0.1f);
        checkingGrounded = true;
    }
    IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(0.2f);
        isGrounded = false;
    }
}
