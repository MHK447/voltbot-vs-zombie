using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using UniRx;
using TMPro;

[UIPath("UI/Page/HUDTotal", true)]
public class HUDTotal : UIBase
{
    [SerializeField]
    private Button UpgradeBtn;
    public Transform GetUpgradeBtn { get { return UpgradeBtn.transform; } }

    [SerializeField]
    private Button NextStageBtn;

    public Transform GetNextStageBtn { get { return NextStageBtn.transform; } }

    [SerializeField]
    private TextMeshProUGUI AdVehicleTimeText;

    [SerializeField]
    private GameObject VehicleObj;

    [SerializeField]
    private Text FpsText;

    [SerializeField]
    private Button BoostBtn;
    public Transform GetBoostBtn { get { return BoostBtn.transform; } }

    [SerializeField]
    private Button SettingBtn;

    public Transform GetUpgradeBtnTr { get { return UpgradeBtn.transform; } }

    [SerializeField]
    private Button InterAdBtn;

    private float deltaTime = 0.0f;

    protected override void Awake()
    {
        base.Awake();
    
        SettingBtn.onClick.AddListener(OnClickSetting);
        TopCurrencySync();

        GameRoot.Instance.ShopSystem.IsVipProperty.Subscribe(x =>
        {
            ProjectUtility.SetActiveCheck(InterAdBtn.gameObject, !x);
        }).AddTo(this);

        ContentsOpenCheck();

#if BANPFRI_LOG
        ProjectUtility.SetActive(FpsText.gameObject , true);
#else
        ProjectUtility.SetActiveCheck(FpsText.gameObject, false);
#endif

    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

#if BANPFRI_LOG
                FpsText.text = $"FPS: {Mathf.CeilToInt(fps)}";
#endif
    }


    public override void TopCurrencySync()
    {
        base.TopCurrencySync();

        if (CurrencyTop.CashText != null)
        {
            CurrencyTop.CashText.text = GameRoot.Instance.UserData.Cash.Value.ToString();
            GameRoot.Instance.UserData.HUDCash.Subscribe(x =>
            {
                CurrencyTop.CashText.text = x.ToString();
            }).AddTo(this);


            CurrencyTop.CashText.text = GameRoot.Instance.UserData.Cash.Value.ToString();

        }


        if (CurrencyTop.MoneyText != null)
        {
            CurrencyTop.MoneyText.text = ProjectUtility.CalculateMoneyToString(GameRoot.Instance.UserData.CurMode.Money.Value);

            GameRoot.Instance.UserData.HUDMoney.Subscribe(x =>
            {

                CurrencyTop.MoneyText.text = ProjectUtility.CalculateMoneyToString(GameRoot.Instance.UserData.CurMode.Money.Value);
            }).AddTo(this);
        }

        CurrencyTop.MoneyText.text = ProjectUtility.CalculateMoneyToString(GameRoot.Instance.UserData.CurMode.Money.Value);

    }

    public void ContentsOpenCheck()
    {
        ProjectUtility.SetActiveCheck(BoostBtn.gameObject, GameRoot.Instance.ContentsOpenSystem.ContentsOpenCheck(ContentsOpenSystem.ContentsOpenType.BoostBuff));
        ProjectUtility.SetActiveCheck(NextStageBtn.gameObject, GameRoot.Instance.ContentsOpenSystem.ContentsOpenCheck(ContentsOpenSystem.ContentsOpenType.NextStageBtn));
    }

    public void OnClickSetting()
    {
        GameRoot.Instance.UISystem.OpenUI<PopupOption>();
    }
}
