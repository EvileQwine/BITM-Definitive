using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.WSA;

public class ThrownHatchet : MonoBehaviour
{
    Rigidbody rb;
    public GameObject player;
    [SerializeField] int rSpeed = 800;
    [SerializeField] int mSpeed = 40;
    GameObject _OB;
    public enum Rotation
    {
        None, Forward, Pull, Push, 
    }
    public Rotation rot = Rotation.None;
    bool rebounded = false;
    int unstuckCount = 0;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(Unstuck());
    }
    public void Throw(GameObject ob, int i)
    {
        rot = (Rotation)i;
        if (rot == Rotation.Forward)
        {
            _OB = ob;
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        }
        StartCoroutine(Findtarget());
    }
    IEnumerator Findtarget()
    {
        GameObject curOb = _OB;
        Vector3 direction = (_OB.transform.position - transform.position).normalized * mSpeed;
        Vector3 velocity = rb.linearVelocity;
        velocity.x = direction.x;
        velocity.y = direction.y;
        velocity.z = direction.z;
        rb.linearVelocity = velocity;
        yield return new WaitForSeconds(0.2f);
        if (curOb == _OB)
        {
            StartCoroutine(Findtarget());
        }
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
            EnemyScript es = other.gameObject.GetComponent<EnemyScript>();
            if (rot == Rotation.Forward)
            {
                if (es.isGrounded)
                {
                    es.WasHit("AxeNeutralGrounded");
                }
                else
                {
                    es.WasHit("AxeNeutralAir");
                }
            }
        }
        if (other.gameObject.GetComponent<EnemyScript>() != null || other.gameObject.layer == 3)
        {
            if (!rebounded)
            {
                Throw(player, 1);
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
        else if (rot == Rotation.Push)
        {
            transform.Rotate(new Vector3(-rSpeed, 0, 0) * Time.deltaTime);
        }
        else if (rot == Rotation.Pull)
        {
            transform.Rotate(new Vector3(-rSpeed, 0, 0) * Time.deltaTime);
        }
    }
    IEnumerator Unstuck()
    {
        yield return new WaitForSeconds(1);
        unstuckCount++;
        if (unstuckCount == 10)
        {
            player.GetComponent<PlayerAbilities>().axeCount++;
            Debug.Log("Too Long");
            Destroy(gameObject);
        }
        StartCoroutine(Unstuck());
    }
}
