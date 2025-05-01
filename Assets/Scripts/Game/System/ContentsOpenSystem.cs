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

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.Stageidx.Value;

        if(td != null)
        {
           

            if(stageidx > td.stage_idx)
            {
                isopencheck = true;
            }
            // else if(stageidx == td.stage_idx && findfacility.IsOpen)
            // {
            //     isopencheck = true;
            // }
        }

        return isopencheck;

    }


}
