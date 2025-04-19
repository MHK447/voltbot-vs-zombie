using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using BanpoFri;
using Unity.VisualScripting;
public class GameNotificationSystem
{
    public enum NotificationType
    {
        Once,
        Passive
    }

    public enum NotificationCategory
    {
        UpgradePopup,
        StageClear,
        UpgradeProduct,
    }

    public class NotificationData
    {
        public NotificationType type;
        public NotificationCategory category;
        public int targetIdx = -1;
        public int targetSubIdx = -1;
        public IReactiveProperty<bool> on = new ReactiveProperty<bool>(false);
        public IReactiveProperty<int> onCount = new ReactiveProperty<int>(0);
    }

    private ReactiveDictionary<NotificationCategory, List<NotificationData>> notifications = new ReactiveDictionary<NotificationCategory, List<NotificationData>>();
    private CompositeDisposable disposables = new CompositeDisposable();

    public ReactiveDictionary<NotificationCategory, List<NotificationData>> GetNotifications { get { return notifications; } }


    private bool IsPassive(NotificationCategory category)
    {
        return false;
    }

    public NotificationData GetData(NotificationCategory _key, int _targetIdx, int _targetSubIdx)
    {
        if (notifications.ContainsKey(_key))
        {
            if (notifications[_key].Count > 0)
            {
                return notifications[_key].Find(x => x.targetIdx == _targetIdx && x.targetSubIdx == _targetSubIdx);
            }
        }

        return null;
    }


    private int BuyOneCardPrice = 0;
    private int BuyTenCardPrice = 0;

    public void Create()
    {
        notifications.Clear();
        disposables.Clear();

        foreach (NotificationCategory e in System.Enum.GetValues(typeof(NotificationCategory)))
        {
            var managerNotiList = new List<NotificationData>();
            var managerNoti = new NotificationData()
            {
                type = IsPassive(e) ? NotificationType.Passive : NotificationType.Once,
                category = e,
                targetIdx = -1,
                targetSubIdx = -1
            };
            managerNotiList.Add(managerNoti);
            notifications.Add(e, managerNotiList);
        }

        GameRoot.Instance.UserData.CurMode.Money.Subscribe(x =>
        {
            UpdateNotification(NotificationCategory.UpgradePopup);
            UpdateNotification(NotificationCategory.StageClear);
            UpdateNotification(NotificationCategory.UpgradeProduct);
        }).AddTo(disposables);

        var upgradelist = GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.ToList().FindAll(x => x.IsBuyCheckProperty.Value == false);

        foreach (var upgrade in upgradelist)
        {
            upgrade.IsBuyCheckProperty.SkipLatestValueOnSubscribe().Subscribe(x => { UpdateNotification(NotificationCategory.StageClear); }).AddTo(disposables);
        }


        GameRoot.Instance.WaitTimeAndCallback(1f, () =>
        {
            UpdateNotification(NotificationCategory.UpgradePopup);
            UpdateNotification(NotificationCategory.StageClear);
            UpdateNotification(NotificationCategory.UpgradeProduct);
        });

    }

    public void AddNoti(NoticeComponent.NoticeType noti, Transform target)
    {
        var newdata = new NoticeData((int)noti, target);

        GameRoot.Instance.UserData.CurMode.NoticeCollections.Add(newdata);
    }

    public void RemoveNoti(NoticeComponent.NoticeType noti, Transform target = null)
    {
        var finddata = target == null ? 
         GameRoot.Instance.UserData.CurMode.NoticeCollections.ToList().Find(x => x.NotiIdx == (int)noti) 
         : GameRoot.Instance.UserData.CurMode.NoticeCollections.ToList().Find(x => x.NotiIdx == (int)noti && x.Target == target);

        if (finddata != null)
            GameRoot.Instance.UserData.CurMode.NoticeCollections.Remove(finddata);
    }
    
    public void UpdateNotification(NotificationCategory category, int subIdx = -1)
    {
        switch (category)
        {
            case NotificationCategory.UpgradePopup:
                {
                    var noti = GetData(category, -1, -1);
                    if (noti == null) return;

                    bool on = false;

                    var upgradelist = GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.ToList();


                    foreach (var upgrade in upgradelist)
                    {
                        if (upgrade.IsBuyCheckProperty.Value) continue;

                        var td = Tables.Instance.GetTable<UpgradeInfo>().GetData(new KeyValuePair<int, int>(upgrade.StageIdx, upgrade.UpgradeIdx));

                        if (td != null)
                        {
                            if (GameRoot.Instance.UserData.CurMode.Money.Value >= td.cost && !upgrade.IsBuyCheckProperty.Value)
                            {
                                on = true;
                                break;
                            }
                        }
                    }


                    if (on != noti.on.Value)
                        noti.on.Value = on;
                }
                break;

            case NotificationCategory.StageClear:
                {
                    var noti = GetData(category, -1, -1);
                    if (noti == null) return;

                    bool on = false;

                    var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

                    var isnonebuyupgrade = GameRoot.Instance.UserData.CurMode.UpgradeGroupData.StageUpgradeCollectionList.ToList().Find(x => x.IsBuyCheckProperty.Value == false);

                    var curmaxcount = GameRoot.Instance.FacilitySystem.GetFishUpgradeMaxLevelCount();

                    var tdlist = Tables.Instance.GetTable<FacilityUpgrade>().DataList.ToList().FindAll(x => x.stageidx == stageidx);

                    if (isnonebuyupgrade == null && tdlist.Count == curmaxcount)
                    {

                        var td = Tables.Instance.GetTable<StageInfo>().GetData(stageidx);

                        if (td != null)
                        {
                            on = GameRoot.Instance.UserData.CurMode.Money.Value >= td.next_stage_money;
                        }
                    }

                    noti.on.Value = on;
                }
                break;
            case NotificationCategory.UpgradeProduct:
                {
                    var noti = GetData(category, -1, -1);
                    if (noti == null) return;

                    bool on = false;

                    var fishupgradedatas = GameRoot.Instance.UserData.CurMode.FishUpgradeDatas.ToList();


                    var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

                    foreach (var fishupgrade in fishupgradedatas)
                    {
                        var td = Tables.Instance.GetTable<FacilityUpgrade>().GetData(new KeyValuePair<int, int>(stageidx, fishupgrade.FishIdx));

                        if (td == null) continue;

                        if (td != null && td.max_ugprade_count <= fishupgrade.Level) continue;

                        var getfindfacility = GameRoot.Instance.UserData.CurMode.StageData.FindFishFacilityData(td.facilityidx);

                        if (getfindfacility == null || !getfindfacility.IsOpen) continue;

                        var curprice = GameRoot.Instance.FacilitySystem.GetFishUpgradeLevelCost(fishupgrade.FishIdx, fishupgrade.Level);

                        if (GameRoot.Instance.UserData.CurMode.Money.Value >= curprice)
                        {
                            on = true;
                            break;
                        }

                    }
                    noti.on.Value = on;
                }
                break;
        }
    }

    public void ChangeOnceNotification(NotificationCategory category, bool value, int targetIdx = -1)
    {
        if (notifications.ContainsKey(category))
        {
            var find = notifications[category].Find(x => x.targetIdx == targetIdx);
            if (find != null)
            {
                find.on.Value = value;
            }
        }
    }

    public void UpdateOneSeconds()
    {
    }

}
