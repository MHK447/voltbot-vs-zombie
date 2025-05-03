using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.AddressableAssets;
using System.Linq;

public class InGameBattle : MonoBehaviour
{

    private InGameStageMap StageMap;

    public InGameStageMap GetStageMap { get { return StageMap; } }



    public void Init()
    {

    }


    public void StartBattle()
    {
        if (StageMap != null)
        {
            Destroy(StageMap.gameObject);
        }

        var stagetd = Tables.Instance.GetTable<StageInfo>().GetData(GameRoot.Instance.UserData.CurMode.StageData.Stageidx.Value);

        if (stagetd != null)
        {
            Addressables.InstantiateAsync(stagetd.prefab).Completed += (handle) =>
          {
              StageMap = handle.Result.GetComponent<InGameStageMap>();

              if (StageMap != null)
              {
                  ProjectUtility.SetActiveCheck(StageMap.gameObject, true);

              }
          };
        }

    }
}
