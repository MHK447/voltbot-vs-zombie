using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UniRx;

public class FishCushionComponent : MonoBehaviour
{
    [SerializeField]
    private BucketComponent BucketComponent;

    [SerializeField]
    private Transform FishCasherTr;

    public Transform GetFishCasherTr { get { return FishCasherTr; } }

    public float FisgingTime = 2f;

    private float FishPos_Y = 0.15f;

    private List<OtterBase> TargetOtterList = new List<OtterBase>();

    private InGameStage InGameStage;

    private FacilityData FacilityData = null;

    private int CapacityMaxCount = 0;

    private int FishIdx = 0;


    private CompositeDisposable disposables = new CompositeDisposable();

    public void Init(FacilityData facility)
    {
        FacilityData = facility;

        InGameStage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;

        var td = Tables.Instance.GetTable<FacilityInfo>().GetData(FacilityData.FacilityIdx);

        if (td != null)
        {
            FishIdx = td.value_1;
            CapacityMaxCount = td.start_capacity;
        }

        ProjectUtility.SetActiveCheck(this.gameObject, FacilityData.IsOpen);

        SetFishingTime();

        disposables.Clear();

        GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.ObserveAdd().Subscribe(x =>
        {
            if (x.Value.UpgradeType == (int)UpgradeSystem.UpgradeType.FishCasherSpeedUp)
            {
                SetFishingTime();
            }
        }).AddTo(disposables);

        foreach (var stageupgrade in GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList)
        {
            if (stageupgrade.UpgradeType == (int)UpgradeSystem.UpgradeType.FishCasherSpeedUp)
                stageupgrade.IsBuyCheckProperty.Subscribe(x => { SetFishingTime(); }).AddTo(disposables);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        var getvalue = other.GetComponent<OtterBase>();

        if (getvalue != null && (getvalue.GetCurUnitType == OtterBase.OtterType.Player || getvalue.GetCurUnitType == OtterBase.OtterType.CarryCasher))
        {
            if (!TargetOtterList.Contains(getvalue))
            {
                getvalue.CoolTimeActive(0f);
                TargetOtterList.Add(getvalue);
            }
        }


    }




    private void OnTriggerExit2D(Collider2D collision)
    {
        if (InGameStage == null) return;


        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("CarryCasher"))
        {
            var getvalue = collision.gameObject.GetComponent<OtterBase>();

            if (getvalue != null)
            {
                if (TargetOtterList.Contains(getvalue))
                {
                    getvalue.CoolTimeActive(0f);
                    TargetOtterList.Remove(getvalue);
                }
            }
        }
    }

    public void SetFishingTime()
    {
        var buffvalue = GameRoot.Instance.UpgradeSystem.GetUpgradeValue(UpgradeSystem.UpgradeType.FishCasherSpeedUp, FacilityData.FacilityIdx);

        var percentvalue = ProjectUtility.PercentCalc(GameRoot.Instance.InGameSystem.default_fishing_time, buffvalue);

        FisgingTime = GameRoot.Instance.InGameSystem.default_fishing_time - percentvalue;
    }


    private void Update()
    {
        if (FacilityData == null) return;

        if (FacilityData.IsOpen == false) return;

        if (FacilityData.CapacityCountProperty.Value >= CapacityMaxCount)
        {
            for (int i = TargetOtterList.Count - 1; i >= 0; i--)
            {
                TargetOtterList[i].CoolTimeActive(0f);
            }
            return;
        }
        for (int i = TargetOtterList.Count - 1; i >= 0; i--)
        {
            if (TargetOtterList[i].IsIdle && !TargetOtterList[i].IsFishing && !TargetOtterList[i].IsCarry)
            {
                TargetOtterList[i].PlayAnimation(OtterBase.OtterState.Fishing, "fishingidle", true);
            }

            if (TargetOtterList[i].IsMove)
            {
                TargetOtterList[i].CurMoneyTime = 0f;
            }

            if (TargetOtterList[i].IsFishing && FacilityData.CapacityCountProperty.Value < CapacityMaxCount)
            {
                TargetOtterList[i].CurMoneyTime += Time.deltaTime;

                var cooltimevalue = (float)TargetOtterList[i].CurMoneyTime / (float)FisgingTime;

                TargetOtterList[i].CoolTimeActive(cooltimevalue);

                if (TargetOtterList[i].CurMoneyTime >= FisgingTime)
                {
                    TargetOtterList[i].CurMoneyTime = 0f;
                    Debug.Log($"[DEBUG] TargetOtterList[{i}] = {TargetOtterList[i]}");
                    Debug.Log($"[DEBUG] TargetOtterList[{i}].GetFishTr = {TargetOtterList[i]?.GetFishTr}");
                    Debug.Log($"[DEBUG] FishIdx = {FishIdx}");
                    
                    var targetotter = TargetOtterList[i];

                    InGameStage.CreateFish(TargetOtterList[i].GetFishTr, FishIdx, FishComponent.State.Bucket, (fish) =>
                    {
                        Debug.Log($"[DEBUG] Created fish: {fish}");
                        StartFishAction(fish, targetotter);
                    });
                    SoundPlayer.Instance.PlaySound("fishing");
                }
            }
        }
    }



    public void StartFishAction(FishComponent fish, OtterBase otter)
    {
        var fishcount = BucketComponent.GetFishCount;
        var posy = FishPos_Y * fishcount;
        fish.LivingFishAnim(true);

        BucketComponent.AddFishQueue(fish);

        fish.FishInBucketAction(BucketComponent.transform, (fish) =>
        {
            BucketComponent.CountUICheck(fish.transform.position);
            fish.transform.position = BucketComponent.transform.position;
            //otter.CoolTimeActive(FacilityData.CapacityCountProperty.Value < CapacityMaxCount);
        }, 1f, posy);
    }


    void OnDisable()
    {
        disposables.Clear();
    }

    void OnDestroy()
    {
        disposables.Clear();
    }
}
