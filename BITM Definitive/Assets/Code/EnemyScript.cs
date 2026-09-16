using System;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float maxHealth;
    public float curHealth;

    Collider col;
    Rigidbody rb;

    bool hitByExplodingGas = false;
    bool insideGas = false;
    void Awake()
    {
        col = GetComponent<Collider>();    
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (curHealth <= 0)
        {
            FindFirstObjectByType<LOIndicationScript>().UpdateLockOn(gameObject);
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
        curHealth -= f;
    }
    public float HealthPercent()
    {
        return curHealth / maxHealth;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Matchstick"))
        {
            RemoveHealth(5);
            Knockback(Vector3.up * 20);
        }
        if (other.gameObject.CompareTag("GasCloud") && !insideGas)
        {
            insideGas = true;
        }
        if (other.gameObject.CompareTag("ExplodingGas") && !hitByExplodingGas)
        {
            hitByExplodingGas = true;
            insideGas = false;
            curHealth -= 20f;
            Knockback(Vector3.up * 40);
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
        if (other.gameObject.CompareTag("ExplodingGas"))
        {
            hitByExplodingGas = false;
        }
    }
    public void Knockback(Vector3 direction)
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.x = direction.x;
        velocity.y = direction.y;
        velocity.z = direction.z;
        rb.linearVelocity = velocity;
    }
}
