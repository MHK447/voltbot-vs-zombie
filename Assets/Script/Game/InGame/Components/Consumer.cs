using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;
using UniRx;
using System.Linq;
using Spine.Unity;



public class Consumer : Chaser
{
    public enum CurState
    {
        Idle,
        Move,
        WaitProduct,
        Wait,
    }


    public class PatternOrderData
    {
        public int FacilityIdx = 0;
        public int Count = 0;


        public PatternOrderData(int facilityidx, int count)
        {
            FacilityIdx = facilityidx;
            Count = count;
        }
    }

    [SerializeField]
    private Transform OrderTr;

    public ConsumerMoveInfoData CurMoveInfoData;

    [SerializeField]
    private Transform ProductRoot;

    private ConsumerOrderUI ConsumerOrderUI;

    private int CurMissionCount = 0;

    private Queue<PatternOrderData> PatternOrderQueue = new Queue<PatternOrderData>();

    private IReactiveProperty<int> CurFacilityIdxProperty = new ReactiveProperty<int>();

    private IReactiveProperty<int> CurCountProperty = new ReactiveProperty<int>();

    private CompositeDisposable disposables = new CompositeDisposable();

    private CurState State = CurState.Idle;

    public CurState GetState { get { return State; } }

    private RackComponent TargetRack;

    private CookedComponent TargetCooked;

    private Transform FacilityTarget;

    private int CurGoalValue = 0;

    private List<FishComponent> CurFishComponentList = new List<FishComponent>();

    private CounterComponent CounterComponent;

    public System.Action<bool> OnEnd = null;

    public bool IsArrivedCounter = false;

    public int CurCounterOrder = 0;

    private bool IsCounter = false;

    private Renderer Renderer;

    private float lastYPosition;

    private void Awake()
    {
        Renderer = skeletonAnimation.GetComponent<Renderer>();
        lastYPosition = transform.position.y;
    }

    public override void Init(int idx)
    {
        base.Init(idx);

        RandSetSkin();

        FacilityTarget = null;
        TargetRack = null;
        TargetCooked = null;
        IsCounter = false;
        IsArrivedCounter = false;
        CurFishComponentList.Clear();
        PatternOrderQueue.Clear();

        CarryStart(false);
        gameObject.transform.rotation = Quaternion.identity;

        Stage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;
        CounterComponent = Stage.GetCounterComponent;

        CurMissionCount = 0;

        var curstageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        CurMoveInfoData = GameRoot.Instance.FacilitySystem.CreatePattern(curstageidx);

        for (int i = 0; i < CurMoveInfoData.facilityidx.Count; ++i)
        {
            var newpatterndata = new PatternOrderData(CurMoveInfoData.facilityidx[i], CurMoveInfoData.count[i]);

            PatternOrderQueue.Enqueue(newpatterndata);
        }


        CurGoalValue = CurMoveInfoData.count[CurMissionCount];

        if (ConsumerOrderUI == null)
        {
            GameRoot.Instance.UISystem.LoadFloatingUI<ConsumerOrderUI>((orderui) =>
            {
                ConsumerOrderUI = orderui;
                ProjectUtility.SetActiveCheck(ConsumerOrderUI.gameObject, true);
                orderui.Init(OrderTr);
                orderui.Set(this, CurMoveInfoData.facilityidx[CurMissionCount], 0, CurMoveInfoData.count[CurMissionCount]);
            });
        }
        else
        {
            ProjectUtility.SetActiveCheck(ConsumerOrderUI.gameObject, true);
            ConsumerOrderUI.SetImage(ConsumerOrderUI.ConsumerState.Food);
            ConsumerOrderUI.Set(this, CurMoveInfoData.facilityidx[CurMissionCount], 0, CurMoveInfoData.count[CurMissionCount]);
        }

        disposables.Clear();

        CurCountProperty.Subscribe(x =>
        {
            if (ConsumerOrderUI != null)
            {
                ConsumerOrderUI.SetCountText(x);
            }
        }).AddTo(disposables);

        MoveFacility();
    }




    public void MoveFacility()
    {
        if (PatternOrderQueue.Count > 0)
        {
            var newdata = PatternOrderQueue.Dequeue();

            CurFacilityIdxProperty.Value = newdata.FacilityIdx;
            CurGoalValue = newdata.Count;
            CurCountProperty.Value = 0;

            if (ConsumerOrderUI != null)
            {
                ConsumerOrderUI.SetFacilityImg(CurFacilityIdxProperty.Value);
                ConsumerOrderUI.Set(this, CurMoveInfoData.facilityidx[CurMissionCount], 0, CurGoalValue);
            }

            GoToFacility(newdata.FacilityIdx, () =>
            {
                NextMoveAction(CurFacilityIdxProperty.Value);
            });
        }
    }


    public void RandSetSkin()
    {
        var tdlist = Tables.Instance.GetTable<ConsumerInfo>().DataList.ToList();

        var randvalue = Random.Range(0, tdlist.Count);

        var skinname = tdlist[randvalue].skin;

        skeletonAnimation.Skeleton.SetSkin(skinname);
    }


    public void NextMoveAction(int facilityidx)
    {
        if (facilityidx > 0 && facilityidx < 100) //기본 물품대 
        {
            ChangeState(CurState.WaitProduct, facilityidx);
        }
        else if (facilityidx > 1000) // 조리대 
        {

            ChangeState(CurState.WaitProduct, facilityidx);
        }
        else if (facilityidx == 1000) //계산대
        {
            IsArrivedCounter = true;
            GameRoot.Instance.NaviSystem.CurNaviOnType = NaviSystem.NaviType.CalcCounter;
            GameRoot.Instance.NaviSystem.NextNavi(NaviSystem.NaviType.CalcCounter);

        }
    }


    public System.Numerics.BigInteger CheckRevenue()
    {
        System.Numerics.BigInteger rewardvalue = 0;

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var stagetd = Tables.Instance.GetTable<StageInfo>().GetData(stageidx);

        if (stagetd != null)
        {
            foreach (var fish in CurFishComponentList)
            {
                var td = Tables.Instance.GetTable<FishInfo>().GetData(fish.GetFishIdx);

                if (td != null)
                {
                    var finddata = GameRoot.Instance.FacilitySystem.GetFacilityUpgradeData(fish.GetFishIdx);

                    if (finddata != null)
                        rewardvalue += GameRoot.Instance.FacilitySystem.GetFishCurSellProductValue(fish.GetFishIdx, finddata.Level);
                }
            }

            rewardvalue = rewardvalue * stagetd.revenue_buff_profit;

            rewardvalue = rewardvalue / 100;
        }

        return rewardvalue;
    }


    private void OnDestroy()
    {
        disposables.Clear();
    }

    private void OnDisable()
    {
        disposables.Clear();

        if (ConsumerOrderUI != null)
        {
            Destroy(ConsumerOrderUI.gameObject);
            ConsumerOrderUI = null;
        }
    }

    public void ChangeState(CurState state, int facilityidx = -1)
    {
        State = state;

        switch (State)
        {
            case CurState.Idle:
                break;
            case CurState.Move:
                break;
            case CurState.WaitProduct:
                {
                    TargetRack = null;
                    TargetCooked = null;

                    var getfacility = Stage.FindFacility(facilityidx);

                    if (facilityidx > 1000)
                    {
                        if (getfacility != null)
                        {
                            TargetCooked = getfacility.GetComponent<CookedComponent>();
                        }
                    }
                    else
                    {
                        if (getfacility != null)
                        {
                            TargetRack = getfacility.GetComponent<RackComponent>();
                        }
                    }

                }
                break;
        }
    }

    public void GoToFacility(int facilityidx, System.Action nextaction)
    {

        if (GameRoot.Instance.InGameSystem.CounterIdx == facilityidx)
        {
            FacilityTarget = CounterComponent.GetEmptyConsumerTr();

            IsCounter = true;

            CounterComponent.AddConsumer(this);
        }
        else
            FacilityTarget = Stage.GetFacilityConsumeTr(facilityidx);


        if (FacilityTarget != null)
        {
            PlayAnimation("move", true);
            SetDestination(FacilityTarget, nextaction);
        }
        else
        {
            //wait
        }

        if (ConsumerOrderUI != null)
            ConsumerOrderUI.SetFacilityImg(facilityidx);
    }


    private void Update()
    {
        if (TargetRack != null && State == CurState.WaitProduct)
        {

            if (CurCountProperty.Value >= CurGoalValue)
            {
                MoveFacility();
            }
            else
            {
                if (TargetRack.GetFishComponentList.Count > 0 && CurFacilityIdxProperty.Value == (int)TargetRack.FacilityTypeIdx)
                {
                    var target = TargetRack.GetFishComponentList.Last();

                    TargetRack.RemoveFish();

                    AddFish(target);

                }
            }
        }
        //else if(TargetCooked != null && State == CurState.WaitProduct)
        //{
        //    if (CurCountProperty.Value >= CurGoalValue)
        //    {
        //        MoveFacility();
        //    }
        //    else
        //    {
        //        if (TargetCooked.FoodCompleteGetCount.Count > 0 && CurFacilityIdxProperty.Value == TargetCooked.FacilityIdx)
        //        {
        //            var getfish = TargetCooked.RemoveFish();

        //            AddFish(getfish);
        //        }
        //    }
        //}

        if (IsCounter && CurCounterOrder > 0)
        {
            var finddata = CounterComponent.FindOrderConsumer(CurCounterOrder - 1);

            if (finddata == null)
            {
                CurCounterOrder -= 1;
                MovementCounterConsumer(CurCounterOrder, null);
            }
        }

    }

    public void CarryStart(bool iscarry)
    {
        IsCarry = iscarry;

    }

    private float FishPos_Y = 0.15f;

    public void AddFish(FishComponent fish)
    {
        CurCountProperty.Value += 1;
        CurFishComponentList.Add(fish);
        CarryStart(CurFishComponentList.Count > 0);

        PlayAnimation("carryidle", true);


        var floory = (FishPos_Y * (CurFishComponentList.Count - 1));

        fish.FishInBucketAction(ProductRoot, (fish) =>
        {
            fish.transform.SetParent(ProductRoot);
        }, 0.25f, floory);
    }



    public void OutCounterConsumer()
    {
        ConsumerOrderUI.SetImage(ConsumerOrderUI.ConsumerState.Pay);
        SetDestination(Stage.GetMiddleEndTr, () =>
        {
            SetDestination(Stage.GetEndTr, () =>
            {
                DataClear();
                ProjectUtility.SetActiveCheck(this.gameObject, false);
                OnEnd?.Invoke(true);
                OnEnd = null;
            });
        });
    }

    public void DataClear()
    {
        if (ConsumerOrderUI != null)
            ProjectUtility.SetActiveCheck(ConsumerOrderUI.gameObject, false);

        foreach (var fish in CurFishComponentList)
        {
            fish.ClearObj();
        }
        CurFishComponentList.Clear();
        CurFacilityIdxProperty.Value = 0;
        CurCountProperty.Value = 0;
        PatternOrderQueue.Clear();
        CurFishComponentList.Clear();
        IsCarry = false;
        CurCounterOrder = 0;
        FacilityTarget = null;
        TargetRack = null;
        IsCounter = false;
        IsArrivedCounter = false;

    }

    public void MovementCounterConsumer(int order, System.Action moveendaction)
    {
        if (CounterComponent != null)
        {
            var consumertr = CounterComponent.GetConsumerTr(order);

            SetDestination(consumertr, moveendaction);
        }
    }

}
