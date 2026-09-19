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

    [SerializeField] GameObject bars;
    [SerializeField] GameObject ranged;
    
    float showTimeCounter;
    bool barsOn = true;

    LOIndicationScript LOIScript;
    PlayerAbilities playerAbilities;

    void Awake()
    {
        foreach (Transform child in bars.transform)
        {
            child.GetComponent<UnityEngine.UI.Image>().enabled = true;  
        }
        playerAbilities = GetComponent<PlayerAbilities>();
    }
    void Start()
    {
        LOIScript = FindFirstObjectByType<LOIndicationScript>();
    }
    void Update()
    {
        bars.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().fillAmount = curHealth / maxHealth;
        bars.transform.GetChild(2).GetComponent<UnityEngine.UI.Image>().fillAmount = playerAbilities.MeterFill();
        if (barsOn && LOIScript.LockedOn == LOstates.Off)
        {
            showTimeCounter += Time.deltaTime;
        }
        if (showTimeCounter >= disappearTime)
        {
            foreach (Transform child in bars.transform)
            {
                child.GetComponent<UnityEngine.UI.Image>().enabled = false;
            }
            barsOn = false;
        }
    }
    public void ShowBars()
    {
        showTimeCounter = 0;
        foreach (Transform child in bars.transform)
        {
            child.GetComponent<UnityEngine.UI.Image>().enabled = true;
        }
        barsOn = true;
    } 
    public void SwapRanged()
    {
        ranged.GetComponent<SelectedRangedIcon>().SwitchRangedIcon();
    }
}
