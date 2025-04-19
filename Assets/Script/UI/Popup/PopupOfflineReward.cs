using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using TMPro;

[UIPath("UI/Popup/PopupOfflineReward")]
public class PopupOfflineReward : UIBase
{
    [SerializeField]
    private TextMeshProUGUI AdRewardValueText;

    [SerializeField]
    private TextMeshProUGUI RewardValueText;


    [SerializeField]
    private Slider TimeSliderValue;

    [SerializeField]
    private TextMeshProUGUI MaxTimeText;

    [SerializeField]
    private TextMeshProUGUI CurTimeText;

    [SerializeField]
    private TextMeshProUGUI UpBenefitText;

    [SerializeField]
    private TextMeshProUGUI MiddleBenefitText;

    [SerializeField]
    private Button RewardBtn;


    [SerializeField]
    private Button ADRewardBtn;

    public int TimeSecond = 0;

    private System.Numerics.BigInteger RewardValue = 0;


    protected override void Awake()
    {
        base.Awake();

        ADRewardBtn.onClick.AddListener(OnClickAdReward);

        RewardBtn.onClick.AddListener(OnClickReward);
    }
    public void Set(int timesecond)
    {
        TimeSecond = timesecond;

        RewardValue = ProjectUtility.CalcOfflineReward(timesecond);

        if (RewardValue == 0)
        {
            Hide();

            return;
        }

        RewardValueText.text = ProjectUtility.CalculateMoneyToString(RewardValue);

        AdRewardValueText.text =
         ProjectUtility.CalculateMoneyToString(RewardValue * GameRoot.Instance.InGameSystem.offline_reward_multiple);

        CurTimeText.text = Utility.GetTimeStringFormattingLong(TimeSecond);

        MaxTimeText.text = Utility.GetTimeStringFormattingLong(GameRoot.Instance.InGameSystem.max_offline_time);

        TimeSliderValue.value = (float)TimeSecond / (float)GameRoot.Instance.InGameSystem.max_offline_time;

        MiddleBenefitText.text = Tables.Instance.GetTable<Localize>().GetFormat("offline_time_middle_value", GameRoot.Instance.InGameSystem.offline_reward_multiple);

        UpBenefitText.text = Tables.Instance.GetTable<Localize>().GetFormat("offline_time_value", GameRoot.Instance.InGameSystem.offline_reward_multiple);
    }

    public void OnClickAdReward()
    {
        GameRoot.Instance.GetAdManager.ShowRewardedAd(() =>
        {
            GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, RewardValue * GameRoot.Instance.InGameSystem.offline_reward_multiple);
            GameRoot.Instance.UserData.CurMode.LastLoginTime = TimeSystem.GetCurTime();
            Hide();
        });
    }

    public void OnClickReward()
    {
        GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, RewardValue);
        GameRoot.Instance.UserData.CurMode.LastLoginTime = TimeSystem.GetCurTime();
        Hide();
    }
}

