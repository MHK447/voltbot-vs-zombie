using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;

[UIPath("UI/Popup/PopupSelectItem")]
public class PopupSelectItem : UIBase
{

    [SerializeField]
    List<SelectItemComponent> SelectItemComponentList = new List<SelectItemComponent>();


    public void Init()
    {
        foreach(var selectitem in SelectItemComponentList)
        {
            selectitem.Set(3);
        }
    }
}
