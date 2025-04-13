using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Linq;
using BanpoFri;

public enum CasherType
{
    CounterCasher = 1,
    CarryCasher = 2,
    FishingCasher = 3,
}

public class InGameStage : MonoBehaviour
{
    public bool IsLoadComplete { get; private set; }

    private CompositeDisposable disposable = new CompositeDisposable();

    [SerializeField]
    private List<Transform> WaitLineListTr = new List<Transform>();

    [SerializeField]
    private List<Transform> StartWayPointTrList = new List<Transform>();

    [SerializeField]
    private Transform EndTr;

    public Transform GetEndTr { get { return EndTr; } }

    [SerializeField]
    private Transform MiddleEndTr;

    public Transform GetMiddleEndTr { get { return MiddleEndTr; } }

    [SerializeField]
    private AssetReference FishRef;

    [SerializeField]
    private AssetReference ConsumerRef;

    [SerializeField]
    private List<Transform> AdVehicleTrList = new List<Transform>();

    [SerializeField]
    private List<FishRoomComponent> FishRoomList = new List<FishRoomComponent>();

    [SerializeField]
    private List<FacilityComponent> FacilityList = new List<FacilityComponent>();

    [SerializeField]
    private TrashCanComponent TrashCanComponent;

    public TrashCanComponent GetTrashCanComponent { get { return TrashCanComponent; } }

    [SerializeField]
    private List<RackComponent> RackComponentList = new List<RackComponent>();

    [SerializeField]
    private List<CookedComponent> CookComponentList = new List<CookedComponent>();

    public Transform CounterCasherTr;

    [SerializeField]
    private CounterComponent CounterComponent;

    public Transform CarrySleepTr;

    public CounterComponent GetCounterComponent { get { return CounterComponent; } }

    public Transform GetStartWayPoint { get { return StartWayPointTrList[0]; } }

    private ObjectPool<FishComponent> FishPool = new ObjectPool<FishComponent>();

    private ObjectPool<Consumer> ConsumerPool = new ObjectPool<Consumer>();

    private List<FishComponent> activeFishObjs = new List<FishComponent>();

    private List<OtterBase> activeCashers = new List<OtterBase>();

    private List<Consumer> activeConsumerObjs = new List<Consumer>();


    private AdVehicleComponent AdVehicleComponent;

    public void Init()
    {
        IsLoadComplete = false;
        disposable.Clear();
        FishPool.Init(FishRef, this.transform, 40);
        ConsumerPool.Init(ConsumerRef, this.transform, 5);
        CreatePoolCasher(10);


        CounterComponent.Init();

        TrashCanComponent.Init();

        foreach (var fishroom in FishRoomList)
        {
            fishroom.Init();
        }

        foreach (var rackcomponent in RackComponentList)
        {
            rackcomponent.Init();
        }

        foreach (var cook in CookComponentList)
        {
            cook.Init();
        }

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var td = Tables.Instance.GetTable<StageInfo>().GetData(stageidx);

        var findfirstcomponent = FindFacility(td.consumerfirst_idx);



        if (findfirstcomponent != null && findfirstcomponent.IsOpenFacility())
        {
            GameRoot.Instance.WaitTimeAndCallback(2f, () =>
            {
                GameRoot.Instance.UpgradeSystem.StartUpgradeCheck();

                var upgradevalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.AddCustomer); //기본 베이스가 호출됨 

                if (GameRoot.Instance.UserData.CurMode.StageData.StageIdx == 1)
                {
                    var getui = GameRoot.Instance.UISystem.GetUI<HUDTotal>();

                    if (getui != null)
                    {
                        ProjectUtility.SetActiveCheck(getui.GetUpgradeBtnTr.gameObject, true);
                    }
                }

                for (int i = 0; i < upgradevalue; ++i)
                {
                    CreateConsumer(1, StartWayPointTrList[i]);
                }
            });
        }
    }


    public void ReturnMainScreen()
    {
        GameRoot.Instance.UserData.CurMode.GachaCoin.Value = 0;
    }

    private void OnDestroy()
    {
        disposable.Clear();
    }


    public void CreateFish(Transform starttr, int fishidx, FishComponent.State state, System.Action<FishComponent> fishcallback = null)
    {
        FishPool.Get((obj) =>
        {
            obj.transform.position = starttr.position;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            activeFishObjs.Add(obj);
            obj.OnEnd += (complete) =>
            {
                obj.transform.SetParent(this.transform);
                FishPool.Return(obj);
                activeFishObjs.Remove(obj);
            };

            obj.Set(fishidx, state);
            fishcallback?.Invoke(obj);
        });
    }


    public void CreateConsumer(int consumeridx, Transform starttr, System.Action<Consumer> consumercallback = null)
    {
        ConsumerPool.Get((obj) =>
        {
            obj.transform.position = starttr.position;
            activeConsumerObjs.Add(obj);
            obj.OnEnd += (complete) =>
            {
                CreateConsumer(1, starttr);
                ConsumerPool.Return(obj);
                activeConsumerObjs.Remove(obj);
            };

            obj.Init(consumeridx);

            consumercallback?.Invoke(obj);
        });
    }


    public void CreatePoolCasher(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            Addressables.InstantiateAsync("CarryCasher").Completed += (handle) =>
            {
                var casher = handle.Result.GetComponent<CarryCasher>();

                if (casher != null)
                {
                    activeCashers.Add(casher);
                    ProjectUtility.SetActiveCheck(casher.gameObject, false);
                }

            };

            Addressables.InstantiateAsync("FishCasher").Completed += (handle) =>
            {
                var casher = handle.Result.GetComponent<FishCasher>();

                if (casher != null)
                {
                    activeCashers.Add(casher);
                    ProjectUtility.SetActiveCheck(casher.gameObject, false);
                }

            };

            Addressables.InstantiateAsync("CounterCasher").Completed += (handle) =>
            {
                var casher = handle.Result.GetComponent<CounterCasher>();

                if (casher != null)
                {
                    activeCashers.Add(casher);
                    ProjectUtility.SetActiveCheck(casher.gameObject, false);
                }

            };
        }
    }

    public OtterBase ActiveCarryCasher(CasherType type)
    {
        var finddata = activeCashers.Find(x => x.gameObject.activeSelf == false && x.GetCasherIdx == (int)type);

        if (finddata != null)
        {
            return finddata;
        }


        return null;
    }


    public bool IsWorkCasherFacilityCheck(int facilityidx)
    {
        return activeCashers.Find(x => x.CarryCasherWorkFacilityIdx == facilityidx) != null;
    }

    public OtterBase FindCasher(CasherType type, int facilityidx)
    {
        OtterBase casher = null;

        if (this == null) return null;

        if (activeCashers.Count == 0) return null;

        if (activeCashers.Any(item => item == null)) return null;

        switch (type)
        {
            case CasherType.FishingCasher:
                {
                    var findcashers = activeCashers.FindAll(x => x.GetCasherIdx == (int)type && x.gameObject.activeSelf);


                    foreach (var fishcasher in findcashers)
                    {
                        if (fishcasher.GetComponent<FishCasher>().GetFacilityIdx == facilityidx)
                        {
                            casher = fishcasher;
                        }
                    }
                }
                break;
            case CasherType.CounterCasher:
                {
                    var findcashers = activeCashers.Find(x => x.GetCasherIdx == (int)type && x.gameObject.activeSelf);

                    if (findcashers != null)
                    {
                        casher = findcashers;
                    }
                }
                break;
        }

        return casher;
    }



    public Transform GetWaitLine(int order)
    {
        if (WaitLineListTr.Count > order)
        {
            return WaitLineListTr[order];
        }

        return WaitLineListTr.First();
    }


    public Transform GetFacilityConsumeTr(int facilityidx)
    {
        var finddata = FacilityList.Find(x => (int)x.FacilityTypeIdx == (int)facilityidx);

        if (finddata != null)
        {
            return finddata.GetConsumerTr();
        }

        return null;
    }

    public FacilityComponent GetOpenConsumerFacility()
    {
        var finddatalist = FacilityList.FindAll(x => (int)x.FacilityTypeIdx > 0 && (int)x.FacilityTypeIdx < 100 && x.IsOpenFacility());

        if (finddatalist.Count == 0) return null;

        var randvalue = UnityEngine.Random.Range(0, finddatalist.Count);

        return finddatalist[randvalue];
    }



    public FacilityComponent FindFacility(int facilityidx)
    {
        var finddata = FacilityList.Find(x => (int)x.FacilityTypeIdx == facilityidx);

        if (finddata != null)
        {
            return finddata;
        }

        return null;
    }

    public void ActiveOffVehicle()
    {
        if (AdVehicleComponent != null)
        {
            AdVehicleComponent.ActiveOff();
        }
    }


    public void ActiveOnAdVehicle()
    {
        var ranvalue = UnityEngine.Random.Range(0, AdVehicleTrList.Count);
        var randpos = GameRoot.Instance.VehicleSystem.AdVehiclePoints[ranvalue];

        GameRoot.Instance.VehicleSystem.IsShowAdVehicle = true;

        if (AdVehicleComponent == null)
        {
            Addressables.InstantiateAsync("Ad_Vehicle").Completed += (handle) =>
              {
                  var vehiclecomponent = handle.Result.GetComponent<AdVehicleComponent>();

                  if (vehiclecomponent != null)
                  {
                      AdVehicleComponent = vehiclecomponent;
                      AdVehicleComponent.transform.position = AdVehicleTrList[ranvalue].position;
                      vehiclecomponent.Init();
                      ProjectUtility.SetActiveCheck(AdVehicleComponent.gameObject, true);

                      GameRoot.Instance.GameNotification.AddNoti(NoticeComponent.NoticeType.Seaweed, AdVehicleComponent.transform);
                  }

              };
        }
        else
        {
            AdVehicleComponent.Init();
            AdVehicleComponent.transform.position = AdVehicleTrList[ranvalue].position;
            ProjectUtility.SetActiveCheck(AdVehicleComponent.gameObject, true);
            GameRoot.Instance.GameNotification.AddNoti(NoticeComponent.NoticeType.Seaweed, AdVehicleComponent.transform);
        }
    }
}
