using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;
using UniRx;
using TMPro;
using System.Numerics;

[UIPath("UI/Popup/PopupNextStage")]
public class PopupNextStage : UIBase
{
    [SerializeField]
    private Image BeforeImg;

    [SerializeField]
    private Image AfterFoodImg;

    [SerializeField]
    private TextMeshProUGUI BeforeFoodNameText;

    [SerializeField]
    private TextMeshProUGUI AfterFoodNameText;


    [SerializeField]
    private TextMeshProUGUI NextStagePurchaseValueText;

    [SerializeField]
    private Button NextStageBtn;

    private CompositeDisposable disposables = new CompositeDisposable();
    private BigInteger PurChaseMoney;

    [SerializeField]
    private TextMeshProUGUI FacilityUpgradeCountText;

    [SerializeField]
    private TextMeshProUGUI FishUpgradeMaxCountText;

    [SerializeField]
    private Button GoToFishUpgradeBtn;

    [SerializeField]
    private Button GotoFacilityUpgradeBtn;


    [SerializeField]
    private Slider FacilityUpgradeSlider;

    [SerializeField]
    private Slider FishUpgradeSlider;


    protected override void Awake()
    {
        base.Awake();

        NextStageBtn.onClick.AddListener(OnClickNextStage);

        GotoFacilityUpgradeBtn.onClick.AddListener(OnClickFacilityUpgrade);

        GoToFishUpgradeBtn.onClick.AddListener(OnClickFoodUpgrade);
    }


    public void OnClickFacilityUpgrade()
    {
        GameRoot.Instance.UISystem.OpenUI<PopupUpgrade>(popup => {
            popup.Init();
            popup.OnClickFacility();
            });
    }

    public void OnClickFoodUpgrade()
    {
        GameRoot.Instance.UISystem.OpenUI<PopupUpgrade>(popup =>{
            popup.Init();   
            popup.OnClickProduct();
            });
    }


    public void OnClickNextStage()
    {
        Hide();
        GameRoot.Instance.InGameSystem.NextGameStage();
    }


    public void Init()
    {
        var curstageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var td = Tables.Instance.GetTable<StageInfo>().GetData(curstageidx);

        var nextstagetd = Tables.Instance.GetTable<StageInfo>().GetData(curstageidx + 1);

        if (td != null)
        {
            PurChaseMoney = td.next_stage_money;

            disposables.Clear();

            UpgradeSliderCheck();

            foreach (var upgrade in GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList)
            {
                upgrade.IsBuyCheckProperty.SkipLatestValueOnSubscribe().Subscribe(x => {
                    if(x)
                    {
                        UpgradeSliderCheck();
                    }
                }).AddTo(disposables);
            }

            GameRoot.Instance.UserData.CurMode.Money.SkipLatestValueOnSubscribe().Subscribe(x => { UpgradeSliderCheck(); }).AddTo(disposables);

            BeforeImg.sprite = Config.Instance.GetIngameImg(td.nextstage_image);
            BeforeFoodNameText.text = Tables.Instance.GetTable<Localize>().GetString(td.nextstage_name);

            NextStagePurchaseValueText.text = ProjectUtility.CalculateMoneyToString(PurChaseMoney);
        }


        if(nextstagetd != null)
        {
            AfterFoodNameText.text = Tables.Instance.GetTable<Localize>().GetString(nextstagetd.nextstage_name);
            AfterFoodImg.sprite = Config.Instance.GetIngameImg(nextstagetd.nextstage_image);
        }
    }

    public void UpgradeSliderCheck()
    {
        var upgradelist  = GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList;

        var isbuylist = GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.ToList().FindAll(x => x.IsBuyCheckProperty.Value);

        FacilityUpgradeSlider.value = (float)isbuylist.Count / (float)upgradelist.Count;

        FacilityUpgradeCountText.text = $"{isbuylist.Count}/{upgradelist.Count}";

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var tdlist = Tables.Instance.GetTable<FacilityUpgrade>().DataList.ToList().FindAll(x=> x.stageidx == stageidx);

        var maxcount = GameRoot.Instance.FacilitySystem.GetFishUpgradeMaxLevelCount();

        FishUpgradeMaxCountText.text = $"{maxcount}/{tdlist.Count}";

        FishUpgradeSlider.value = (float)maxcount / (float)tdlist.Count;

        NextStageBtn.interactable = isbuylist.Count >= upgradelist.Count && GameRoot.Instance.UserData.CurMode.Money.Value >= PurChaseMoney && maxcount >= tdlist.Count;
    }
}
