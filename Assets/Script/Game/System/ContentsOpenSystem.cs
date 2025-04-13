using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;

public class ContentsOpenSystem : MonoBehaviour
{
    public enum ContentsOpenType
    {
        BoostBuff = 1,
        AdVehicle = 2,
        NextStageBtn = 3,
        Interstitial = 4,
    }


    public bool ContentsOpenCheck(ContentsOpenType opentype)
    {
        bool isopencheck = false;

        var td = Tables.Instance.GetTable<ContentsOpenCheck>().GetData((int)opentype);

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        if(td != null)
        {
            var findfacility = GameRoot.Instance.UserData.CurMode.StageData.FindFacilityData(td.open_facilityidx);
            

            if(stageidx > td.stage_idx)
            {
                isopencheck = true;
            }
            else if(stageidx == td.stage_idx && findfacility.IsOpen)
            {
                isopencheck = true;
            }
        }

        return isopencheck;

    }


}
