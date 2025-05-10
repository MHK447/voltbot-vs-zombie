using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using TMPro;
using UniRx;
[UIPath("UI/Popup/PopupIngameBottom")]
public class PopupIngameBottom : UIBase
{
    [SerializeField]
    private SelectItemContents SelectItemContents;

    [SerializeField]
    private TextMeshProUGUI MoneyText;

    public SelectItemContents GetSelectItemContents { get { return SelectItemContents; } }

    protected override void Awake()
    {
        base.Awake();
        GameRoot.Instance.UserData.Money.Subscribe(x=> { MoneyText.text = x.ToString(); }).AddTo(this);
    }

    public void Init()
    {
        SelectItemContents.Init();
    }


    public void ActiveSelectOn()
    {
        //SelectItemContents.ActiveSelectOn();
    }
}
