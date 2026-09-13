using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float maxHealth;
    public float curHealth;

    Collider col;

    void Awake()
    {
        col = GetComponent<Collider>();    
    }
    void Update()
    {
        if (curHealth <= 0)
        {
            FindFirstObjectByType<LOIndicationScript>().UpdateLockOn(gameObject);
        }    
    }
    public float HealthPercent()
    {
        return curHealth / maxHealth;
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("GasCloud"))
        {
            curHealth -= 0.5f;
        }    
    }
}
