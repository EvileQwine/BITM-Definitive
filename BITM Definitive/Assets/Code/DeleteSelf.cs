using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

public class DeleteSelf : MonoBehaviour
{
    public float deleteCounter;

    ParticleSystem p;
    private void Awake()
    {
        p = gameObject.GetComponent<ParticleSystem>();
    }
    void Start()
    {
        StartCoroutine(Deletion());
    }

    IEnumerator Deletion()
    {
        yield return new WaitForSeconds(deleteCounter);
        p.Clear();
        Destroy(gameObject);
        p.Clear();
    }
}
