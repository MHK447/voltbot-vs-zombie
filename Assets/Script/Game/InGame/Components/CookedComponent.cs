using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using Spine.Unity;
using System.Linq;
using UniRx;

public class CookedComponent : FacilityComponent
{
    public enum State
    {
        None,
        Working,
        Break,
        Idle,
    }

    [SerializeField]
    private Transform ProgressTr;

    [SerializeField]
    private GameObject FixObj;

    [SerializeField]
    private List<Transform> FoodTrList = new List<Transform>();

    [SerializeField]
    protected SkeletonAnimation skeletonAnimation;

    [SerializeField]
    private List<CookedMaterialComponent> CookedMaterialList = new List<CookedMaterialComponent>();

    [SerializeField]
    private Transform TroubleUITr;

    [SerializeField]
    private Transform CarryCasherWaitTr;

    [SerializeField]
    private CookTableComponent TableComponent;

    public CookTableComponent GetCookTableComponent { get { return TableComponent; } }

    [SerializeField]
    private ColActionComponent ColAction;

    [SerializeField]
    private RackComponent TargetCookedRack;

    public RackComponent GetTargetCookedRack { get { return TargetCookedRack; } }

    public Transform GetCarryCasherWaitTr { get { return CarryCasherWaitTr; } }

    private State CurState = State.None;

    private int MaterialMaxCount = 0;

    private float Cookeddeltime = 0f;

    private CooltimeProgress Progress;

    private List<OtterBase> CasherOtterList = new List<OtterBase>();

    private float movematerialdeltime = 0f;

    private int FoodIdx = 0;

    private int CurBreakCount = 0;

    private int MaxBreakCount = 0;

    private float CookingCoolTime = 0f;

    private bool IsCookStart = false;

    private CompositeDisposable disposables = new CompositeDisposable();

    private UI_TroubleBubble TroubleBubble;

    public override void Init()
    {
        base.Init();

        TableComponent.Init(FacilityData);
        IsCookStart = false;

        ColAction.TriggerEnterEvent = OnTriggerEnter2D;
        ColAction.TriggerExitEvent = OnTriggerExit2D;

        var td = Tables.Instance.GetTable<CookingInfo>().GetData((int)FacilityTypeIdx);

        ProjectUtility.SetActiveCheck(FixObj, false);

        var ingametycoon = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>();

        Player = ingametycoon.GetPlayer;

        if (td != null)
        {
            CookingCoolTime = (float)td.cooking_cooltime / 100f;

            MaxBreakCount = td.break_count;

            ChangeState(State.Idle);

            MaterialMaxCount = td.material_max_count;

            FoodIdx = td.food_idx;

            for (int i = 0; i < td.material_idxs.Count; ++i)
            {
                CookedMaterialList[i].Set(td.material_idxs[i], MaterialMaxCount, FacilityData.IsOpen);
            }

            if (Progress == null)
            {
                GameRoot.Instance.UISystem.LoadFloatingUI<CooltimeProgress>((_progress) =>
                {
                    Progress = _progress;
                    ProjectUtility.SetActiveCheck(Progress.gameObject, false);
                    Progress.Init(ProgressTr);
                    Progress.SetValue(0);
                });
            }

            disposables.Clear();

            SetCookedSpeed();

            GameRoot.Instance.UISystem.LoadFloatingUI<UI_TroubleBubble>((_progress) =>
            {
                TroubleBubble = _progress;
                ProjectUtility.SetActiveCheck(TroubleBubble.gameObject, false);
                //ProjectUtility.SetActiveCheck(AmountUI.gameObject, FacilityData.CapacityCountProperty.Value > 0);
                TroubleBubble.Init(TroubleUITr);
            });


            var donebuylist = GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.ToList().FindAll(x => x.IsBuyCheckProperty.Value == false);
            foreach (var donebuy in donebuylist)
            {
                donebuy.IsBuyCheckProperty.Subscribe(x =>
                {
                    if (donebuy.UpgradeType == (int)UpgradeSystem.UpgradeType.CookingSpeedUp)
                    {
                        SetCookedSpeed();
                    }
                }).AddTo(disposables);
            }
        }
    }




    public void SetCookedSpeed()
    {
        var td = Tables.Instance.GetTable<CookingInfo>().GetData((int)FacilityTypeIdx);

        if (td != null)
        {
            var upgradevalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.ShelfCapacityUp, (int)FacilityTypeIdx);
            var basevalue = (float)td.cooking_cooltime / 100f;
            float buffvalue = 0f;

            if (upgradevalue > 0f)
            {
                buffvalue = ProjectUtility.PercentCalc(CookingCoolTime, upgradevalue) / 100f;
                CookingCoolTime = basevalue - buffvalue;
            }
        }
    }


    public bool IsMaterialMaxCheck(int materialidx)
    {
        bool ismaxcheck = false;

        var finddata = CookedMaterialList.Find(x => x.GetFishIdx == materialidx);

        if (finddata != null)
        {
            ismaxcheck = finddata.IsMaxCheck();
        }



        return ismaxcheck;
    }


    public void ChangeState(State state)
    {
        if (CurState == state) return;

        if (skeletonAnimation == null) return;

        CurState = state;

        switch (CurState)
        {
            case State.Working:
                {
                    skeletonAnimation.state.SetAnimation(0, "work", true);
                }
                break;
            case State.Break:
                {
                    GameRoot.Instance.GameNotification.AddNoti(NoticeComponent.NoticeType.Break, this.transform);
                    skeletonAnimation.state.SetAnimation(0, "break", true);
                }
                break;
            case State.Idle:
                {
                    skeletonAnimation.state.SetAnimation(0, "idle", true);
                }
                break;
        }

    }


    public void CoolTimeActive(float cooltimevalue)
    {
        if (Progress == null) return;

        Progress.SetValue(cooltimevalue);

        if (cooltimevalue > 0f && !Progress.gameObject.activeSelf)
        {
            ProjectUtility.SetActiveCheck(Progress.gameObject, true);
        }

        if (cooltimevalue <= 0f && Progress.gameObject.activeSelf)
        {
            ProjectUtility.SetActiveCheck(Progress.gameObject, false);
        }

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOpenFacility()) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("CarryCasher"))
        {
            movematerialdeltime = 0f;
            var getvalue = collision.GetComponent<OtterBase>();

            if (getvalue != null)
            {
                if (CurState == State.Break && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    ProjectUtility.SetActiveCheck(FixObj, true);

                    if (TroubleBubble != null)
                        ProjectUtility.SetActiveCheck(TroubleBubble.gameObject, false);

                    GameRoot.Instance.GameNotification.RemoveNoti(NoticeComponent.NoticeType.Break, this.transform);

                    skeletonAnimation.state.SetAnimation(0, "idle", true);

                    GameRoot.Instance.WaitTimeAndCallback(1f, () =>
                    {
                        if (this != null)
                        {
                            ProjectUtility.SetActiveCheck(FixObj, false);
                        }
                    });

                    IsCookStart = false;
                    ChangeState(State.None);
                }
            }

            if (getvalue != null && getvalue.GetFishComponentList.Count > 0)
            {
                if (!CasherOtterList.Contains(getvalue))
                {
                    CasherOtterList.Add(getvalue);
                }
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("CarryCasher"))
        {
            movematerialdeltime = 0f;
            var getvalue = collision.GetComponent<OtterBase>();

            if (getvalue != null)
            {
                if (CasherOtterList.Contains(getvalue))
                {
                    CasherOtterList.Remove(getvalue);
                }
            }
        }
    }




    public void CheckMaterialMoveOtter()
    {
        for (int i = CasherOtterList.Count - 1; i >= 0; i--)
        {
            if (CasherOtterList[i].IsMove) continue;


            if (CasherOtterList[i].GetFishComponentList.Count > 0)
            {

                for (int j = CasherOtterList[i].GetFishComponentList.Count - 1; j >= 0; j--)
                {
                    var findfish = CasherOtterList[i].GetFishComponentList[j];

                    if (findfish != null)
                    {
                        var finddata = CookedMaterialList.Find(x => x.GetFishIdx == findfish.GetFishIdx);

                        if (finddata != null && !finddata.IsMaxCheck())
                        {
                            movematerialdeltime += Time.deltaTime;

                            if (movematerialdeltime > 0.2f)
                            {
                                movematerialdeltime = 0f;

                                CasherOtterList[i].RemoveFish(findfish);

                                findfish.FishInBucketAction(finddata.GetCurFishTr(), (fish) =>
                                {
                                    fish.transform.SetParent(this.transform);
                                    fish.transform.position = finddata.GetCurFishTr().position;
                                }, 0.2f);

                                finddata.AddMaterial(findfish);
                            }
                        }
                    }
                }
                CasherOtterList[i].ChangeState(OtterBase.OtterState.Wait);

                if (CasherOtterList[i].GetFishComponentList.Count == 0)
                {
                    CasherOtterList[i].CarryEnd();
                }
            }
        }
    }

    public void ProduceFood()
    {
        if (CurState == State.Break)
        {
            return;
        }

        if (IsMaxCountCheck())
        {
            if (IsCookStart)
                ChangeState(State.Idle);

            return;
        }

        if (!MaterialAllCountCheck())
        {
            if (IsCookStart)
                ChangeState(State.Idle);

            return;
        }


        if (!IsCookStart)
        {
            IsCookStart = true;
            ChangeState(State.Working);
        }

        Cookeddeltime += Time.deltaTime;

        var cooltimevalue = Cookeddeltime / 1.5f;

        CoolTimeActive(cooltimevalue);

        if (cooltimevalue > 1 && (MaxBreakCount > 0 || MaxBreakCount == -1))
        {
            Cookeddeltime = 0f;

            CoolTimeActive(0f);

            IsCookStart = false;

            ChangeState(State.Idle);


            if (MaxBreakCount > -1)
                CurBreakCount += 1;

            if (CurBreakCount >= MaxBreakCount && MaxBreakCount != -1)
            {
                GameRoot.Instance.WaitTimeAndCallback(2f, () =>
                {
                    BreakMachine();
                });
            }

            InGameStage.CreateFish(FoodTrList[TableComponent.FoodCompleteGetCount.Count], FoodIdx, FishComponent.State.Cook, (fish) =>
            {
                fish.transform.SetParent(FoodTrList[TableComponent.FoodCompleteGetCount.Count]);
                FoodCreateComplete(fish);
            });

            foreach (var material in CookedMaterialList)
            {
                material.RemoveMaterial();
            }
        }
    }

    public void BreakMachine()
    {

        ChangeState(State.Break);
        CurBreakCount = 0;
        Cookeddeltime = 0f;
        CoolTimeActive(0f);
        IsCookStart = false;

        if (TroubleBubble != null)
            ProjectUtility.SetActiveCheck(TroubleBubble.gameObject, true);


    }

    public void FoodCreateComplete(FishComponent fish)
    {
        TableComponent.FoodCompleteGetCount.Enqueue(fish);
    }


    public bool MaterialAllCountCheck()
    {
        foreach (var material in CookedMaterialList)
        {
            if (material.MaterialCount < 1)
            {
                return false;
            }
        }
        return true;
    }

    public bool MaterialMaxCheck(int materialidx)
    {
        var finddata = CookedMaterialList.Find(x => x.GetFishIdx == materialidx);

        if (finddata != null)
        {
            return finddata.IsMaxCheck();
        }
        return false;
    }

    public override bool IsMaxCountCheck()
    {
        return TableComponent.FoodCompleteGetCount.Count >= CapacityMaxCount;
    }


    public void Update()
    {
        CheckMaterialMoveOtter();
        ProduceFood();
    }

    private void OnDisable()
    {
        if (Progress != null)
        {
            Destroy(Progress.gameObject);
            Progress = null;
        }
    }



}
