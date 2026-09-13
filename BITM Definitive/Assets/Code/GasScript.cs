using UnityEngine;

public class GasScript : MonoBehaviour
{
    float existanceTimer = 0;
    float deathTime = 10f;
    public bool shrinking = false;

    void Update()
    {
        if (!shrinking)
        {
            existanceTimer += Time.deltaTime;
            if (existanceTimer >= deathTime)
            {
                shrinking = true;
            }
        }
        else
        {
            if (Time.timeScale != 0)
            {
                transform.localScale -= new Vector3(0.005f, 0.005f, 0.005f);
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
            if (!shrinking && !other.GetComponent<GasScript>().shrinking)
            {
                if (existanceTimer > other.GetComponent<GasScript>().GetTimer())
                {
                    transform.localScale += new Vector3(0.2f, 0.1f, 0.2f);
                    existanceTimer = 0;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
    public float GetTimer()
    {
        return existanceTimer;
    }
}
