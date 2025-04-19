using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;
using UniRx;
using System.Linq;

public class FacilityComponent : MonoBehaviour
{
    [SerializeField]
    protected BoxCollider2D RigidCol;

    [SerializeField]
    private List<GameObject> FacilityOpenList = new List<GameObject>();

    [SerializeField]
    protected List<Transform> ConsumerWaitTr = new List<Transform>();

    [SerializeField]
    private ContentsOpenComponent ContentsOpenComponent;

    public Transform GetContentsOpenComponentTr { get { return ContentsOpenComponent.transform; } }


    public Config.FacilityTypeIdx FacilityTypeIdx = Config.FacilityTypeIdx.None;

    protected int CapacityMaxCount = 0;

    protected FacilityData FacilityData;

    public FacilityData GetFacilityData { get { return FacilityData; } }

    private CompositeDisposable disposables = new CompositeDisposable();

    protected InGameStage InGameStage;

    protected OtterBase Player;

    protected int BaseCapacity = 0;

    private int consumerOrder = 0;

    public virtual void Init()
    {
        consumerOrder = 0;

        InGameStage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;

        Player = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().GetPlayer;

        FacilityData = GameRoot.Instance.UserData.CurMode.StageData.FindFacilityData((int)FacilityTypeIdx);

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var facilitytd = Tables.Instance.GetTable<FacilityInfo>().GetData((int)FacilityTypeIdx);

        BaseCapacity = facilitytd.start_capacity;

        var stagefacilitytd = Tables.Instance.GetTable<StageFacilityInfo>().DataList.Where(x => x.facilityidx == (int)FacilityTypeIdx
        && x.stageidx == stageidx).FirstOrDefault();

        if (stagefacilitytd == null) return;

        var buffvalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.ShelfCapacityUp, (int)FacilityTypeIdx);

        CapacityMaxCount = BaseCapacity + (int)buffvalue;

        RigidCol.isTrigger = !FacilityData.IsOpen;

        foreach (var facility in FacilityOpenList)
        {
            ProjectUtility.SetActiveCheck(facility, FacilityData.IsOpen);
        }

        ContentsOpenComponent.Set(FacilityData, OpenFacility);
    }


    public virtual Transform GetConsumerTr()
    {
        if (ConsumerWaitTr.Count == 0) return null;

        consumerOrder %= ConsumerWaitTr.Count; // consumerOrder가 범위를 넘지 않도록 보장
        Transform selectedTransform = ConsumerWaitTr[consumerOrder]; // 현재 consumerOrder 위치 선택
        consumerOrder++; // 다음 차례로 증가

        return selectedTransform;
    }



    public virtual bool IsMaxCountCheck()
    {
        if (FacilityData == null) return false;


        return FacilityData.CapacityCountProperty.Value >= CapacityMaxCount;
    }


    public Transform GetConsumerTr(int order)
    {
        return ConsumerWaitTr[order];
    }

    public bool IsOpenFacility()
    {
        if (FacilityData == null) return false;


        return FacilityData.IsOpen;
    }



    public void OpenFacility()
    {
        if (FacilityData == null)
        {
            Debug.LogError("OpenFacility: FacilityData is null");
            return;
        }

        FacilityData.IsOpen = true;

        // 다음 시설 인덱스 업데이트 전 안전성 검사
        if (GameRoot.Instance != null && 
            GameRoot.Instance.UserData != null && 
            GameRoot.Instance.UserData.CurMode != null && 
            GameRoot.Instance.UserData.CurMode.StageData != null)
        {
            var nextFacilityProperty = GameRoot.Instance.UserData.CurMode.StageData.NextFacilityOpenOrderProperty;
            if (nextFacilityProperty != null)
            {
                nextFacilityProperty.Value += 1;
            }
            else
            {
                Debug.LogWarning("OpenFacility: NextFacilityOpenOrderProperty is null");
            }
        }
        else
        {
            Debug.LogError("OpenFacility: GameRoot or UserData path is null");
            return;
        }

        Init();

        try
        {
            var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;
            var stageinfotd = Tables.Instance.GetTable<StageInfo>().GetData(stageidx);

            if (stageinfotd != null)
            {
                if (GameRoot.Instance.UserData.CurMode.StageData.StageIdx == 1 && FacilityTypeIdx == Config.FacilityTypeIdx.GrilledRedSnapperDisplay)
                {
                    if (GameRoot.Instance.TutorialSystem != null && !GameRoot.Instance.TutorialSystem.IsClearTuto("1"))
                    {
                        GameRoot.Instance.TutorialSystem.StartTutorial("1");
                    }
                }

                if (SoundPlayer.Instance != null)
                {
                    SoundPlayer.Instance.PlaySound("newcontents");
                }

                if (GameRoot.Instance.NaviSystem != null)
                {
                    switch (FacilityTypeIdx)
                    {
                        case Config.FacilityTypeIdx.RedSnapperDisplay:
                            GameRoot.Instance.NaviSystem.NextNavi(NaviSystem.NaviType.Fish_01);
                            break;
                        case Config.FacilityTypeIdx.CheckoutCounter:
                            GameRoot.Instance.NaviSystem.NextNavi(NaviSystem.NaviType.Rack_01);
                            break;
                        case Config.FacilityTypeIdx.RedSnapperFishing:
                            GameRoot.Instance.NaviSystem.NextNavi(NaviSystem.NaviType.Fishing);
                            break;
                    }
                }

                if (stageinfotd.consumerfirst_idx == (int)FacilityTypeIdx)
                {
                    var upgradevalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.AddCustomer);

                    for (int i = 0; i < upgradevalue; ++i)
                    {
                        InGameStage.CreateConsumer(1, InGameStage.GetStartWayPoint);
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("OpenFacility 실행 중 오류 발생: " + e.ToString());
        }
    }

    private void OnDestroy()
    {
        disposables.Clear();
    }

    private void OnDisable()
    {
        disposables.Clear();
    }

}
