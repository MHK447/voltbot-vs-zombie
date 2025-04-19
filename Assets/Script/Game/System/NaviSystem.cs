using System.Collections;
using System.Collections.Generic;
using BanpoFri;
using UnityEngine;

public class NaviSystem
{
    public enum NaviType
    {
        Conversation_1,
        Counter,
        Rack_01,
        Fish_01,
        Fishing,
        GoToBucket,
        RackFishAdd,
        CalcCounter,
        WaitCalc,
        UpgradeStart,
        UpgradeBtn,
        CloseUpgradeBtn,
        End,

    }

    public Queue<NaviType> NaviQueue = new Queue<NaviType>();

    public ArrowNaviUI NaviUI;

    public PopupConversation PopupConversation;

    public Dictionary<NaviType, GameObject> NaviArrowList = new Dictionary<NaviType, GameObject>();


    public List<NaviType> ClearNaviArrowList = new List<NaviType>();


    public NaviType CurNaviOnType = NaviType.End;

    public bool IsNaviOn = false;

    public void Create()
    {
        GameRoot.Instance.UISystem.LoadFloatingUI<ArrowNaviUI>((_naviui) =>
         {
             NaviUI = _naviui;
             ProjectUtility.SetActiveCheck(_naviui.gameObject, false);
         });

        GameRoot.Instance.UISystem.OpenUI<PopupConversation>(popup => popup.Hide());
    }


    public void FirstStartNavi()
    {
        var recordcount = GameRoot.Instance.UserData.GetRecordCount(Config.RecordCountKeys.Navi_Start);

        if (recordcount == 0)
        {
            IsNaviOn = true;
            GameRoot.Instance.UserData.AddRecordCount(Config.RecordCountKeys.Navi_Start, 1);
            for (int i = 0; i < (int)NaviType.End; ++i)
            {
                NaviQueue.Enqueue((NaviType)i);
            }
        }
    }


    public void StarNexttNavi()
    {
        if (NaviQueue.Count > 0)
        {
            var nextmove = NaviQueue.Dequeue();

            NaviOn(nextmove);
        }
    }

    public void NextNavi(NaviType type)
    {
        if (!IsNaviOn) return;

        if (!ClearNaviArrowList.Contains(type))
        {
            ClearNaviArrowList.Add(type);
            if (NaviUI != null)
            {
                ProjectUtility.SetActiveCheck(NaviUI.gameObject, false);
            }

            foreach (var navi in NaviArrowList)
            {
                ProjectUtility.SetActiveCheck(navi.Value.gameObject, false);
            }
            StarNexttNavi();
        }
    }

    public void NaviOff(NaviType naviontype)
    {
        if (NaviUI != null)
        {
            ProjectUtility.SetActiveCheck(NaviUI.gameObject, false);
        }

        foreach (var navi in NaviArrowList)
        {
            ProjectUtility.SetActiveCheck(navi.Value.gameObject, false);
        }

    }


    public void NaviOn(NaviType type)
    {
        if (NaviUI != null)
        {
            ProjectUtility.SetActiveCheck(NaviUI.gameObject, false);
        }

        foreach (var navi in NaviArrowList)
        {
            ProjectUtility.SetActiveCheck(navi.Value.gameObject, false);
        }

        var stage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;
        switch (type)
        {
            case NaviType.Conversation_1:
                {
                    GameRoot.Instance.GetJoyStick.IsLock = true;
                    GameRoot.Instance.UISystem.OpenUI<PopupConversation>(popup => popup.Set(Tables.Instance.GetTable<Localize>().GetString("food_01"), PopupConversation.OtterType.Happy), () =>
                    {
                        CurNaviOnType = NaviType.Counter;
                        NextNavi(CurNaviOnType);
                        GameRoot.Instance.GetJoyStick.IsLock = false;
                    });
                }
                break;
            case NaviType.Counter:
                {
                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.CheckoutCounter);

                    if (findfacility != null)
                    {
                        NaviUI.SetOffset(new Vector3(0, 3.5f, 0));
                        NaviUI.Init(findfacility.GetContentsOpenComponentTr);
                        ProjectUtility.SetActiveCheck(NaviUI.gameObject, true);
                    }
                }
                break;
            case NaviType.Rack_01:
                {
                    GameRoot.Instance.GetJoyStick.IsLock = true;
                    GameRoot.Instance.WaitTimeAndCallback(3.5f, () =>
                   {
                       GameRoot.Instance.UISystem.OpenUI<PopupConversation>(popup => popup.Set(Tables.Instance.GetTable<Localize>().GetString("food_09"), PopupConversation.OtterType.Idle), () =>
                       {

                           GameRoot.Instance.GetJoyStick.IsLock = false;
                       });
                   });

                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.RedSnapperDisplay);

                    if (findfacility != null)
                    {
                        NaviUI.SetOffset(new Vector3(0, 3.5f, 0));
                        NaviUI.Init(findfacility.GetContentsOpenComponentTr);
                        ProjectUtility.SetActiveCheck(NaviUI.gameObject, true);
                    }
                }
                break;
            case NaviType.Fish_01:
                {
                    GameRoot.Instance.GetJoyStick.IsLock = true;
                    GameRoot.Instance.WaitTimeAndCallback(3.5f, () =>
                   {
                       GameRoot.Instance.UISystem.OpenUI<PopupConversation>(popup => popup.Set(Tables.Instance.GetTable<Localize>().GetString("food_02"), PopupConversation.OtterType.Idle), () =>
                       {
                           GameRoot.Instance.GetJoyStick.IsLock = false;
                       });
                   });

                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.RedSnapperFishing);

                    if (findfacility != null)
                    {
                        NaviUI.SetOffset(new Vector3(0, 3.5f, 0));
                        NaviUI.Init(findfacility.GetContentsOpenComponentTr);
                        ProjectUtility.SetActiveCheck(NaviUI.gameObject, true);
                    }

                }
                break;
            case NaviType.Fishing:
                {

                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.RedSnapperFishing).GetComponent<FishRoomComponent>();

                    if (findfacility != null)
                    {
                        NaviUI.SetOffset(new Vector3(0, 1.35f, 0));
                        NaviUI.Init(findfacility.GetCushionComponent.transform);
                        ProjectUtility.SetActiveCheck(NaviUI.gameObject, true);
                    }

                }
                break;
            case NaviType.GoToBucket:
                {
                    GameRoot.Instance.GetJoyStick.IsLock = true;
                    GameRoot.Instance.UISystem.OpenUI<PopupConversation>(popup => popup.Set(Tables.Instance.GetTable<Localize>().GetString("food_03"), PopupConversation.OtterType.Idle),
                    () =>
                    {
                        GameRoot.Instance.GetJoyStick.IsLock = false;
                    });

                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.RedSnapperFishing).GetComponent<FishRoomComponent>();

                    if (findfacility != null)
                    {
                        NaviUI.Init(findfacility.GetBucketComponent.transform);
                        ProjectUtility.SetActiveCheck(NaviUI.gameObject, true);
                    }
                }
                break;
            case NaviType.RackFishAdd:
                {
                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.RedSnapperDisplay);

                    if (findfacility != null)
                    {
                        NaviUI.Init(findfacility.transform);
                        ProjectUtility.SetActiveCheck(NaviUI.gameObject, true);
                    }
                }
                break;
            case NaviType.CalcCounter:
                {
                    GameRoot.Instance.GetJoyStick.IsLock = true;
                    GameRoot.Instance.UISystem.OpenUI<PopupConversation>(popup => popup.Set(Tables.Instance.GetTable<Localize>().GetString("food_04"), PopupConversation.OtterType.Idle), () =>
                    {
                        GameRoot.Instance.GetJoyStick.IsLock = false;
                    });

                    var findfacility = stage.FindFacility((int)Config.FacilityTypeIdx.CheckoutCounter);

                    if (findfacility != null)
                    {
                        NaviUI.SetOffset(new Vector3(0, 1f, 0));
                        NaviUI.Init(findfacility.GetContentsOpenComponentTr);
                        ProjectUtility.SetActiveCheck(NaviUI.gameObject, true);
                    }
                }
                break;
            case NaviType.UpgradeStart:
                {
                    GameRoot.Instance.GetJoyStick.IsLock = true;
                    GameRoot.Instance.UISystem.OpenUI<PopupConversation>(popup => popup.Set(Tables.Instance.GetTable<Localize>().GetString("food_05"), PopupConversation.OtterType.Happy), () =>
                    {
                        ProjectUtility.SetActiveCheck(GameRoot.Instance.UISystem.GetUI<HUDTotal>()?.GetUpgradeBtn.gameObject, true);
                        GameRoot.Instance.GetJoyStick.IsLock = false;
                        ProjectUtility.SetActiveCheck(NaviArrowList[NaviType.UpgradeStart], true);
                    });

                }
                break;
            case NaviType.UpgradeBtn:
                {
                    ProjectUtility.SetActiveCheck(NaviArrowList[NaviType.UpgradeBtn], true);
                }
                break;
            case NaviType.CloseUpgradeBtn:
                {
                    GameRoot.Instance.GetJoyStick.IsLock = true;
                    GameRoot.Instance.UISystem.OpenUI<PopupConversation>(popup => popup.Set(Tables.Instance.GetTable<Localize>().GetString("food_06"), PopupConversation.OtterType.Happy), () =>
                    {
                        GameRoot.Instance.GetJoyStick.IsLock = false;
                        IsNaviOn = false;
                    });
                }
                break;
        }
    }
}
