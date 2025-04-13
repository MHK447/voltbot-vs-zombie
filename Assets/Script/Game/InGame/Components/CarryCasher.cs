using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;
using UnityEngine.AI;
using Spine.Unity;
using UniRx;

public class CarryCasher : OtterBase
{
    private float waitdeltime = 0f;

    private Queue<System.Action> WorkActionQueue = new Queue<System.Action>();

    private CompositeDisposable disposables = new CompositeDisposable();

    private float sleepdeltime = 0f;


    public override void Init()
    {
        base.Init();

        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;

        _navMeshAgent.enabled = true;

        GameRoot.Instance.StartCoroutine(WaitOneFrame());

        CurState = OtterState.Idle;

        FishComponentList.Clear();

        WorkActionQueue.Clear();

        GameRoot.Instance.WaitTimeAndCallback(1f, () => { StartWork(); });

        SetCapacity();

        // Event 콜백 등록
        skeletonAnimation.AnimationState.Complete += HandleEvent;

        var buffvalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.TransportStaffSpeedUp);

        var getcalcvalue = ProjectUtility.PercentCalc(GameRoot.Instance.InGameSystem.casher_move_speed, buffvalue);

        ProjectUtility.SetActiveCheck(SpeedUpObj.gameObject, getcalcvalue > 0);

        CasherMoveSpeed = GameRoot.Instance.InGameSystem.casher_move_speed + getcalcvalue;

        var donebuylist = GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.ToList().FindAll(x => x.IsBuyCheckProperty.Value == false);

        disposables.Clear();

        foreach (var donebuy in donebuylist)
        {
            donebuy.IsBuyCheckProperty.Subscribe(x =>
            {
                if (donebuy.UpgradeType == (int)UpgradeSystem.UpgradeType.TransportStaffSpeedUp)
                {
                    var buffvalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.TransportStaffSpeedUp);

                    var getcalcvalue = ProjectUtility.PercentCalc(GameRoot.Instance.InGameSystem.casher_move_speed, buffvalue);

                    CasherMoveSpeed = GameRoot.Instance.InGameSystem.casher_move_speed + getcalcvalue;

                    if (SpeedUpObj != null)
                    {
                        ProjectUtility.SetActiveCheck(SpeedUpObj.gameObject, buffvalue > 0);
                    }
                }
            }).AddTo(disposables);
        }
    }


    public void StartWork()
    {
        CarryCasherWorkFacilityIdx = -1;

        if (TargetWorkFacility())
        {
            ChangeState(OtterState.Work);
            NextWorkAction();
        }
        else
        {
            waitdeltime = 0f;
            ChangeState(OtterState.Wait);
        }

        Debug.Log($"[CarryCasher] StartWork called. FoundTarget: {CarryCasherWorkFacilityIdx}");
    }

    public override void AddFish(FishComponent fish)
    {
        base.AddFish(fish);
    }

    public bool TargetWorkFacility()
    {
        var facilitydatas = GameRoot.Instance.UserData.CurMode.StageData.StageFacilityDataList
            .Where(x => x.IsOpen && (x.FacilityIdx > 100)).ToList();

        // 가능한 작업들 리스트
        var availableWorks = new List<System.Action>();

        for (int i = facilitydatas.Count - 1; i >= 0; i--)
        {
            var facility = facilitydatas[i];
            var facilityInfo = Tables.Instance.GetTable<FacilityInfo>().GetData(facility.FacilityIdx);
            if (facilityInfo == null) continue;

            if (CurStage == null)
            {
                CurStage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;
            }

            var mainFacility = CurStage.FindFacility(facilityInfo.facilityidx);
            if (mainFacility == null) continue;

            if (CurStage.IsWorkCasherFacilityCheck(facility.FacilityIdx)) continue;

            if (HandleCookedToDisplayCheck(facility.FacilityIdx))
            {
                availableWorks.Add(() =>
                {
                    CarryCasherWorkFacilityIdx = facility.FacilityIdx;
                    CookedToDisplayStart(facility.FacilityIdx);
                });
            }

            if (HandleFishCookedDisplay(facility.FacilityIdx))
            {
                availableWorks.Add(() =>
                {
                    CarryCasherWorkFacilityIdx = facility.FacilityIdx;
                    StartFishCookedDisplay(facility.FacilityIdx);
                });
            }

            if (HandleFishDisplay(facility.FacilityIdx, mainFacility))
            {
                availableWorks.Add(() =>
                {
                    CarryCasherWorkFacilityIdx = facility.FacilityIdx;
                    StartFishDisplay(facility.FacilityIdx, mainFacility);
                });
            }

            // 가능한 작업이 있으면 랜덤하게 하나 선택
        }
        if (availableWorks.Count > 0)
        {
            var randomIdx = UnityEngine.Random.Range(0, availableWorks.Count);
            availableWorks[randomIdx].Invoke();
            return true;
        }

        return false;
    }


    private bool HandleFishDisplay(int facilityidx, FacilityComponent mainFacility)
    {
        var facilitytd = Tables.Instance.GetTable<FacilityInfo>().GetData(facilityidx);

        if (facilitytd != null)
        {
            var rackfacility = CurStage.FindFacility(facilitytd.rack_group);
            if (rackfacility == null) return false;

            if (rackfacility.IsMaxCountCheck()) return false;

            var fishRoom = mainFacility.GetComponent<FishRoomComponent>();
            if (fishRoom == null || mainFacility.GetFacilityData.CapacityCountProperty.Value <= 0) return false;

            return true;
        }

        return false;
    }

    public void StartFishDisplay(int facilityidx, FacilityComponent mainFacility)
    {
        var facilitytd = Tables.Instance.GetTable<FacilityInfo>().GetData(facilityidx);

        if (facilitytd != null)
        {
            var rackfacility = CurStage.FindFacility(facilitytd.rack_group);

            if (rackfacility == null) return;

            var fishRoom = mainFacility.GetComponent<FishRoomComponent>();

            if (fishRoom == null || mainFacility.GetFacilityData.CapacityCountProperty.Value <= 0) return;

            EnqueueFishDisplayActions(fishRoom, rackfacility);
        }
    }


    private bool HandleFishCookedDisplay(int facilityidx)
    {
        var cookedfacility = CurStage.FindFacility(facilityidx);
        if (cookedfacility == null) return false;

        var facilitytd = Tables.Instance.GetTable<FacilityInfo>().GetData(facilityidx);

        if (facilitytd == null) return false;

        var cookcomponent = cookedfacility.GetComponent<CookedComponent>();

        if (cookcomponent == null) return false;

        var cookedtd = Tables.Instance.GetTable<CookingInfo>().GetData((int)cookcomponent.FacilityTypeIdx);

        if (cookedtd == null) return false;

        foreach (var materialidx in cookedtd.material_idxs)
        {
            if (!cookcomponent.IsMaterialMaxCheck(materialidx))
            {
                var findfacilitytd = Tables.Instance.GetTable<FishInfo>().GetData(materialidx);

                if (findfacilitytd != null)
                {
                    var findfishroom = CurStage.FindFacility(findfacilitytd.fish_rack_idx);

                    if (findfishroom != null)
                    {
                        var fishRoom = findfishroom.GetComponent<FishRoomComponent>();

                        if (fishRoom != null && fishRoom.GetBucketComponent.GetFishCount > 0 && !cookcomponent.GetTargetCookedRack.IsMaxCountCheck())
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public void StartFishCookedDisplay(int facilityidx)
    {
        var cookedfacility = CurStage.FindFacility(facilityidx);
        var cookcomponent = cookedfacility.GetComponent<CookedComponent>();

        var facilitytd = Tables.Instance.GetTable<FacilityInfo>().GetData(facilityidx);
        var cookedtd = Tables.Instance.GetTable<CookingInfo>().GetData((int)cookcomponent.FacilityTypeIdx);


        foreach (var materialidx in cookedtd.material_idxs)
        {
            if (!cookcomponent.IsMaterialMaxCheck(materialidx))
            {
                var findfacilitytd = Tables.Instance.GetTable<FishInfo>().GetData(materialidx);

                if (findfacilitytd != null)
                {
                    var findfishroom = CurStage.FindFacility(findfacilitytd.fish_rack_idx);

                    if (findfishroom != null)
                    {
                        var fishRoom = findfishroom.GetComponent<FishRoomComponent>();

                        if (fishRoom != null && fishRoom.GetBucketComponent.GetFishCount > 0 && !cookcomponent.GetTargetCookedRack.IsMaxCountCheck())
                        {
                            EnqueueFishDisplayActions(fishRoom, cookcomponent);
                        }
                    }
                }
            }
        }
    }



    private bool HandleCookedToDisplayCheck(int facilityidx)
    {
        var cookedfacility = CurStage.FindFacility(facilityidx);
        if (cookedfacility == null) return false;

        if (cookedfacility.IsOpenFacility() == false) return false;

        var facilitytd = Tables.Instance.GetTable<FacilityInfo>().GetData(facilityidx);

        if (facilitytd == null) return false;

        var cookcomponent = cookedfacility.GetComponent<CookedComponent>();

        if (cookcomponent == null) return false;


        if (cookcomponent.GetCookTableComponent.FoodCompleteGetCount.Count > 0 && !cookcomponent.GetTargetCookedRack.IsMaxCountCheck())
        {
            return true;
        }


        return false;
    }

    public void CookedToDisplayStart(int facilityidx)
    {
        var cookedfacility = CurStage.FindFacility(facilityidx);

        var cookcomponent = cookedfacility.GetComponent<CookedComponent>();

        if (cookcomponent.GetCookTableComponent.FoodCompleteGetCount.Count > 0 && !cookcomponent.GetTargetCookedRack.IsMaxCountCheck())
        {
            EnqueueCookDiplayActions(cookcomponent, cookcomponent.GetTargetCookedRack);
        }
    }



    private void EnqueueCookDiplayActions(CookedComponent cookcomponent, RackComponent rackcomponent)
    {
        WorkActionQueue.Clear();
        System.Action moveToCooked = () =>
        {
            SetDestination(cookcomponent.GetCookTableComponent.GetCasherTr.transform, () =>
            {
                GameRoot.Instance.StartCoroutine(CheckWaitProductMax(NextWorkAction));
            });
        };
        WorkActionQueue.Enqueue(moveToCooked);

        System.Action moveToDisplay = () =>
        {
            SetDestination(rackcomponent.GetCarryCasherWaitTr(this.transform), () =>
            {
                PlayAnimation(OtterState.Idle, "idle", true);
            });
            GameRoot.Instance.StartCoroutine(CheckWaitProductNone(NextWorkAction, rackcomponent));
        };
        WorkActionQueue.Enqueue(moveToDisplay);

        System.Action WaitToWork = () =>
        {
            if (FishComponentList.Count > 0)
            {
                GameRoot.Instance.StartCoroutine(CheckWaitTrashCan(() =>
                {
                    if (FishComponentList.Count > 0)
                    {
                        GoToTrashCan(() =>
                        {
                        });
                    }
                    else
                        PlayAnimation(OtterState.Wait, "idle", true);
                }));

            }
            else
                PlayAnimation(OtterState.Wait, "idle", true);
        };
        WorkActionQueue.Enqueue(WaitToWork);
    }


    private void EnqueueFishDisplayActions(FishRoomComponent fishRoom, CookedComponent targetdisplay)
    {
        WorkActionQueue.Clear();
        System.Action moveToBucket = () =>
        {
            SetDestination(fishRoom.GetBucketCarryCasherTr.transform, () =>
            {
                GameRoot.Instance.StartCoroutine(CheckWaitProductMax(NextWorkAction));
            });
        };
        WorkActionQueue.Enqueue(moveToBucket);

        System.Action moveToDisplay = () =>
        {
            SetDestination(targetdisplay.GetCarryCasherWaitTr, () =>
            {
                PlayAnimation(OtterState.Idle, "idle", true);
            });
            GameRoot.Instance.StartCoroutine(CheckWaitProductNone(NextWorkAction, targetdisplay));
        };
        WorkActionQueue.Enqueue(moveToDisplay);

        System.Action WaitToWork = () =>
        {
            if (FishComponentList.Count > 0)
            {
                if (FishComponentList.Count > 0)
                {
                    GameRoot.Instance.StartCoroutine(CheckWaitTrashCan(() =>
                    {
                        if (FishComponentList.Count > 0)
                        {
                            GoToTrashCan(() =>
                            {
                            });
                        }
                        else
                            PlayAnimation(OtterState.Wait, "idle", true);
                    }));
                }
                else
                    PlayAnimation(OtterState.Wait, "idle", true);
            }
            else
                PlayAnimation(OtterState.Wait, "idle", true);
        };
        WorkActionQueue.Enqueue(WaitToWork);
    }


    private void EnqueueFishDisplayActions(FishRoomComponent fishRoom, FacilityComponent targetdisplay)
    {
        System.Action moveToBucket = () =>
        {
            if (this == null) return;
            if (!this.gameObject.activeSelf) return;

            SetDestination(fishRoom.GetBucketCarryCasherTr.transform, () =>
            {
                GameRoot.Instance.StartCoroutine(CheckWaitProductMax(NextWorkAction));
            });
        };
        WorkActionQueue.Enqueue(moveToBucket);

        var rackcomponent = targetdisplay.GetComponent<RackComponent>();

        System.Action moveToDisplay = () =>
        {
            if (this == null) return;
            if (!this.gameObject.activeSelf) return;

            SetDestination(rackcomponent.GetCarryCasherWaitTr(this.transform), () =>
            {
                PlayAnimation(OtterState.Idle, "idle", true);
            });
            GameRoot.Instance.StartCoroutine(CheckWaitProductNone(NextWorkAction, rackcomponent));
        };
        WorkActionQueue.Enqueue(moveToDisplay);

        System.Action WaitToWork = () =>
        {
            if (this == null) return;
            if (!this.gameObject.activeSelf) return;

            if (FishComponentList.Count > 0)
            {
                GameRoot.Instance.StartCoroutine(CheckWaitTrashCan(() =>
                {
                    if (FishComponentList.Count > 0)
                    {
                        GoToTrashCan(() =>
                        {
                        });
                    }
                    else
                        PlayAnimation(OtterState.Wait, "idle", true);
                }));
            }
            else
                PlayAnimation(OtterState.Wait, "idle", true);
        };
        WorkActionQueue.Enqueue(WaitToWork);
    }

    public void NextWorkAction()
    {
        if (WorkActionQueue.Count > 0)
        {
            var nextaction = WorkActionQueue.Dequeue();
            nextaction?.Invoke();
        }
        else
        {
            // 액션 다 끝났는데 상태가 Idle이라면, 바로 다음 일거리 찾기 시도
            if (CurState == OtterState.Idle || CurState == OtterState.Wait)
            {
                StartWork();
            }
        }
        Debug.Log($"[CarryCasher] NextWorkAction dequeued. QueueCount: {WorkActionQueue.Count}");
    }

    public void StartWorkCheck()
    {
        if (CurState == OtterState.Sleep || CurState == OtterState.SleepMove) return;
        if (IsSleepStart) return; // napend 진행 중이면 아예 무시

        if (CurState == OtterState.Wait || (CurState == OtterState.Idle && WorkActionQueue.Count == 0))
        {
            waitdeltime += Time.deltaTime;

            if (waitdeltime >= 3f)
            {
                waitdeltime = 0f;
                StartWork();
            }
        }
    }
    private bool IsGoingTrashCan = false;
    public override void Update()
    {
        base.Update();

        sleepdeltime += Time.deltaTime;

        if (sleepdeltime >= GameRoot.Instance.InGameSystem.carry_sleep_time)
        {
            if (FishComponentList.Count == 0
          && !IsSleepStart
          && !IsGoingTrashCan)
            {
                WorkActionQueue.Clear();
                sleepdeltime = 0f;
                ChangeState(OtterState.SleepMove);
                SetDestination(CurStage.CarrySleepTr, () =>
                {
                    GameRoot.Instance.GameNotification.AddNoti(NoticeComponent.NoticeType.Nap, this.transform);
                    PlayAnimation(OtterState.Sleep, "napstart", false);
                });

                return;
            }
        }
        else if ((CurState == OtterState.Idle || CurState == OtterState.Wait) && WorkActionQueue.Count >= 6 && FishComponentList.Count == 0)
        {
            WorkActionQueue.Clear();
        }


        StartWorkCheck();
    }

    public void GoToTrashCan(System.Action endaction)
    {
        IsGoingTrashCan = true;
        SetDestination(CurStage.GetTrashCanComponent.GetConsumerTr, () =>
        {
            IsGoingTrashCan = false;
            WorkActionQueue.Clear();
            // ⭐ 쓰레기통 도착 시 Fish 비우기

            endaction?.Invoke();
        });
    }
    private float CheckDuration = 3f;

    private IEnumerator CheckWaitProductMax(System.Action nextaction)
    {
        float elapsedTime = 0f;

        while (elapsedTime < CheckDuration)
        {
            if (FishComponentList.Count >= StartCarryCount)
            {
                nextaction?.Invoke();
                yield break; // 코루틴 종료
            }

            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        nextaction?.Invoke();
    }

    private IEnumerator CheckWaitProductNone(System.Action nextaction, RackComponent rackComponent)
    {
        float elapsedTime = 0f;
        float timeout = 3f;

        while (elapsedTime < timeout)
        {
            if (FishComponentList.Count == 0 || rackComponent.IsMaxCountCheck())
            {
                nextaction?.Invoke();
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        nextaction?.Invoke();
        Debug.Log($"[CarryCasher] CheckWaitProductNone running, FishCount: {FishComponentList.Count}");
    }


    private IEnumerator CheckWaitProductNone(System.Action nextaction, CookedComponent cookedcomponent)
    {
        if (FishComponentList.Count == 0)
        {
            nextaction?.Invoke();
            yield break;
        }


        for (int i = FishComponentList.Count - 1; i >= 0; i--)
        {
            var fish = FishComponentList[i];
            var fishidx = fish.GetFishIdx;

            // material이 max 값이면, 해당 항목을 리스트에서 제거하고 다음으로 진행
            if (cookedcomponent.IsMaterialMaxCheck(fishidx))
            {
                FishComponentList.RemoveAt(i);  // 리스트에서 항목 제거
                Debug.Log($"[CarryCasher] Removed fish with idx: {fishidx}");

                // 한 번 제거된 후, 다시 체크할 필요가 없으므로 바로 종료
                nextaction?.Invoke();
                yield break;
            }
        }

        yield return new WaitUntil(() => FishComponentList.Count <= 0);
        nextaction?.Invoke();
        Debug.Log($"[CarryCasher] CheckWaitProductNone running, FishCount: {FishComponentList.Count}");
    }

    private IEnumerator CheckWaitTrashCan(System.Action nextaction)
    {
        if (FishComponentList.Count == 0)
        {
            nextaction?.Invoke();
            yield break;
        }

        float timeout = Time.time + 1f;

        yield return new WaitUntil(() => FishComponentList.Count == 0 || Time.time >= timeout);

        nextaction?.Invoke();
        Debug.Log($"[CarryCasher] CheckWaitProductNone running, FishCount: {FishComponentList.Count}");
    }




    private void HandleEvent(Spine.TrackEntry trackEntry)
    {
        switch (trackEntry.Animation.Name)
        {
            case "fishingstart":
                {
                    PlayAnimation(OtterState.Fishing, "fishingidle", true);
                }
                break;
            case "napstart":
                {
                    skeletonAnimation.state.SetAnimation(0, "napidle", true);
                }
                break;
            case "napend":
                {
                    Debug.Log("[CarryCasher] napend - 깨어나는 중!");

                    sleepdeltime = 0f; // ⭐ Sleep 시간 초기화

                    WorkActionQueue.Clear(); // 혹시 남아있던 일 초기화

                    ChangeState(OtterState.Idle); // ⭐ 명확하게 Idle로 전환
                    PlayAnimation(OtterState.Idle, "idle", true);

                    // 바로 일 시작
                    GameRoot.Instance.WaitTimeAndCallback(0.5f, () =>
                    {
                        IsSleepStart = false;
                        Debug.Log("[CarryCasher] napend 끝, StartWork 호출");
                        StartWork();

                    });
                }
                break;
        }
        AnimAction?.Invoke();

        AnimAction = null;
    }

    private bool IsSleepStart = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CurState == OtterState.Sleep && !IsSleepStart)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                sleepdeltime = 0f;

                IsSleepStart = true;

                skeletonAnimation.state.SetAnimation(0, "napend", false);

                GameRoot.Instance.GameNotification.RemoveNoti(NoticeComponent.NoticeType.Nap, this.transform);
            }
        }
    }

    void OnDisable()
    {
        WorkActionQueue.Clear();
        disposables.Clear();
        // 콜백 해제
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationState.Complete -= HandleEvent;
        }
    }

    private void OnDestroy()
    {
        WorkActionQueue.Clear();
        disposables.Clear();
        // 콜백 해제
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationState.Complete -= HandleEvent;
        }
    }
}
