using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BanpoFri;


public class UpgradeSystem
{
    public enum UpgradeType
    {
        AddTransportStaff = 1,        // 운반 직원 추가
        TransportStaffSpeedUp = 2,    // 운반 직원 속도 업
        AddCustomer = 3,              // 손님 추가
        PlayerSpeedUp = 4,            // 플레이어 속도 업
        PlayerCapacityUp = 5,         // 플레이어 용량 추가
        ShelfCapacityUp = 6,          // 매대 용량 추가
        CookingSpeedUp = 7,           // 음식 조리 속도 업
        TransportStaffCapacityUp = 8, // 운반 직원 용량 추가
        AddFishingStaff = 9,          // 낚시 직원 추가
        AddCounterCashierStaff = 10,          // 계산대 직원 추가
        FishCasherSpeedUp = 11, //낚시 직원 속도 증가 
    }


    public void StartUpgradeCheck()
    {
        foreach(var upgradedata in GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList)
        {
            if (upgradedata.IsBuyCheckProperty.Value)
            {
                if ((int)UpgradeType.AddCustomer == upgradedata.UpgradeType) continue;

                AddUpgradeData(upgradedata.UpgradeIdx, upgradedata.UpgradeType);
            } 
        }
    }


    public float GetUpgradeValue(UpgradeType type , int value2 = -1)
    {
        float returnvalue = 0f;

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var upgradelist = GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.ToList().FindAll(x => x.UpgradeType == (int)type && x.IsBuyCheckProperty.Value);

        if (type == UpgradeType.AddCustomer)
        {

            var td = Tables.Instance.GetTable<StageInfo>().GetData(stageidx);

            if(td != null)
            {
                foreach(var upgrade in upgradelist)
                {
                    var upgradetd = Tables.Instance.GetTable<UpgradeInfo>().GetData(new KeyValuePair<int, int>(stageidx, upgrade.UpgradeIdx));

                    if(upgradetd != null)
                    {
                        returnvalue += upgradetd.value;
                    }

                }

                returnvalue += td.start_consumer_count;
            }
            
        }
        else
        {
            foreach (var upgrade in upgradelist)
            {
                var upgradetd = Tables.Instance.GetTable<UpgradeInfo>().GetData(new KeyValuePair<int, int>(stageidx, upgrade.UpgradeIdx));

                if(value2 > 0)
                {
                    if (upgradetd != null && value2 == upgradetd.value2)
                    {
                        returnvalue += upgradetd.value;
                    }
                }
                else
                {
                    returnvalue += upgradetd.value;
                }
            }
        }

        return returnvalue;

    }


    public void AddUpgradeData(int upgradeidx  , int upgradetype)
    {
        switch(upgradetype)
        {
            case (int)UpgradeType.AddTransportStaff:
                {
                    var ingamestage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>();
                    var finddata = ingamestage.curInGameStage.ActiveCarryCasher(CasherType.CarryCasher);

                    if(finddata != null)
                    {
                        var findcarrycasher = finddata.GetComponent<CarryCasher>();

                        if (findcarrycasher != null)
                        {
                            findcarrycasher.transform.position = ingamestage.GetPlayer.transform.position;
                            ProjectUtility.SetActiveCheck(findcarrycasher.gameObject, true);
                            findcarrycasher.Init();
                        }
                    }
                }
                break;
            case (int)UpgradeType.TransportStaffSpeedUp:
                {


                }
                break;
            case (int)UpgradeType.AddCustomer:
                {
                    var ingamestage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>();
                    ingamestage.curInGameStage.CreateConsumer(1, ingamestage.curInGameStage.GetStartWayPoint);
                }
                break;
            case (int)UpgradeType.PlayerSpeedUp:
                {

                }
                break;
            case (int)UpgradeType.PlayerCapacityUp:
                break;
            case (int)UpgradeType.ShelfCapacityUp:
                break;
            case (int)UpgradeType.CookingSpeedUp:
                break;
            case (int)UpgradeType.TransportStaffCapacityUp:
                break;
            case (int)UpgradeType.AddFishingStaff:
                {

                    var ingamestage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>();
                    var finddata = ingamestage.curInGameStage.ActiveCarryCasher(CasherType.FishingCasher);

                    if (finddata != null)
                    {
                        var fishingdata = finddata.GetComponent<FishCasher>();

                        if(fishingdata != null)
                        {
                            var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

                            var td = Tables.Instance.GetTable<UpgradeInfo>().GetData(new KeyValuePair<int, int>(stageidx, upgradeidx));

                            if(td != null)
                            {
                                finddata.transform.position = ingamestage.GetPlayer.transform.position;
                                ProjectUtility.SetActiveCheck(finddata.gameObject, true);
                                fishingdata.Init();
                                fishingdata.Set(td.value2);
                            }
                        }
                    }
                }
                break;
            case (int)UpgradeType.AddCounterCashierStaff:
                {

                    var ingamestage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>();
                    var finddata = ingamestage.curInGameStage.ActiveCarryCasher(CasherType.CounterCasher);

                    var counterdata = finddata.GetComponent<CounterCasher>();

                    if (counterdata != null)
                    {
                        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

                        var td = Tables.Instance.GetTable<UpgradeInfo>().GetData(new KeyValuePair<int, int>(stageidx, upgradeidx));

                        if (td != null)
                        {
                            finddata.transform.position = ingamestage.GetPlayer.transform.position;
                            ProjectUtility.SetActiveCheck(finddata.gameObject, true);
                            counterdata.Init();
                        }
                    }
                }
                break;
        }
    }

    public void StageSetUpgradeData(int stageidx)
    {

        var finddata = GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.ToList().Find(x => x.StageIdx == stageidx);

        if(finddata == null)
        {
            GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.Clear();


            var upgradelist = Tables.Instance.GetTable<UpgradeInfo>().DataList.ToList().FindAll(x => x.stageidx == stageidx);

            foreach(var upgrade in upgradelist)
            {
                var newdata = new UpgradeData(upgrade.upgrade_idx, upgrade.upgrade_type, stageidx, false);

                GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.Add(newdata);
            }

        }

        GameRoot.Instance.UserData.Save();
    }
}
