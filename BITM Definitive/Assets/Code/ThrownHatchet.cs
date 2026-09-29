using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.WSA;

public class ThrownHatchet : MonoBehaviour
{
    Rigidbody rb;
    public GameObject player;
    [SerializeField] int rSpeed = 650;
    [SerializeField] int mSpeed = 50;
    enum Rotation
    {
        None, Forward, Backward 
    }
    Rotation rot = Rotation.None;
    bool rebounded = false;
    int unstuckCount = 0;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(Unstuck());
    }
    public void Throw(Vector3 direction, int i)
    {
        rot = (Rotation)i;
        if (rot == Rotation.Forward)
        {
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        }
        transform.LookAt(direction);
        direction = transform.forward * mSpeed;
        Vector3 velocity = rb.linearVelocity;
        velocity.x = direction.x;
        velocity.y = direction.y;
        velocity.z = direction.z;
        rb.linearVelocity = velocity;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (rebounded)
            {
                player.GetComponent<PlayerAbilities>().axeCount++;
                Destroy(gameObject);
            }
            else return;
        }
        else if (other.gameObject.GetComponent<EnemyScript>() != null)
        {
            other.gameObject.GetComponent<EnemyScript>().Knockback(new Vector3(0, 20, 0));
            other.gameObject.GetComponent<EnemyScript>().RemoveHealth(20, 1);
        }
        if (other.gameObject.GetComponent<EnemyScript>() != null || other.gameObject.layer == 3)
        {
            if (!rebounded)
            {
                transform.rotation = Quaternion.Euler(0, 0, -90);
                Throw(player.transform.position, 1);
                rebounded = true;
            }
        }
    } 
    void Update()
    {
        if (rot == Rotation.Forward)
        {
            transform.Rotate(new Vector3(rSpeed, 0, 0) * Time.deltaTime);
        }
        else if (rot == Rotation.Backward)
        {
            transform.Rotate(new Vector3(-rSpeed, 0, 0) * Time.deltaTime);
        }
    }
    IEnumerator Unstuck()
    {
        yield return new WaitForSeconds(1);
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance < 300)
        {
            player.GetComponent<PlayerAbilities>().axeCount++;
            Destroy(gameObject);
        }
        unstuckCount++;
        if (unstuckCount == 20)
        {
            player.GetComponent<PlayerAbilities>().axeCount++;
            Destroy(gameObject);
        }
        StartCoroutine(Unstuck());
    }
}
