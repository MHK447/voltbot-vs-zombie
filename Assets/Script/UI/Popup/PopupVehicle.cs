using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;
using TMPro;
using System.Linq;

[UIPath("UI/Popup/PopupVehicle")]
public class PopupVehicle : UIBase
{
    [SerializeField]
    private TextMeshProUGUI MinuteText;

    [SerializeField]
    private TextMeshProUGUI CostText;


    [SerializeField]
    private Button AdBtn;


    [SerializeField]
    private Button CashBtn;

    [SerializeField]
    private TextMeshProUGUI BuffValueText;

    private int VehicleIdx = 0;

    protected override void Awake()
    {
        base.Awake();

        CashBtn.onClick.AddListener(OnClickCash);
        AdBtn.onClick.AddListener(OnClickAd);

    }

    public void Init()
    {
        MinuteText.text = ProjectUtility.GetTimeStringFormattingShort(GameRoot.Instance.VehicleSystem.ad_ride_time);
        CostText.text = GameRoot.Instance.VehicleSystem.ride_cash_value.ToString();

        var td = Tables.Instance.GetTable<VehicleInfo>().GetData(1);

        if (td != null)
        {
            BuffValueText.text = Tables.Instance.GetTable<Localize>().GetFormat("ad_vehicle_value", td.buff_value);

        }
    }


    public void OnClickCash()
    {
        if (GameRoot.Instance.UserData.Cash.Value >= GameRoot.Instance.VehicleSystem.ride_cash_value)
        {
            GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Cash, -GameRoot.Instance.VehicleSystem.ride_cash_value);
            Hide();
            GameRoot.Instance.VehicleSystem.AdVehicleActive(true);
        }

    }

    public void OnClickAd()
    {
        Hide();

    GameRoot.Instance.GetAdManager.ShowRewardedAd(() =>
    {
        GameRoot.Instance.VehicleSystem.AdVehicleActive(true);
    });
    }
    

    public override void Hide()
    {
        base.Hide();

        if (!GameRoot.Instance.VehicleSystem.IsAdEquipVehicle)
        {
            GameRoot.Instance.VehicleSystem.AdVehicleShowTime = 0;
            GameRoot.Instance.VehicleSystem.IsShowAdVehicle = false;
        }

        GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.ActiveOffVehicle();
        
        GameRoot.Instance.GameNotification.RemoveNoti(NoticeComponent.NoticeType.Seaweed);
    }
}
