using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;

public class FPSScript : MonoBehaviour
{
    TextMeshProUGUI textMesh;

    float fpsU;
    List<int> fpss = new();
    int averageFps;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        StartCoroutine(FPSReading());
        averageFps = (int)(1/Time.deltaTime);
    }

    void Update()
    {
        fpss.Add((int)(1f / Time.deltaTime));
    }
    void FixedUpdate()
    {
        textMesh.text = $"{averageFps}fps";
    }
    IEnumerator FPSReading()
    {
        fpss.Clear();
        yield return new WaitForSeconds(0.4f);
        int p = 0;
        for (int i = 0; i < fpss.Count; i++)
        {
            p += fpss[i];
        }
        averageFps = p/fpss.Count;
        StartCoroutine(FPSReading());
    }
}
