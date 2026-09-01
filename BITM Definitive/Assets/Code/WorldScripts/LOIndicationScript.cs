using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System.Linq;

public class LOIndicationScript : MonoBehaviour
{

    public KeyCode LockOnActive = KeyCode.Space;
    public bool LockedOn = false;

    GameObject[] Enemies;
    RectTransform rectTransform;
    UnityEngine.UI.Image image;

    void Awake()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        rectTransform = GetComponent<RectTransform>();
        image.enabled = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(LockOnActive))
        { JustLockedOn(); }
        if (Input.GetKeyUp(LockOnActive))
        {
            LockedOn = false;
            image.enabled = false;
        }
        if (LockedOn)
        {
            WhileLockedOn();
        }
    }
    void JustLockedOn()
    {
        LockedOn = true;
        image.enabled = true;
        EnemyScript[] temp = FindObjectsByType<EnemyScript>(FindObjectsSortMode.None);
        Enemies = new GameObject[temp.Count()];
        for (int i = 0; i < temp.Length; i++)
        {
            Enemies[i] = temp[i].gameObject;
        }
    }
    void WhileLockedOn()
    {
        image.fillAmount = Enemies[0].GetComponent<EnemyScript>().HealthPercent();
        rectTransform.position = Camera.main.WorldToScreenPoint(Enemies[0].transform.position);
    }
}
