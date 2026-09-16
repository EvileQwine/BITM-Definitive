using UnityEngine;

public class SpawnEnemiesTestScene : MonoBehaviour
{
    [SerializeField] GameObject e;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerMovement>() != null)
        {
            Instantiate(e, e.transform.position, e.transform.rotation);
        }
    }
}
