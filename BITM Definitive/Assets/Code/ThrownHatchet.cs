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
        Side, PrePull, Push, Pull, 
    }
    public Rotation rot;
    public bool rebounded = false;
    int unstuckCount = 0;

    Vector3 targetPos;
    public int pullPointProgression;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(Unstuck());
        pullPointProgression = 0;
    }
    public void Throw(GameObject ob, int i)
    {
        rot = (Rotation)i;
        _OB = ob;
        switch (rot)
        {
            case Rotation.Side:
                transform.rotation = Quaternion.Euler(transform.rotation.x, 0f, 90f);
                transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
                break;
            case Rotation.PrePull:
              targetPos = new Vector3(_OB.transform.position.x, _OB.transform.position.y + 3, _OB.transform.position.z);
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
                break;
            case Rotation.Push:
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
                break;
        }
        StartCoroutine(Findtarget());
    }
    IEnumerator Findtarget()
    {
        GameObject curOb = _OB;
        if (_OB == null)
        {
            _OB = player;
        }
        if (_OB == player)
        {
            targetPos = player.transform.position;
        }
        else
        {  
            targetPos = GetTargetPos();
        }
        Vector3 direction = (targetPos - transform.position).normalized * mSpeed;
        Vector3 velocity = rb.linearVelocity;
        velocity.x = direction.x;
        velocity.y = direction.y;
        velocity.z = direction.z;
        rb.linearVelocity = velocity;
        yield return new WaitForSeconds(0.1f);
        if (curOb == _OB)
        {
            StartCoroutine(Findtarget());
        }
    }
    Vector3 GetTargetPos()
    {
        if (rot == Rotation.PrePull && pullPointProgression != 2)
        {
            return targetPos;
        }
        return _OB.transform.position;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (rebounded || rot == Rotation.Pull)
            {
                player.GetComponent<PlayerAbilities>().axeCount++;
                Destroy(gameObject);
            }
            else return;
        }
        else if (other.gameObject.GetComponent<EnemyScript>() != null)
        {
            EnemyScript es = other.gameObject.GetComponent<EnemyScript>();
            switch (rot)
            {
                case Rotation.Side:
                    if (es.isGrounded)
                    {
                        es.WasHit("AxeNeutralGrounded");
                    }
                    else
                    {
                        es.WasHit("AxeNeutralAir");
                    }
                    break;
                case Rotation.Push:
                    es.WasHit("AxePush");
                    break;
                case Rotation.PrePull:
                    if (es.isGrounded)
                    {
                        es.WasHit("AxeNeutralGrounded");
                    }
                    else
                    {
                        es.WasHit("AxeNeutralAir");
                    }
                    Throw(player, 0);
                    break;
                case Rotation.Pull:
                    if (es.isGrounded)
                    {
                        es.WasHit("AxePullGround");
                    }
                    else
                    {
                        es.WasHit("AxePullAir");
                    }
                    _OB = player;
                    break;
            }
        }
        if (other.gameObject.GetComponent<EnemyScript>() != null || other.gameObject.layer == 3)
        {
            if (!rebounded && rot != Rotation.Pull)
            {
                rb.linearVelocity = Vector3.zero;
                if (rot == Rotation.PrePull)
                {
                    Throw(player, (int)rot);
                }
                else
                {
                    Throw(player, 0);
                }
                rebounded = true;
                rb.angularVelocity = Vector3.zero;
            }
            else
            {
                if (rot == Rotation.PrePull)
                {
                    _OB = player;
                }
            }
        }
    } 
    void Update()
    {
        if (rebounded)
        {
            transform.Rotate(new Vector3(-rSpeed, 0, 0) * Time.deltaTime);
        }
        else
        {
            transform.Rotate(new Vector3(rSpeed, 0, 0) * Time.deltaTime);
        }
        if (rot == Rotation.PrePull)
        {
            Vector3 offset = transform.position - targetPos;
            if (offset.sqrMagnitude <= 0.6)
            {
                if (pullPointProgression == 0)
                {
                    Vector3 AB = (player.transform.position - _OB.transform.position).normalized;
                    targetPos = _OB.transform.position - (AB * 6);
                }
                else if (pullPointProgression == 1)
                {
                    Throw(player, 3);
                }
                pullPointProgression++;
            }
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
