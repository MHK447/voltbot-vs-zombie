using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;

public class CookTableComponent : MonoBehaviour
{
    [SerializeField]
    private Transform CasherTr;

    public Transform GetCasherTr { get { return CasherTr; } }

    private Queue<FishComponent> FoodComponetQueue = new Queue<FishComponent>();

    public Queue<FishComponent> FoodCompleteGetCount { get { return FoodComponetQueue; } }

    private List<OtterBase> TargetOtterList = new List<OtterBase>();

    private FacilityData FacilityData;
    
    public void Init(FacilityData facilitydata)
    {
        FacilityData = facilitydata;

        FoodComponetQueue.Clear();
        TargetOtterList.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var getvalue = collision.GetComponent<OtterBase>();

            if (getvalue != null && getvalue.IsMaxFishCheck() == false)
            {
                if (!TargetOtterList.Contains(getvalue))
                {
                    TargetOtterList.Add(getvalue);
                }
            }
        }

        if(collision.gameObject.layer == LayerMask.NameToLayer("CarryCasher"))
        {
            var getvalue = collision.GetComponent<OtterBase>();

            if (getvalue != null && getvalue.GetFishComponentList.Count == 0)
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

    private float FishCarrydeltime = 0f;

    private float FishCarryTime = 0.2f;

    private void Update()
    {
        if (FoodComponetQueue.Count <= 0) return;

        for (int i = 0; i < TargetOtterList.Count; ++i)
        {
            if (TargetOtterList.Count > 0 && !TargetOtterList[i].IsFishing)
            {
                if(TargetOtterList[i].gameObject.layer == LayerMask.NameToLayer("CarryCasher"))
                {
                    if(TargetOtterList[i].IsMove)
                    {
                        continue;
                    }
                }


                FishCarrydeltime += Time.deltaTime;

                if (FishCarrydeltime >= FishCarryTime && !TargetOtterList[i].IsMaxFishCheck())
                {
                    FishCarrydeltime = 0f;

                    var fishcomponent = FoodComponetQueue.Dequeue();

                    //if (FoodComponetQueue.Count > 0)
                    //    CountUI.Init(FishStackComponent.First().transform);
                    //else
                    //    CountUI.Init(AmountUITr);

                    TargetOtterList[i].AddFish(fishcomponent);

                    FacilityData.CapacityCountProperty.Value -= 1;

                    break;
                }
            }
        }
    }
}
