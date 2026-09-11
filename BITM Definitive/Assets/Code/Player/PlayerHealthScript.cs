using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class PlayerHealthScript : MonoBehaviour
{
    [SerializeField] float maxHealth = 100;
    [SerializeField] float curHealth = 100;
    [SerializeField] float disappearTime  = 5;

    [SerializeField] UnityEngine.UI.Image bar;
    [SerializeField] UnityEngine.UI.Image overlay;
    
    float showTimeCounter;

    LOIndicationScript LOIScript;

    void Awake()
    {
        bar.enabled = true;
        LOIScript = FindFirstObjectByType<LOIndicationScript>();
    }
    void Update()
    {
        bar.fillAmount = curHealth/maxHealth;
        if (bar.enabled && LOIScript.LockedOn == LOstates.Off)
        {
            showTimeCounter += Time.deltaTime;
        }
        if (showTimeCounter >= disappearTime)
        {
            bar.enabled = false;
            overlay.enabled = false;
        }
    }
    public void ResetCounter()
    {
        showTimeCounter = 0;
        bar.enabled = true;
        overlay.enabled = true;
    } 
}
