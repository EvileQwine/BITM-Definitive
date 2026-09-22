using UnityEngine;
using UnityEngine.WSA;

public class ThrownHatchet : MonoBehaviour
{
    Rigidbody rb;
    public GameObject player;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void FrontSpin(Vector3 direction)
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.x = direction.x;
        velocity.y = direction.y;
        velocity.z = direction.z;
        rb.linearVelocity = velocity;
        rb.angularVelocity = new Vector3(0, 0, 30);
    }
    public void BackSpin()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyScript>() != null)
        {

        }
        else
        {
            transform.LookAt(player.transform.position);
            FrontSpin(transform.forward * 50);
        }
    }
}
