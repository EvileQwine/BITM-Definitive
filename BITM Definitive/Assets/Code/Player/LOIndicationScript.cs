using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public enum LOstates { On, Off, None };
public class LOIndicationScript : MonoBehaviour
{
    [SerializeField] GameObject Player;

    public LOstates LockedOn;

    public GameObject LockedOnEnemy;
    RectTransform rectTransform;
    UnityEngine.UI.Image image;

    void Awake()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        rectTransform = GetComponent<RectTransform>();
        image.enabled = false;
        LockedOn = LOstates.Off;
}
    void Update()
    {
        if (LockedOn == LOstates.On)
        {
            WhileLockedOn();
            if (LockedOnEnemy == null)
            {
                JustLockedOn();
            }
        }
    }
    public void JustLockedOn()
    {
        Player.GetComponent<PlayerHealthScript>().ShowBars();
        if (Player.GetComponent<PlayerMovement>().horMovement != null)
        {
            if (Physics.SphereCast(Player.transform.position, 15, Player.GetComponent<PlayerMovement>().horMovement, out RaycastHit hit, 200))
            {
                GameObject hitObject = hit.transform.gameObject;
                if (hitObject != null)
                {
                    if (hitObject.GetComponent<EnemyScript>() != null && !hitObject.CompareTag("DyingEnemy"))
                    {
                        LockedOnEnemy = hitObject;
                        LockedOn = LOstates.On;
                        image.enabled = true;
                        return;
                    }
                }
            }
        }
        if (FindClosest(Player.transform) == null)
        {
            LockedOn = LOstates.None;
            return;
        }
        else
        {
            LockedOnEnemy = FindClosest(Player.transform);
        }
        LockedOn = LOstates.On;
        image.enabled = true;
    }
    public void JustLockedOff()
    {
        LockedOn = LOstates.Off;
        image.enabled = false;
    }
    void WhileLockedOn()
    {
        image.fillAmount = LockedOnEnemy.GetComponent<EnemyScript>().HealthPercent();
        rectTransform.position = Camera.main.WorldToScreenPoint(LockedOnEnemy.transform.position);
    }
    public GameObject FindClosest(Transform searchPos)
    {
        GameObject closest;
        EnemyScript[] temp = FindObjectsByType<EnemyScript>(FindObjectsSortMode.None);
        List<GameObject> enemies = new(); 
        for (int i = 0; i < temp.Length; i++)
        {
            if (!temp[i].gameObject.CompareTag("DyingEnemy"))
            {
                enemies.Add(temp[i].gameObject);
            }
        }
        if (enemies.Count == 0)
        {
            return null;
        }
        closest = enemies.ToList().OrderBy(x => (x.transform.position - searchPos.position).magnitude).First();
        return closest;
    }
    public void UpdateLockOn(GameObject enemy)
    {
        if (LockedOn == LOstates.On)
        {    
            JustLockedOff();
            enemy.tag = "DyingEnemy";
            JustLockedOn();
        }
        Destroy(enemy);
    }
}
