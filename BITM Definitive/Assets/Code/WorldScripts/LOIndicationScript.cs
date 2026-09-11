using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class LOIndicationScript : MonoBehaviour
{
    [SerializeField] GameObject Player;

    public LOstates LockedOn;

    GameObject[] Enemies;
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
        }
    }
    public void JustLockedOn()
    {
        if (Player.GetComponent<PlayerMovement>().horMovement != null)
        {
            if (Physics.SphereCast(Player.transform.position, 15, Player.GetComponent<PlayerMovement>().horMovement, out RaycastHit hit, 200))
            {
                GameObject hitObject = hit.transform.gameObject;
                if (hitObject != null && hitObject.GetComponent<EnemyScript>() != null)
                {
                    LockedOnEnemy = hitObject;
                    LockedOn = LOstates.On;
                    image.enabled = true;
                    return;
                }
            }
        }
        EnemyScript[] temp = FindObjectsByType<EnemyScript>(FindObjectsSortMode.None);
        Enemies = new GameObject[temp.Count()];
        for (int i = 0; i < temp.Length; i++)
        {
            Enemies[i] = temp[i].gameObject;
        }
        if (Enemies.Length == 0)
        {
            LockedOn = LOstates.None;
            return;
        }
        LockedOnEnemy = Enemies.ToList().OrderBy(x => (x.transform.position - Player.transform.position).magnitude).First();
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
}
