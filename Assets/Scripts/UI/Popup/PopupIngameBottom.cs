using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;

[UIPath("UI/Popup/PopupIngameBottom")]
public class PopupIngameBottom : UIBase
{
    [SerializeField]
    private SelectItemContents SelectItemContents;

    public void Init()
    {
        SelectItemContents.Init();
    }
}
