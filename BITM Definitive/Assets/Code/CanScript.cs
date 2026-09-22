using UnityEngine;
using UnityEngine.UIElements;

public class CanScript : MonoBehaviour
{
    [SerializeField] GameObject gasPrefab;
    public GameObject Player;

    Rigidbody rb;
    float rd = 5;
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
                collided.RemoveHealth(10, 1);
            }
            for (int i = 0; i < 3; i++)
            {
                Vector3 position = new Vector3(transform.position.x + Random.Range(-rd, rd), transform.position.y, transform.position.z + Random.Range(-rd, rd));
                MakeGas(position);
            }
            Vector3 direction = (Player.transform.position - transform.position).normalized;
            direction.y = transform.position.y;
            MakeGas(direction * 6);
            Destroy(gameObject);
        }
    }
    void MakeGas(Vector3 position)
    {
        Quaternion q = Random.rotation;
        q.x = 0;
        q.z = 0;
        GameObject gas = Instantiate(gasPrefab, position, q);
        gas.GetComponent<GasScript>().combineable = false;
        gas.transform.localScale *= Random.Range(0.75f, 2f);
    }
}
