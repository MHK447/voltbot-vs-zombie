using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class UpgradeComponent : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI DescText;

    [SerializeField]
    private TextMeshProUGUI UpgradeNameText;

    [SerializeField]
    private TextMeshProUGUI CostText;

    [SerializeField]
    private Button UpgradeBtn;

    [SerializeField]
    private Image UpgradeIcon;

    private int UpgradeIdx = 0;

    private System.Numerics.BigInteger UpgradeCost;

    private UpgradeData UpgradeData;

    private CompositeDisposable disposables = new CompositeDisposable();

    private void Awake()
    {
        UpgradeBtn.onClick.AddListener(OnClickBtn);
    }

    public void Set(int upgradeidx)
    {
        UpgradeIdx = upgradeidx;

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var upgradetd = Tables.Instance.GetTable<UpgradeInfo>().GetData(new KeyValuePair<int, int>(stageidx, upgradeidx));

        if (upgradetd != null)
        {
            UpgradeData = GameRoot.Instance.UserData.CurMode.UpgradeGroupData.FindUpgradeData(UpgradeIdx);

            UpgradeCost = upgradetd.cost;


            DescText.text = Tables.Instance.GetTable<Localize>().GetFormat(upgradetd.desc, upgradetd.value);

            if (upgradetd.upgrade_type == (int)UpgradeSystem.UpgradeType.FishCasherSpeedUp)
            {
                var facilitytd = Tables.Instance.GetTable<FacilityInfo>().GetData(upgradetd.value2);

                if (facilitytd != null)
                {
                    UpgradeNameText.text = Tables.Instance.GetTable<Localize>().GetFormat(upgradetd.name, Tables.Instance.GetTable<Localize>().GetString($"food_name_{facilitytd.fish_idx}"));
                }
            }
            else
                UpgradeNameText.text = Tables.Instance.GetTable<Localize>().GetString(upgradetd.name);

            CostText.text = Utility.CalculateMoneyToString(UpgradeCost);

            UpgradeIcon.sprite = Config.Instance.GetCommonImg(upgradetd.icon);

            GameRoot.Instance.UserData.CurMode.Money.Subscribe(x =>
            {
                UpgradeBtn.interactable = x >= UpgradeCost;
            }).AddTo(disposables);


            GameRoot.Instance.StartCoroutine(WaitOneFrame());
        }
    }

    public IEnumerator WaitOneFrame()
    {
        yield return new WaitForEndOfFrame();


        UpgradeBtn.interactable = GameRoot.Instance.UserData.CurMode.Money.Value >= UpgradeCost;
    }

    public void OnClickBtn()
    {
        if (GameRoot.Instance.UserData.CurMode.Money.Value >= UpgradeCost)
        {
            UpgradeData.UpgradeGet();

            ProjectUtility.SetActiveCheck(this.gameObject, false);

            GameRoot.Instance.UpgradeSystem.AddUpgradeData(UpgradeIdx, UpgradeData.UpgradeType);

            GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, -UpgradeCost);

            GameRoot.Instance.NaviSystem.CurNaviOnType = NaviSystem.NaviType.CloseUpgradeBtn;
            GameRoot.Instance.NaviSystem.NaviOff(NaviSystem.NaviType.UpgradeStart);
        }

    }

}
