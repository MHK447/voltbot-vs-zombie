using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using BanpoFri;
using BanpoFri.Data;
public enum DataState
{
    None,
    Main,
    Event
}
public partial class UserDataSystem
{
    public bool Bgm = true;
    public bool Effect = true;
    public bool SlowGraphic = false;
    public Config.Language Language = Config.Language.ko;
    public IReactiveProperty<int> Cash { get; private set; } = new ReactiveProperty<int>(0);
    public IReactiveCollection<string> BuyInappIds { get; private set; } = new ReactiveCollection<string>();
    public IReactiveCollection<string> Tutorial { get; private set; } = new ReactiveCollection<string>();
    public IReactiveCollection<string> GameNotifications { get; private set; } = new ReactiveCollection<string>();
    public Dictionary<string, int> RecordCount { get; private set; } = new Dictionary<string, int>();
    public IUserDataMode CurMode { get; private set; }
    private UserDataMain mainData = new UserDataMain();
    private UserDataEvent eventData = new UserDataEvent();
    public DataState DataState { get; private set; } = DataState.None;
    public bool IsMainState { get { return DataState == DataState.Main; } }
    public IReactiveCollection<int> OneLink { get; private set; } = new ReactiveCollection<int>();

    public IReactiveProperty<BigInteger> HUDMoney = new ReactiveProperty<BigInteger>(0);
    public IReactiveProperty<BigInteger> HudEnergyMoney = new ReactiveProperty<BigInteger>(0);
    public IReactiveProperty<int> HUDCash = new ReactiveProperty<int>(0);


    void ConnectReadOnlyDatas()
    {

        RecordCount.Clear();
        ChangeDataMode(DataState.Main);

        mainData.StageData.StageIdx = flatBufferUserData.Stagedata.Value.Stageidx;
        Cash.Value = flatBufferUserData.Cash;
        mainData.Money.Value = BigInteger.Parse(flatBufferUserData.Money);
        mainData.LastLoginTime = new System.DateTime(flatBufferUserData.Lastlogintime);
        mainData.CurPlayDateTime = new System.DateTime(flatBufferUserData.Curplaydatetime);



        for (int i = 0; i < flatBufferUserData.RecordcountLength; ++i)
        {
            var data = flatBufferUserData.Recordcount(i);
            RecordCount.Add(data.Value.Idx, data.Value.Count);
        }


        Tutorial.Clear();
        if (!string.IsNullOrEmpty(flatBufferUserData.Tutorial))
        {
            var splitArr = flatBufferUserData.Tutorial.Split(';');
            foreach (var split in splitArr)
            {
                Tutorial.Add(split);
            }
        }
        
        Language = (Config.Language)System.Enum.Parse(typeof(Config.Language), flatBufferUserData.Optiondata.Value.Language);
        Bgm = flatBufferUserData.Optiondata.Value.Bgm;
        Effect = flatBufferUserData.Optiondata.Value.Effect;
        SlowGraphic = flatBufferUserData.Optiondata.Value.Slowgraphic;
        Vib = flatBufferUserData.Optiondata.Value.Vibration;
        AutoFelling = flatBufferUserData.Optiondata.Value.Autofelling;
        SubscribeOrder = flatBufferUserData.Optiondata.Value.Subscribeorder;


        if (flatBufferUserData.Stagedata != null)
        {
            mainData.StageData.StageFacilityDataList.Clear();

            mainData.StageData.StageIdx = flatBufferUserData.Stagedata.Value.Stageidx;

            mainData.StageData.NextFacilityOpenOrderProperty.Value = flatBufferUserData.Stagedata.Value.Facilityopenorder;

            for (int i = 0; i < flatBufferUserData.Stagedata.Value.FacilitydatasLength; ++i)
            {
                var data = flatBufferUserData.Stagedata.Value.Facilitydatas(i);

                var moneyvalue = System.Numerics.BigInteger.Parse(data.Value.Moneycount);

                var newdata = new FacilityData(data.Value.Facilityidx, moneyvalue, data.Value.Isopen, data.Value.Capacitycount);

                mainData.StageData.StageFacilityDataList.Add(newdata);
            }
        }



        mainData.BoostTime.Value = flatBufferUserData.Boosttime;


        mainData.UpgradeGroupData.StageUpgradeCollectionList.Clear();

        for (int i = 0; i < flatBufferUserData.UpgradedatasLength; ++i)
        {
            var data = flatBufferUserData.Upgradedatas(i);

            var newdata = new UpgradeData(data.Value.Upgradeidx, data.Value.Upgradetype, data.Value.Stageidx, data.Value.Isbuycheck);

            mainData.UpgradeGroupData.StageUpgradeCollectionList.Add(newdata);
        }


        mainData.FishUpgradeDatas.Clear();

        for (int i = 0; i < flatBufferUserData.FacilityupgradedatasLength; ++i)
        {
            var data = flatBufferUserData.Facilityupgradedatas(i);

            var newdata = new StageFishUpgradeData(data.Value.Faciltiyidx, data.Value.Level);

            mainData.FishUpgradeDatas.Add(newdata);
        }

        SyncHUDCurrency();
    }


    public UserDataEvent CurEventData { get { return eventData; } }


    public UserDataMain CurMainData { get { return mainData; } }

    private void SnycCollectionToDB<T, U>(IList<T> db, IEnumerable<U> collector) where T : class
    {
        db.Clear();
        foreach (var iter in collector)
        {
            db.Add(iter as T);
        }
    }

    private void SnycCollectionToClient<T, U>(IList<T> db, IEnumerable<U> collector)
    where T : class, IReadOnlyData
    where U : class, IReadOnlyData
    {
        db.Clear();
        foreach (var iter in collector)
        {
            db.Add(iter.Clone() as T);
        }
    }

    public void SyncHUDCurrency(int currencyID = -1)
    {
        if (currencyID < 0)
        {
            HUDMoney.Value = CurMode.Money.Value;
            HudEnergyMoney.Value = CurMode.EnergyMoney.Value;
            HUDCash.Value = Cash.Value;
        }
        else if (currencyID == (int)Config.CurrencyID.Cash)
        {
            HUDCash.Value = Cash.Value;
        }
        else if (currencyID == (int)Config.CurrencyID.EnergyMoney)
        {
            HudEnergyMoney.Value = CurMode.EnergyMoney.Value;
        }
        else if (currencyID == (int)Config.CurrencyID.Money)
        {
            HUDMoney.Value = CurMode.Money.Value;
        }
    }

    public void SetHUDUIReward(int rewardType, int rewardIdx, BigInteger rewardCnt)
    {
        if (rewardType != (int)Config.RewardType.Currency) return;
        switch (rewardIdx)
        {
            case (int)Config.CurrencyID.EnergyMoney:
                {
                    HudEnergyMoney.Value += (int)rewardCnt;
                }
                break;
            case (int)Config.CurrencyID.Money:
                {
                    HUDMoney.Value += rewardCnt;
                }
                break;
            case (int)Config.CurrencyID.Cash:
                {
                    HUDCash.Value += (int)rewardCnt;
                }
                break;
        }
    }

    public void ResetRecordCount(Config.RecordCountKeys idx, params object[] objs)
    {
        var strKey = ProjectUtility.GetRecordCountText(idx, objs);
        if (RecordCount.ContainsKey(strKey))
            RecordCount[strKey] = 0;
    }


    public void ResetRecordCount(Config.RecordCountKeys idx, int resetvalue = 0, params object[] objs)
    {
        var strKey = ProjectUtility.GetRecordCountText(idx, objs);
        if (RecordCount.ContainsKey(strKey))
            RecordCount[strKey] = resetvalue;
    }



    public void AddRecordCount(Config.RecordCountKeys idx, int count, params object[] objs)
    {
        var strKey = ProjectUtility.GetRecordCountText(idx, objs);
        if (RecordCount.ContainsKey(strKey))
            RecordCount[strKey] += count;
        else
            RecordCount.Add(strKey, count);
    }

    public int GetRecordCount(Config.RecordCountKeys idx, params object[] objs)
    {
        var strKey = ProjectUtility.GetRecordCountText(idx, objs);

        if (RecordCount.ContainsKey(strKey))
        {
            return RecordCount[strKey];
        }
        else
        {
            return 0;
        }
    }

    public void SetReward(int rewardType, int rewardIdx, BigInteger rewardCnt, bool hudRefresh = true)
    {
        switch (rewardType)
        {
            case (int)Config.RewardType.Currency:
                {
                    switch (rewardIdx)
                    {
                        case (int)Config.CurrencyID.Money:
                            {
                                CurMode.Money.Value += rewardCnt;
                            }
                            break;
                        case (int)Config.CurrencyID.Cash:
                            {
                                Cash.Value += (int)rewardCnt;
                            }
                            break;
                        case (int)Config.CurrencyID.EnergyMoney:
                            {
                                CurMode.EnergyMoney.Value += (int)rewardCnt;
                            }
                            break;
                        case (int)Config.CurrencyID.GachaCoin:
                            {
                                CurMode.GachaCoin.Value += (int)rewardCnt;
                            }
                            break;


                    }
                    if (hudRefresh)
                    {
                        SetHUDUIReward(rewardType, rewardIdx, rewardCnt);
                    }
                }
                break;
        }
    }

    public void RefreshUICurrency()
    {

    }


    private void TutoDataCheck()
    {

    }
}