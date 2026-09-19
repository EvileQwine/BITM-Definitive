using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class SelectedRangedIcon : MonoBehaviour
{
    UnityEngine.UI.Image rangedDisplay;

    [SerializeField] Sprite hatchets;
    [SerializeField] Sprite gasCans;
    public byte current = 0;

    void Awake()
    {
        rangedDisplay = GetComponent<UnityEngine.UI.Image>();
    }
    public void SwitchRangedIcon()
    {
        if (current == 0)
        {
            rangedDisplay.sprite = hatchets;
            current = 1;
            return;
        }
        rangedDisplay.sprite = gasCans;
        current = 0;
    }
}
