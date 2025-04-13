using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Spine.Unity;
using UniRx;
using BanpoFri;

public class FishCasher : OtterBase
{
    private Transform TargetComponent;

    private float waitdeltime = 0f;

    private int MaxProductCount = 5;

    private Queue<System.Action> WorkActionQueue = new Queue<System.Action>();

    Coroutine _currentMoveProcess;

    WaitForSeconds _waitTick;
    private bool isFishing = false;

    private int FacilityIdx = 0;

    public int GetFacilityIdx { get { return FacilityIdx; } }

    private FishRoomComponent FishRoomComponent;

    private CompositeDisposable disposables = new CompositeDisposable();

    public override void Init()
    {
        base.Init();


        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;

        _navMeshAgent.enabled = true;

        GameRoot.Instance.StartCoroutine(WaitOneFrame());

        CurState = OtterState.Idle;

        CurStage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;

        FishComponentList.Clear();

        WorkActionQueue.Clear();
    }


    public void Set(int facilityidx)
    {
        FacilityIdx = facilityidx;

        CurState = OtterState.Sleep;


        var findfacility = CurStage.FindFacility(facilityidx);

        if (findfacility != null)
        {
            FishRoomComponent = findfacility.GetComponent<FishRoomComponent>();

            disposables.Clear();

            FishRoomComponent.GetFacilityData.CapacityCountProperty.SkipLatestValueOnSubscribe().Subscribe(x =>
            {
                if (FishRoomComponent.IsMaxCountCheck())
                {
                    isFishing = false;
                    PlayAnimation(OtterState.Sleep, "napstart", false);
                }
                else if (!isFishing)
                {
                    isFishing = true;
                    PlayAnimation(OtterState.Idle, "fishingidle", false);
                }

            }).AddTo(disposables);
        }

        GameRoot.Instance.WaitTimeAndCallback(1f, () => { StartWork(); });

        // Event 콜백 등록
        skeletonAnimation.AnimationState.Complete += HandleEvent;
    }

    public void StartWork()
    {
        SetDestination(FishRoomComponent.GetCushionComponent.GetFishCasherTr, () =>
        {
           //FishRoomComponent.GetCushionComponent.ChangeTarget(this);
            if (FishRoomComponent.IsMaxCountCheck())
            {
                PlayAnimation(OtterState.Sleep, "napstart", false);
                isFishing = false; // 낚시 끝
            }
            else if (!isFishing)
            {
                PlayAnimation(OtterState.Idle, "fishingidle", false);
                isFishing = true; // 낚시 시작
            }
        });
    }



    private void OnDestroy()
    {
        disposables.Clear();
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationState.End -= HandleEvent;
        }
        isFishing = false; // Destroy 시 초기화
    }


    public override void SetPlayerSpeed()
    {
        var vehicleidx = GameRoot.Instance.UserData.CurMode.PlayerData.VehiclePropertyIdx.Value;

        var td = Tables.Instance.GetTable<VehicleInfo>().GetData(vehicleidx);

        var buffvalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.PlayerSpeedUp, FacilityIdx);

        var getcalcvalue = ProjectUtility.PercentCalc(GameRoot.Instance.InGameSystem.casher_move_speed, buffvalue);

        if (td != null)
        {
            PlayerSpeed = default_player_speed + ProjectUtility.PercentCalc(default_player_speed, td.buff_value) + getcalcvalue;
        }
        else
        {
            PlayerSpeed = default_player_speed + getcalcvalue;
        }
    }


    private void OnDisable()
    {
        disposables.Clear();
        isFishing = false; // Disable 시 초기화
    }
    public override void AddFish(FishComponent fish)
    {
    }


    private void HandleEvent(Spine.TrackEntry trackEntry)
    {
        switch (trackEntry.Animation.Name)
        {
            case "fishingstart":
                {
                    if (!isFishing)
                    {
                        PlayAnimation(OtterState.Fishing, "fishingidle", true);
                        isFishing = true;
                    }
                }
                break;
            case "napstart":
                {
                    //PlayAnimation(OtterState.Sleep, "napidle", true);
                    skeletonAnimation.state.SetAnimation(0, "napidle", true);
                    isFishing = false;
                }
                break;
            case "napend":
                {
                    // fishingstart 실행 전에 이미 isFishing false로 초기화되어서 다시 루프 탈 가능성
                    if (!isFishing)
                    {
                        PlayAnimation(OtterState.Fishing, "fishingstart", false); // 여기서 false 루프 X
                    }
                }
                break;
            case "fishingidle":
                {
                    // fishingidle 자체에서는 아무것도 하지 않도록!
                    // 이 부분이 중요 (루프마다 Complete 이벤트 호출되니까)
                }
                break;
        }
        AnimAction?.Invoke();
        AnimAction = null;
    }



}
