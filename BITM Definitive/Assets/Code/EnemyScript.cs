using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float maxHealth;
    public float curHealth;
    public bool canDie = true;
    public int comboCount = 0;
    public bool isGrounded;

    [SerializeField] LayerMask groundLayer;

    float raycastDistance;
    bool justExploded = false;

    Collider col;
    Rigidbody rb;

    bool insideGas = false;
    void Awake()
    {
        col = GetComponent<Collider>();    
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        raycastDistance = (GetComponent<BoxCollider>().size.y * transform.localScale.y / 2) + 0.1f;
    }
    void Update()
    {
        if (curHealth <= 0 && canDie)
        {
            FindFirstObjectByType<LOIndicationScript>().UpdateLockOn(gameObject);
        }

        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, raycastDistance, groundLayer);
        if (isGrounded && rb.linearVelocity.y < 0)
        {
            comboCount = 0;
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
            RemoveHealth(0.1f);
        }
    }
    public void RemoveHealth(float f)
    {
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
            RemoveHealth(5);
            Knockback(Vector3.up * 20);
        }
        if (other.gameObject.CompareTag("GasCloud") && !justExploded)
        {
            insideGas = true;
        }
    }
    void OnTriggerStay(Collider other)
    {

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
            RemoveHealth(20);
            Knockback(Vector3.up * 40);
        }
    }
    IEnumerator NoGas()
    {
        justExploded = true;
        yield return new WaitForSeconds(0.2f);
        justExploded = false;
    }
    public void Knockback(Vector3 direction)
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.x = direction.x;
        velocity.y = direction.y;
        velocity.z = direction.z;
        rb.linearVelocity = velocity;
        comboCount++;
    }
}
