using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;

[UIPath("UI/Popup/PopupIngameBottom")]
public class PopupIngameBottom : UIBase
{
    [SerializeField]
    private SelectItemContents SelectItemContents;


    public SelectItemContents GetSelectItemContents { get { return SelectItemContents; } }

    public void Init()
    {
        SelectItemContents.Init();
    }


    public void ActiveSelectOn()
    {
        ProjectUtility.SetActiveCheck(SelectItemContents.gameObject , true);

        SelectItemContents.ActiveSelectOn();
    }
}
