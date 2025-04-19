using System.Collections;
using System.Collections.Generic;
using BanpoFri;
using UnityEngine;
using UniRx;

public class VehicleSystem
{
    public int ad_ride_time = 0;
    public int ride_cash_value = 0;
    public int ad_retimer_time = 0;

    public int ad_vehicle_time = 0;

    public int ad_vehicle_show_time = 0;


    public IReactiveProperty<int> AdVehiceTimeProperty = new ReactiveProperty<int>(0);

    public bool IsAdEquipVehicle = false;
    public Vector2[] AdVehiclePoints = new Vector2[]
    {
        new Vector2(-7.13f, -2.15f),
        new Vector2(2.59f, 2.59f),
        new Vector2(6.5f, 0.28f),
        new Vector2(1.15f, -4.5f),
    };

    public int AdVehicleShowTime = 0;

    public bool IsShowAdVehicle = false;

    public void Create()
    {
        ad_ride_time = Tables.Instance.GetTable<Define>().GetData("ad_ride_time").value;
        ride_cash_value = Tables.Instance.GetTable<Define>().GetData("ride_cash_value").value;
        ad_retimer_time = Tables.Instance.GetTable<Define>().GetData("ad_retimer_time").value;
        ad_vehicle_time = Tables.Instance.GetTable<Define>().GetData("ad_vehicle_time").value;
        ad_vehicle_show_time = Tables.Instance.GetTable<Define>().GetData("ad_vehicle_show_time").value;

        AdVehiceTimeProperty.Value = 0;

        AdVehicleShowTime = 0;
    }



    public void OneSecondUpdate()
    {
        if(!GameRoot.Instance.ContentsOpenSystem.ContentsOpenCheck(ContentsOpenSystem.ContentsOpenType.AdVehicle)) return;

        if (IsAdEquipVehicle && AdVehiceTimeProperty.Value > 0)
        {
            AdVehiceTimeProperty.Value -= 1;
            IsShowAdVehicle = false;
        }

        if (!IsAdEquipVehicle && AdVehicleShowTime < ad_vehicle_show_time && !IsShowAdVehicle)
        {
            AdVehicleShowTime += 1;

            if (AdVehicleShowTime >= ad_vehicle_show_time)
            {
                GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.ActiveOnAdVehicle();
                AdVehicleShowTime = 0;
                IsShowAdVehicle = true;
            }
        }
    }

    public void AdVehicleActive(bool isactive)
    {
        IsAdEquipVehicle = isactive;
        AdVehiceTimeProperty.Value = isactive ? ad_ride_time : 0;
        GameRoot.Instance.UserData.CurMode.PlayerData.VehiclePropertyIdx.Value = isactive ? 1 : 0;
    }


}
