using System;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class GasScript : MonoBehaviour
{
    float existanceTimer = 0;
    float deathTime = 10f;
    bool shrinking = false;
    public bool combineable = true;
    [SerializeField] GameObject Explosion;

    SphereCollider col;
    MeshRenderer mr;

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        mr.enabled = false;
        col = GetComponent<SphereCollider>();    
    }
    void Start()
    {
        StartCoroutine(ShowSelf());
        transform.localScale *= UnityEngine.Random.Range(0.8f, 1.2f);
    }
    void Update()
    {
        if (!shrinking)
        {
            existanceTimer += Time.deltaTime;
            if (existanceTimer >= deathTime)
            {
                shrinking = true;
                combineable = false;
                GetComponent<SphereCollider>().center = Camera.main.transform.up * - 300;
            }
        }
        else
        {
            if (Time.timeScale != 0)
            {
                transform.localScale -= new Vector3(0.01f, 0.005f, 0.01f);
                if (transform.localScale.x <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GasCloud"))
        {
            if (combineable && other.gameObject.GetComponent<GasScript>().combineable)
            {
                if (transform.localScale.x > other.gameObject.transform.localScale.x)
                {
                    transform.localScale += new Vector3(0.2f, 0.1f, 0.2f);
                    existanceTimer = 0;
                    if (transform.localScale.x >= 4f)
                    {
                        combineable = false;
                    }
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
    public void Explode()
    {
        col.radius *= 1.5f;
        GameObject ExplosionParticle = Instantiate(Explosion, transform.position + UnityEngine.Random.insideUnitSphere, transform.rotation);
        ExplosionParticle.transform.localScale = transform.localScale;
        Destroy(gameObject);
    }
    IEnumerator ShowSelf()
    {
        yield return new WaitForSeconds(0.1f);
        mr.enabled = true;
    }
}
