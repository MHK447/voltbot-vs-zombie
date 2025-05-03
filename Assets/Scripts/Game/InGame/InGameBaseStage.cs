using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using BanpoFri;
using UniRx;
using System.Linq;
using UnityEngine.AI;
using NavMeshPlus.Components;
using Unity.VisualScripting;

public class InGameBaseStage : InGameMode
{
    public InGameBattle curInGameBattle;

    private int ProductHeroIdxs = 0;
    public override void Load()
    {
        base.Load();

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.Stageidx.Value;

        var td = Tables.Instance.GetTable<StageInfo>().GetData(stageidx);

        if (td != null)
        {
            GameRoot.Instance.UISystem.OpenUI<PageLobbyBattle>();
        }
    }
    protected override void LoadUI()
    {
        base.LoadUI();
        GameRoot.Instance.InGameSystem.InitPopups();
    }


    public override void UnLoad()
    {
        base.UnLoad();
    }
}
