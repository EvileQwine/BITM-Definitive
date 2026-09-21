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
    int averageFps = 6967;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        StartCoroutine(OneSecond());
    }

    void Update()
    {
        fpss.Add((int)(1f / Time.deltaTime));
    }
    void FixedUpdate()
    {
        textMesh.text = $"{averageFps}fps";
    }
    IEnumerator OneSecond()
    {
        fpss.Clear();
        yield return new WaitForSeconds(1);
        int p = 0;
        for (int i = 0; i < fpss.Count; i++)
        {
            p += fpss[i];
        }
        averageFps = p/fpss.Count;
        StartCoroutine(OneSecond());
    }
}
