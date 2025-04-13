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
            UpgradeCost = upgradetd.cost;


            DescText.text = Tables.Instance.GetTable<Localize>().GetFormat(upgradetd.desc, upgradetd.value);

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
            ProjectUtility.SetActiveCheck(this.gameObject, false);

            GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, -UpgradeCost);
        }

    }

}
