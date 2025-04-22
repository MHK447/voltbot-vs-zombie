using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using BanpoFri;
using TMPro;


public class SelectItemComponent : MonoBehaviour
{
    [SerializeField]
    private Image ItemImage; 

    [SerializeField]
    private TextMeshProUGUI PriceText;

    

    private int SelectItemIdx = 0;

    public void Set(int selectitemidx)
    {
        SelectItemIdx = selectitemidx; 

    }
}
