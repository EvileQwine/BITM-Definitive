using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KentDaiStats : MonoBehaviour
{
    TextMeshProUGUI textMesh;
    LOIndicationScript LOIScript;
    EnemyScript Kent;
    int comboCount;
    
    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        LOIScript = FindFirstObjectByType<LOIndicationScript>();
    }
    void Start()
    {
        EnemyScript[] temp = FindObjectsByType<EnemyScript>(FindObjectsSortMode.None);
        for (int i = 0; i < temp.Length; i++)
        {
            if (!temp[i].gameObject.GetComponent<EnemyScript>().canDie)
            {
                Kent = temp[i];
            }
        }
    }
    void Update()
    {
        textMesh.text = $"{(int)(Kent.curHealth * -1)} \n {Kent.comboCount}";
    }
}
