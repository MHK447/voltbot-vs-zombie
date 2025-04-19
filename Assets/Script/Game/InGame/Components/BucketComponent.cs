using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UniRx;
using System.Linq;


public class BucketComponent : MonoBehaviour
{
    [SerializeField]
    private Transform StartFishTr;

    [SerializeField]
    private Transform AmountUITr;

    [SerializeField]
    private Stack<FishComponent> FishStackComponent = new Stack<FishComponent>();

    public int GetFishCount { get { return FishStackComponent.Count; } }

    private List<OtterBase> TargetOtterList = new List<OtterBase>();

    private InGameStage InGameStage;

    private int FishCount = 0;

    private float FishCarrydeltime = 0f;

    private float FishCarryTime = 0.2f;

    private float FishPos_Y = 0.15f;

    private FacilityData FacilityData = null;

    private TextCount_UI CountUI = null;

    private CompositeDisposable disposables = new CompositeDisposable();

    private int CapacityMaxCount = 0;

    private int FishIdx = 0;

    public void Init(FacilityData facility)
    {
        FishCount = 0;
        InGameStage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;

        FacilityData = facility;

        var td = Tables.Instance.GetTable<FacilityInfo>().GetData(FacilityData.FacilityIdx);

        if (td != null)
        {
            FishIdx = td.value_1;
            CapacityMaxCount = td.start_capacity;
        }

        ProjectUtility.SetActiveCheck(this.gameObject, FacilityData.IsOpen);

        GameRoot.Instance.UISystem.LoadFloatingUI<TextCount_UI>((_progress) =>
        {
            CountUI = _progress;
            ProjectUtility.SetActiveCheck(CountUI.gameObject, FacilityData.IsOpen);
            CountUI.Init(AmountUITr);
            CountUI.SetText(FacilityData.CapacityCountProperty.Value, CapacityMaxCount);
        });

        disposables.Clear();


        FacilityData.CapacityCountProperty.Subscribe(x =>
        {
            if (CountUI != null)
            {
                CountUI.SetText(x, CapacityMaxCount);
            }
        }).AddTo(disposables);

        GameRoot.Instance.StartCoroutine(WaitOneFrame());

    }


    public IEnumerator WaitOneFrame()
    {
        yield return new WaitForSeconds(1f);


        for (int i = 0; i < FacilityData.CapacityCountProperty.Value; ++i)
        {
            var posy = FishPos_Y * (i + 1);

            InGameStage.CreateFish(this.transform, FishIdx, FishComponent.State.Bucket, (fish) =>
            {
                FishStackComponent.Push(fish);
                fish.LivingFishAnim(true);
                if (CountUI != null)
                {
                    CountUI.Init(fish.transform);
                }
                fish.FishInBucketAction(this.transform, (fish) =>
                {

                }, 0f, posy);
            });
        }


        yield return new WaitForSeconds(1f);

        if (FishStackComponent.Count > 0)
            CountUICheck(FishStackComponent.First().transform.position);

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 오브젝트의 레이어를 확인합니다.
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("CarryCasher"))
        {
            FishCarrydeltime = 0f;
            var getvalue = other.GetComponent<OtterBase>();

            if (getvalue != null && getvalue.IsMaxFishCheck() == false)
            {
                if (!TargetOtterList.Contains(getvalue))
                {
                    TargetOtterList.Add(getvalue);
                }
            }
        }
    }




    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("CarryCasher"))
        {
            var getvalue = collision.gameObject.GetComponent<OtterBase>();

            if (getvalue != null)
            {
                if (TargetOtterList.Contains(getvalue))
                {
                    TargetOtterList.Remove(getvalue);
                }
            }
        }
    }


    private void Update()
    {
        if (FishStackComponent.Count <= 0) return;

        for (int i = 0; i < TargetOtterList.Count; ++i)
        {
            if (TargetOtterList.Count > 0 && !TargetOtterList[i].IsFishing)
            {
                FishCarrydeltime += Time.deltaTime;

                if (FishCarrydeltime >= FishCarryTime && !TargetOtterList[i].IsMaxFishCheck())
                {
                    FishCarrydeltime = 0f;

                    var fishcomponent = FishStackComponent.Pop();

                    if (FishStackComponent.Count > 0)
                        CountUI.Init(FishStackComponent.First().transform);

                    TargetOtterList[i].AddFish(fishcomponent);
                    
                    GameRoot.Instance.NaviSystem.NextNavi(NaviSystem.NaviType.RackFishAdd);

                    FacilityData.CapacityCountProperty.Value -= 1;

                    break;
                }
            }
        }
    }

    public void AddFishQueue(FishComponent fish)
    {
        FishStackComponent.Push(fish);
        FacilityData.CapacityCountProperty.Value += 1;

        if (GameRoot.Instance.NaviSystem.IsNaviOn)
        {
            GameRoot.Instance.NaviSystem.NextNavi(NaviSystem.NaviType.GoToBucket);
        }

    }

    public void CountUICheck(Vector3 pos)
    {
        CountUI.SetUpdatePos(pos);
    }


    private void OnDisable()
    {
        if (CountUI != null)
        {
            Destroy(CountUI.gameObject);
            CountUI = null;
        }
    }
}
