using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using Unity.VisualScripting;

public class AdVehicleComponent : MonoBehaviour
{


    private OtterBase TargetObj;


    private bool IsOpenPopup = false;

    private int ActiveTime = 0;

    private VehicleUI VehicleTextUI;

    private VehicleBubbleUI BubbleUI;

    [SerializeField]
    private Transform VehicleTr;

    [SerializeField]
    private Transform VehicleBubbleTr;

    public void Init()
    {
        IsOpenPopup = false;
        TargetObj = null;
        ActiveTime = GameRoot.Instance.VehicleSystem.ad_vehicle_time;


        var buffvalue = Tables.Instance.GetTable<VehicleInfo>().GetData(1).buff_value;


        if (BubbleUI == null)
        {
            GameRoot.Instance.UISystem.LoadFloatingUI<VehicleBubbleUI>((_progress) =>
            {
                BubbleUI = _progress;
                ProjectUtility.SetActiveCheck(BubbleUI.gameObject, true);
                BubbleUI.Init(VehicleBubbleTr);
            });
        }
        else
        {
            BubbleUI.Init(VehicleBubbleTr);
            ProjectUtility.SetActiveCheck(BubbleUI.gameObject, true);
        }


        if (VehicleTextUI == null)
        {
            GameRoot.Instance.UISystem.LoadFloatingUI<VehicleUI>((vehicleui) =>
            {
                VehicleTextUI = vehicleui;
                ProjectUtility.SetActiveCheck(VehicleTextUI.gameObject, true);
                VehicleTextUI.Init(VehicleTr);
                VehicleTextUI.SetText(buffvalue);
            });
        }
        else
        {
            VehicleTextUI.Init(VehicleTr);
            ProjectUtility.SetActiveCheck(VehicleTextUI.gameObject, true);
            VehicleTextUI.SetText(buffvalue);
        }

        

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            TargetObj = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().GetPlayer;
        }
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            TargetObj = null;
            IsOpenPopup = false;
        }
    }

    public void ActiveOff()
    {
        if(BubbleUI != null)
        {
            ProjectUtility.SetActiveCheck(BubbleUI.gameObject , false);
        }

        if(VehicleTextUI != null)
        {
            ProjectUtility.SetActiveCheck(VehicleTextUI.gameObject, false);
        }

        ProjectUtility.SetActiveCheck(this.gameObject , false);
    }


    void Update()
    {
        if (ActiveTime <= 0)
        {
            ProjectUtility.SetActiveCheck(this.gameObject, false);
            GameRoot.Instance.VehicleSystem.IsShowAdVehicle = false;
            GameRoot.Instance.VehicleSystem.AdVehicleShowTime = 0;

            if (VehicleTextUI != null)
                ProjectUtility.SetActiveCheck(VehicleTextUI.gameObject, false);
        }

        if (TargetObj != null)
        {
            if (TargetObj.IsIdle && !IsOpenPopup)
            {
                IsOpenPopup = true;
                GameRoot.Instance.UISystem.OpenUI<PopupVehicle>(popup => popup.Init());
            }
        }
    }



}
