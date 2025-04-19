using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;

public class FacilitySystem
{
    public enum FacilityType
    {
        FishDisplay = 1,
        Counter = 2,
        Fishing = 3,
        Cooked = 4,

    }


    public Vector2 NoneOpenSize = new Vector2(1.76f, 1.76f);

    public void Create()
    {
    }

    public void CreateStageFacility(int stageidx)
    {
        var stageinfotd = Tables.Instance.GetTable<StageFacilityInfo>().DataList.ToList().FindAll(x => x.stageidx == stageidx).ToList();

        GameRoot.Instance.UserData.CurMode.StageData.StageFacilityDataList.Clear();

        foreach (var stageinfo in stageinfotd)
        {
            var newfacility = new FacilityData(stageinfo.facilityidx, 0, false, 0);

            GameRoot.Instance.UserData.CurMode.StageData.StageFacilityDataList.Add(newfacility);
        }

        GameRoot.Instance.UserData.Save();
    }


    public ConsumerMoveInfoData CreatePattern(int stageidx)
    {
        var tdstagelist = Tables.Instance.GetTable<ConsumerMoveInfo>().DataList.ToList().FindAll(x => x.stageidx == stageidx); // facility 안열린것도 포함시키기 

        List<ConsumerMoveInfoData> patternlist = new List<ConsumerMoveInfoData>();

        for (int i = 0; i < tdstagelist.Count; i++)
        {
            // 모든 시설이 조건을 만족하면 true, 하나라도 만족하지 않으면 false
            bool allFound = true;

            for (int j = 0; j < tdstagelist[i].facilityidx.Count; j++)
            {
                var finddata = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.FindFacility(tdstagelist[i].facilityidx[j]);

                if (finddata == null)
                {
                    allFound = false;
                    break;
                }

                if (!finddata.IsOpenFacility())
                {
                    allFound = false;
                    break;
                }

                var facilityinfotd = Tables.Instance.GetTable<FacilityInfo>().GetData(tdstagelist[i].facilityidx[j]);

                if(facilityinfotd != null && facilityinfotd.cooking_group > 0)
                {
                    var findcookfacility = GameRoot.Instance.UserData.CurMode.StageData.FindFacilityData(facilityinfotd.cooking_group);

                    if(findcookfacility == null || !findcookfacility.IsOpen)
                    {
                        allFound = false;
                        break;
                    }
                }
                
            }

            // 모든 facilityidx가 조건을 만족했다면 patternlist에 추가
            if (allFound)
            {
                patternlist.Add(tdstagelist[i]);
            }
        }


        if (patternlist.Count > 0)
        {
            var randvalue = Random.Range(0, patternlist.Count);


            return patternlist[randvalue];
        }

        return null;
    }


    public StageFishUpgradeData GetFacilityUpgradeData(int facilityidx)
    {
        var finddata = GameRoot.Instance.UserData.CurMode.FishUpgradeDatas.Find(x => x.FishIdx == facilityidx);

        if (finddata == null)
        {
            finddata = new StageFishUpgradeData(facilityidx, 1);

            GameRoot.Instance.UserData.CurMode.FishUpgradeDatas.Add(finddata);
        }

        return finddata;
    }


    public int GetFishUpgradeMaxLevelCount()
    {
        int count = 0;


        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;


        var upgradelist = GameRoot.Instance.UserData.CurMode.FishUpgradeDatas.ToList();

        foreach (var upgrade in upgradelist)
        {
            var td = Tables.Instance.GetTable<FacilityUpgrade>().GetData(new KeyValuePair<int, int>(stageidx, upgrade.FishIdx));

            if (td != null && td.max_ugprade_count <= upgrade.Level)
            {
                count += 1;
            }
        }


        return count;
    }


    public System.Numerics.BigInteger GetFishUpgradeLevelCost(int fishidx, int level)
    {
        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var td = Tables.Instance.GetTable<FacilityUpgrade>().GetData(new KeyValuePair<int, int>(stageidx, fishidx));


        if (td != null)
        {
            var costincrease = td.base_income_cost * (td.income_cost_multiple * (level - 1)) / 100;

            int exponent = level / td.value_count; // 5당 2배 증가

            var levelgroupbuffvalue = exponent == 0 ? 1 : exponent  * td.income_cost_level_multiple;

            levelgroupbuffvalue = levelgroupbuffvalue == 0 ? 1 : levelgroupbuffvalue;

            var lastvalue = (td.base_income_cost + costincrease) * levelgroupbuffvalue;

            int percentage = levelgroupbuffvalue == 1 ? 1 : 100;

            return lastvalue / percentage;

        }

        return 0;
    }



    public System.Numerics.BigInteger GetFishCurSellProductValue(int fishidx, int level)
    {
        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var td = Tables.Instance.GetTable<FacilityUpgrade>().GetData(new KeyValuePair<int, int>(stageidx, fishidx));

        var basevalue = Tables.Instance.GetTable<FishInfo>().GetData(fishidx).base_revenue;

        if (td != null)
        {
            var levelbuff = basevalue * (td.income_multiple_value * (level - 1)) / 100;

            int exponent = level / td.value_count; // 5당 2배 증가

            var levelgroupbuffvalue = exponent == 0 ? 1 : exponent * td.income_multiple_value_group;

            levelgroupbuffvalue = levelgroupbuffvalue == 0 ? 1 : levelgroupbuffvalue;
    
            var lastvalue = (basevalue + levelbuff) * levelgroupbuffvalue;

            int percentage = levelgroupbuffvalue == 1 ? 1 : 100;

            var boostbuffvalue = GameRoot.Instance.BoostSystem.IsBoostOnProperty.Value ? 2 : 1;

            return (lastvalue / percentage) * boostbuffvalue;
        }

        return 0;
    }



    public System.Numerics.BigInteger GetFishLevelBuffValue(int fishidx, int level)
    {
        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var td = Tables.Instance.GetTable<FacilityUpgrade>().GetData(new KeyValuePair<int, int>(stageidx, fishidx));

        if (td != null)
        {
            return (td.income_multiple_value * (level - 1));
        }

        return 1;
    }


    public bool IsOpenPattern(List<int> facilityidxlist)
    {
        bool isopen = true;


        foreach (var facilityidx in facilityidxlist)
        {
            var facilitydata = GameRoot.Instance.UserData.CurMode.StageData.StageFacilityDataList.Find(x => x.FacilityIdx == facilityidx);
            if (facilitydata != null && facilitydata.IsOpen == false)
            {
                isopen = false;

                break;
            }
        }

        return isopen;
    }
}
