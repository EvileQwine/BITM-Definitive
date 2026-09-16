using UnityEngine;

public class CanScript : MonoBehaviour
{
    [SerializeField] GameObject gasPrefab;

    Rigidbody rb;
    float rd = 8;
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
        if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("GasCloud"))
        {
            if (other.gameObject.GetComponent<EnemyScript>() != null)
            {
                EnemyScript collided = other.gameObject.GetComponent<EnemyScript>();
                collided.RemoveHealth(10);
                collided.Knockback(rb.linearVelocity);
            }
            for (int i = 0; i < 3; i++)
            {
                Vector3 position = new Vector3(transform.position.x + Random.Range(-rd, rd), transform.position.y, transform.position.z + Random.Range(-rd, rd));
                GameObject gas = Instantiate(gasPrefab, position, transform.rotation);
                gas.GetComponent<GasScript>().combineable = false;
                gas.transform.localScale *= Random.Range(0.75f, 2f);
            }
            Destroy(gameObject);
        }
    }
}
