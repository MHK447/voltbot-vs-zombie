using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CounterComponent : FacilityComponent
{

    private List<Consumer> CounterConsumerList = new List<Consumer>();

    private float CheckOutConsumerTime = 2f;

    private float checkoutdeltime = 0f;

    private CounterCasher CasherCounter;

    private bool IsPlayer = false;

    public override void Init()
    {
        base.Init();
        CounterConsumerList.Clear();

        CasherCounter = InGameStage.FindCasher(CasherType.CounterCasher, FacilityData.FacilityIdx) as CounterCasher;

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (InGameStage.FindCasher(CasherType.CounterCasher, FacilityData.FacilityIdx) != null)
        {
            IsPlayer = true;
            return;
        }

        // 충돌한 오브젝트의 레이어를 확인합니다.

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            checkoutdeltime = 0f;

            IsPlayer = true;

            if(GameRoot.Instance.NaviSystem.CurNaviOnType != NaviSystem.NaviType.Counter)
            GameRoot.Instance.NaviSystem.NextNavi(NaviSystem.NaviType.WaitCalc);
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (InGameStage.FindCasher(CasherType.CounterCasher, FacilityData.FacilityIdx) != null)
        {
            IsPlayer = true;
            return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (Player != null)
            {
                Player.CoolTimeActive(0f);
            }

            checkoutdeltime = 0f;
            IsPlayer = false;
        }
    }

    public void Update()
    {
        if (InGameStage == null) return;

        if (CasherCounter == null)
        {
            CasherCounter = InGameStage.FindCasher(CasherType.CounterCasher, FacilityData.FacilityIdx) as CounterCasher;

            if (CasherCounter != null)
                IsPlayer = true;
        }

        if ((IsPlayer && CounterConsumerList.Count > 0))
        {


            var findconsumer = CounterConsumerList.Find(x => x.CurCounterOrder == 0 && x.IsArrivedCounter);

            if (findconsumer != null)
            {
                checkoutdeltime += Time.deltaTime;

                var valuetime = checkoutdeltime / CheckOutConsumerTime;
                if (CasherCounter != null)
                {
                    CasherCounter.CalcFish(true);
                }

                if (CasherCounter != null)
                {
                    CasherCounter.CoolTimeActive(valuetime);
                }
                else
                    Player.CoolTimeActive(valuetime);

                if (checkoutdeltime >= CheckOutConsumerTime)
                {
                    checkoutdeltime = 0f;

                    if (CasherCounter != null)
                        CasherCounter.CoolTimeActive(0f);
                    else
                        Player.CoolTimeActive(0f);

                    GameRoot.Instance.EffectSystem.MultiPlay<TextEffectMoney>(findconsumer.transform.position, (effect) =>
                    {
                        System.Numerics.BigInteger reward = findconsumer.CheckRevenue();

                        effect.SetAutoRemove(true, 1.5f);
                        effect.SetText(reward);
                        GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, reward);
                    });

                    SoundPlayer.Instance.PlaySound("counter");
                    GameRoot.Instance.NaviSystem.NextNavi(NaviSystem.NaviType.UpgradeStart);


                    if (findconsumer != null)
                    {
                        findconsumer.OutCounterConsumer();
                        CounterConsumerList.Remove(findconsumer);

                        if (CasherCounter != null && CounterConsumerList.Count == 0)
                        {
                            CasherCounter.CalcFish(false);
                        }

                    }
                }
            }
        }
    }

    public Consumer FindOrderConsumer(int order)
    {
        var finddata = CounterConsumerList.Find(x => x.CurCounterOrder == order);

        if (finddata != null)
        {
            return finddata;
        }

        return null;
    }



    public Transform GetEmptyConsumerTr()
    {
        return ConsumerWaitTr[CounterConsumerList.Count];
    }

    public void AddConsumer(Consumer consumer)
    {
        consumer.CurCounterOrder = CounterConsumerList.Count;
        CounterConsumerList.Add(consumer);
    }
}
