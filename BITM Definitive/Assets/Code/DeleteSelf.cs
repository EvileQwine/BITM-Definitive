using System.Collections;
using UnityEngine;

public class DeleteSelf : MonoBehaviour
{
    public float deleteCounter;
    void Start()
    {
        StartCoroutine(Deletion());
    }

    IEnumerator Deletion()
    {
        yield return new WaitForSeconds(deleteCounter);
        Destroy(gameObject);
    }
}
