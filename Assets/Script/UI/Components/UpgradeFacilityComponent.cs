using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;
using TMPro;
using UniRx;
using Unity.VisualScripting;


public class UpgradeFacilityComponent : MonoBehaviour
{
    [SerializeField]
    private Image CashImg;

    [SerializeField]
    private Image FishImg;

    [SerializeField]
    private TextMeshProUGUI BefroreValueText;

    [SerializeField]
    private TextMeshProUGUI AfterValueText;

    [SerializeField]
    private TextMeshProUGUI CurCostValueText;

    [SerializeField]
    private TextMeshProUGUI LevelText;

    [SerializeField]
    private ButtonPressed UpgradeBtn;

    [SerializeField]
    private Slider MiddleSlider;

    [SerializeField]
    private TextMeshProUGUI MiddleSliderValueText;

    [SerializeField]
    private GameObject ButtonCurrencyObj;

    [SerializeField]
    private RectTransform OpacityRoot;

    private int FishIdx = 0;

    private StageFishUpgradeData CurStageFacilityData = null;

    private System.Numerics.BigInteger CurPrice = 0;

    private FacilityUpgradeData FacilityUpgradeData;

    private CompositeDisposable disposables = new CompositeDisposable();

    private bool IsMaxLevel = false;


    private void Awake()
    {
        UpgradeBtn.OnPressed = () => OnClickUpgrade();

        transform.rotation = Quaternion.identity;
    }


    public void Set(int fishidx)
    {
        transform.rotation = Quaternion.identity;

        FishIdx = fishidx;

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var td = Tables.Instance.GetTable<FacilityUpgrade>().GetData(new KeyValuePair<int, int>(stageidx, FishIdx));

        FacilityUpgradeData = Tables.Instance.GetTable<FacilityUpgrade>().GetData(new KeyValuePair<int, int>(stageidx, fishidx));

        if (td != null)
        {
            CurStageFacilityData = GameRoot.Instance.FacilitySystem.GetFacilityUpgradeData(FishIdx);
            SetInfo();
        }


        var fishtd = Tables.Instance.GetTable<FishInfo>().GetData(FishIdx);

        if (fishtd != null)
        {
            FishImg.sprite = Config.Instance.GetIngameImg(fishtd.icon);
        }

        UpgradeBtn.Interactable = GameRoot.Instance.UserData.CurMode.Money.Value >= CurPrice;

        disposables.Clear();


        GameRoot.Instance.UserData.CurMode.Money.Subscribe(x =>
        {
            SetInfo();
        }).AddTo(disposables);
    }


    public void SetInfo()
    {
        LevelText.text = Tables.Instance.GetTable<Localize>().GetFormat("str_level", CurStageFacilityData.Level);

        CurPrice = GameRoot.Instance.FacilitySystem.GetFishUpgradeLevelCost(FishIdx, CurStageFacilityData.Level);


        CurCostValueText.text = Utility.CalculateMoneyToString(CurPrice);

        BefroreValueText.text = Utility.CalculateMoneyToString(GameRoot.Instance.FacilitySystem.GetFishCurSellProductValue(FishIdx, CurStageFacilityData.Level));
        AfterValueText.text = Utility.CalculateMoneyToString(GameRoot.Instance.FacilitySystem.GetFishCurSellProductValue(FishIdx, CurStageFacilityData.Level + 1));


        var curvalue = CurStageFacilityData.Level % FacilityUpgradeData.value_count;
        MiddleSlider.value = (float)curvalue / (float)FacilityUpgradeData.value_count;

        MiddleSliderValueText.text = $"{curvalue}/{FacilityUpgradeData.value_count}";


        IsMaxLevel = CurStageFacilityData.Level >= FacilityUpgradeData.max_ugprade_count;

        ProjectUtility.SetActiveCheck(ButtonCurrencyObj, !IsMaxLevel);
        if (IsMaxLevel)
        {
            MiddleSlider.value = 1f;
            LevelText.text = Tables.Instance.GetTable<Localize>().GetString("str_lv_max");
            MiddleSliderValueText.text = CurCostValueText.text = Tables.Instance.GetTable<Localize>().GetString("str_max");

        }

        ProjectUtility.SetActiveCheck(CashImg.gameObject, !IsMaxLevel);

        UpgradeBtn.Interactable = GameRoot.Instance.UserData.CurMode.Money.Value >= CurPrice && !IsMaxLevel;

        OpacityRoot.anchoredPosition = IsMaxLevel ? new Vector2(OpacityRoot.anchoredPosition.x, -20f) : Vector2.zero;
    }

    public void OnClickUpgrade()
    {
        if (CurPrice <= GameRoot.Instance.UserData.CurMode.Money.Value)
        {
            CurStageFacilityData.Level += 1;

            GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, -CurPrice);

            SetInfo();

            if (CurStageFacilityData.Level % FacilityUpgradeData.value_count == 0)
            {
                var getui = GameRoot.Instance.UISystem.GetUI<PopupUpgrade>();
                ProjectUtility.PlayGoodsEffect(Vector3.zero, (int)Config.RewardType.Currency, (int)Config.CurrencyID.Cash, 1, 1, true, null, 0, "", getui);
            }
        }

    }

    private void OnDestroy()
    {
        disposables.Clear();
    }

}
