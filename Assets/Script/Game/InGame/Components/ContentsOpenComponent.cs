using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using UniRx;
using System.Linq;
public class ContentsOpenComponent : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer FacilitySprite;

    [SerializeField]
    private Transform NewRoot;

    [SerializeField]
    private Transform MoneyRootTr;

    private NewFacilityUI NewFacilityUI;

    private bool OnEnter = false;

    private int MoneySpeedCount = 0;

    private float FacilityOpenSpeed = 0.2f;

    private FacilityData FacilityData;

    private CompositeDisposable disposables = new CompositeDisposable();

    private int FacilityOpenOrder = 0;

    private int GoalCount = 0;

    private System.Action OpenAction = null;

    private OtterBase Player = null;

    private bool IsNoneFocusTargetFacility = false;

    public void Set(FacilityData facilitydata, System.Action openaction)
    {
        disposables.Clear();

        int curstageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        FacilityOpenOrder = Tables.Instance.GetTable<StageFacilityInfo>().DataList.ToList().Find(x => x.stageidx == curstageidx
        && facilitydata.FacilityIdx == x.facilityidx).openorder;


        IsNoneFocusTargetFacility =  curstageidx == 1 && FacilityOpenOrder == 3;

        var openorder = GameRoot.Instance.UserData.CurMode.StageData.NextFacilityOpenOrderProperty;

        ProjectUtility.SetActiveCheck(this.gameObject, !facilitydata.IsOpen
                        && FacilityOpenOrder == openorder.Value);

        if (facilitydata.IsOpen) return;

        FacilityData = facilitydata;

        OpenAction = openaction;

        var ingametycoon = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>();

        Player = ingametycoon.GetPlayer;

        var stagefacilitytd = Tables.Instance.GetTable<StageFacilityInfo>().DataList.Where(x => x.facilityidx == facilitydata.FacilityIdx
     && x.stageidx == curstageidx).FirstOrDefault();

        var faccilitytd = Tables.Instance.GetTable<FacilityInfo>().GetData(facilitydata.FacilityIdx);

        if (stagefacilitytd == null) return;

        GoalCount = stagefacilitytd.open_cost;

        if (FacilitySprite != null)
            FacilitySprite.sprite = Config.Instance.GetIngameImg(faccilitytd.image);
            
        openorder.SkipLatestValueOnSubscribe().Subscribe(x =>
        {
            if (NewFacilityUI != null)
            {
                if (!FacilityData.IsOpen && FacilityOpenOrder == openorder.Value && !IsNoneFocusTargetFacility)
                {
                    GameRoot.Instance.WaitTimeAndCallback(1f, () =>
                    {
                        GameRoot.Instance.InGameSystem.CurInGame.IngameCamera.FoucsPosition(NewFacilityUI.transform);
                    });
                    GameRoot.Instance.WaitTimeAndCallback(3f, () =>
                    {
                        GameRoot.Instance.InGameSystem.CurInGame.IngameCamera.FocusOff();
                    });
                }

                ProjectUtility.SetActiveCheck(this.gameObject, !FacilityData.IsOpen
                                && FacilityOpenOrder == openorder.Value);

                ProjectUtility.SetActiveCheck(NewFacilityUI.gameObject, x == FacilityOpenOrder &&
                    !FacilityData.IsOpen);
            }
        }).AddTo(disposables);



        if (NewFacilityUI == null)
        {
            GameRoot.Instance.UISystem.LoadFloatingUI<NewFacilityUI>((_newfacility) =>
            {
                NewFacilityUI = _newfacility;

                ProjectUtility.SetActiveCheck(NewFacilityUI.gameObject, !FacilityData.IsOpen
                    && FacilityOpenOrder == openorder.Value);

                _newfacility.Init(NewRoot);
                _newfacility.SliderValue(FacilityData.MoneyCount, stagefacilitytd.open_cost);
            });
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 오브젝트의 레이어를 확인합니다.
        if ((collision.gameObject.layer == LayerMask.NameToLayer("Player")) && !FacilityData.IsOpen)
        {
            if (NewFacilityUI != null && NewFacilityUI.gameObject.activeSelf)
                OnEnter = true;
            MoneySpeedCount = 0;
            FacilityOpenSpeed = 0.2f;
        }
    }

    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if ((collision.gameObject.layer == LayerMask.NameToLayer("Player")) && !FacilityData.IsOpen)
        {
            OnEnter = false;
        }
    }



    public void OpenFacility()
    {
        ProjectUtility.SetActiveCheck(NewFacilityUI.gameObject, false);
        OpenAction?.Invoke();
    }



    private float moneydeltime = 0f;

    public virtual void Update()
    {
        if (FacilityData != null && !FacilityData.IsOpen && OnEnter)
        {
            if (GameRoot.Instance.UserData.CurMode.Money.Value > 0)
            {
                moneydeltime += Time.deltaTime;

                if (moneydeltime >= FacilityOpenSpeed)
                {
                    System.Numerics.BigInteger addmoneycount = 0;

                    // 가중치 추가: MoneySpeedCount 증가
                    MoneySpeedCount += (int)(MoneySpeedCount * 5f) + 1;  // 10% 증가 + 최소 1 보장

                    // FacilityOpenSpeed 감소 (최소 값 제한)
                    FacilityOpenSpeed = Mathf.Max(0.1f, FacilityOpenSpeed - 0.02f);

                    GameRoot.Instance.EffectSystem.MultiPlay<MoneyEffect>(Player.transform.position, effect =>
                    {
                        effect.SetAutoRemove(true, 1f);
                        effect.Init(MoneyRootTr, () =>
                        {
                            ProjectUtility.SetActiveCheck(effect.gameObject, false);
                        });
                    });

                    moneydeltime = 0f;

                    var remaincount = GoalCount - FacilityData.MoneyCount;

                    // 가중치 적용하여 addmoneycount 계산
                    addmoneycount = (MoneySpeedCount < remaincount) ? MoneySpeedCount : remaincount;
                    addmoneycount = (addmoneycount < GameRoot.Instance.UserData.CurMode.Money.Value) ? addmoneycount : GameRoot.Instance.UserData.CurMode.Money.Value;

                    // 재화 차감 및 시설 자금 증가
                    GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, -addmoneycount);
                    FacilityData.MoneyCount += addmoneycount;

                    // UI 업데이트
                    NewFacilityUI.SliderValue(FacilityData.MoneyCount, GoalCount);

                    // 목표 금액 도달 시 시설 개방
                    if (FacilityData.MoneyCount >= GoalCount)
                    {
                        OpenFacility();
                    }
                }
            }
        }
    }

    void OnDestroy()
    {
        disposables.Clear();
    }

    void OnDisable()
    {
        disposables.Clear();
    }

}
