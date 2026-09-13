using UnityEngine;
using TMPro;

public class FPSScript : MonoBehaviour
{
    TextMeshProUGUI textMesh;

    float fpsU;
    float fpsF;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        fpsU = 1f / Time.deltaTime;
    }
    void FixedUpdate()
    {
        fpsF = 1f / Time.deltaTime;
        textMesh.text = $"{(int)fpsU}fpsU \n {(int)fpsF}fpsF";
    }
}
