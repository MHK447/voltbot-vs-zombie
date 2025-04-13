using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;

[UIPath("UI/Page/PageLobbyBattle")]
public class PageLobbyBattle : UIBase
{

    [SerializeField]
    private Button StartBtn;

    [SerializeField]
    private Text HighWaveText;

    [SerializeField]
    private List<Image> UnitImgList = new List<Image>();

    protected override void Awake()
    {
        base.Awake();
        StartBtn.onClick.AddListener(OnClickStart);
    }


    public void OnClickStart()
    {
        StartBtn.interactable = false;
        GameRoot.Instance.InGameSystem.DeadCount.Value = 0;
        GameRoot.Instance.InGameSystem.LevelProperty.Value = 0;

        GameRoot.Instance.WaitTimeAndCallback(2f, () => {
            StartBtn.interactable = true;
        });
    }


    //public void Init()
    //{
    //    var highwave = GameRoot.Instance.UserData.CurMode.StageData.StageHighWave;

    //    var stagewavetd = Tables.Instance.GetTable<StageWaveInfo>().DataList.ToList().FindAll(x => x.wave_idx > highwave);


    //    float closestValue = stagewavetd[0].wave_idx;
    //    float minDifference = Mathf.Abs(highwave - closestValue);

    //    StageWaveInfoData data = null;


    //    if (stagewavetd.Count == 0)
    //    {
    //        data = Tables.Instance.GetTable<StageWaveInfo>().DataList.ToList().Last();
    //    }
    //    else
    //    {
    //        data = stagewavetd.First();
    //    }



    //    if (data != null)
    //    {
    //        HighWaveText.text = $"Highest Wave:{highwave}";

    //        var unittd = Tables.Instance.GetTable<EnemyInfo>().GetData(data.boss_idx);

    //        foreach (var unitimg in UnitImgList)
    //        {
    //            unitimg.sprite = Config.Instance.GetUnitImg(unittd.image);
    //        }
    //    }
    //}


    public override void CustomSortingOrder()
    {
        base.CustomSortingOrder();

        transform.GetComponent<Canvas>().sortingOrder = (int)UIBase.HUDTypeTopSorting.POPUPTOP;
    }


    public void SortingRollBack()
    {
        transform.GetComponent<Canvas>().sortingOrder = UISystem.START_PAGE_SORTING_NUMBER;
    }




}
