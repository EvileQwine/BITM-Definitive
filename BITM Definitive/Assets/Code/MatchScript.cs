using NUnit.Framework;
using System;
using UnityEngine;

public class MatchScript : MonoBehaviour
{
    Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Launch(Vector3 direction)
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.x = direction.x;
        velocity.y = direction.y;
        velocity.z = direction.z;
        rb.linearVelocity = velocity;
    }
    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            if (other.GetComponent<GasScript>() != null)
            {
                GasScript[] gasses = FindObjectsByType<GasScript>(FindObjectsSortMode.None);
                foreach (GasScript gas in gasses)
                {
                    gas.Explode();
                }
                EnemyScript[] enemies = FindObjectsByType<EnemyScript>(FindObjectsSortMode.None);
                foreach (EnemyScript es in enemies)
                {
                    es.ShouldExplode();
                }
            }
            Destroy(gameObject);
        }
    }
}
