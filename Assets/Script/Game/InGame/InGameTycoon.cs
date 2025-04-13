using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using BanpoFri;
using UniRx;
using System.Linq;
using UnityEngine.AI;
using NavMeshPlus.Components;

public class InGameTycoon : InGameMode
{

    [HideInInspector]
    public InGameStage curInGameStage;

    [SerializeField]
    private NavMeshSurface NavMeshSurFace;


    public IReactiveProperty<int> FarsightedTimeProperty = new ReactiveProperty<int>(0);
    public IReactiveProperty<bool> MaxMode = new ReactiveProperty<bool>(true);
    public IReactiveProperty<float> GameSpeedMultiValue = new ReactiveProperty<float>(1f);
    //private int dustMaxCnt = 20;


    private int ProductHeroIdxs = 0;
    public override void Load()
    {
        base.Load();
        
        //CalculateGameSpeed();

    }

    //public void CalculateGameSpeed()
    //{
    //    var buffValue = GameRoot.Instance.BuffSystem.GetValueByBuffType(BuffSystem.BuffType.ProductSpeed);
    //    GameSpeedMultiValue.Value = 1f - buffValue;
    //}

    protected override void LoadUI()
    {
        base.LoadUI();
        GameRoot.Instance.InGameSystem.InitPopups();
        GameRoot.Instance.UISystem.OpenUI<HUDTotal>();
    }


    public override void UnLoad()
    {
        base.UnLoad();
    }


    private IEnumerator UpdateNavMeshProcess()
    {
        AsyncOperation asyncOp = NavMeshSurFace.UpdateNavMesh(NavMeshSurFace.navMeshData);
        yield return new WaitUntil(() => asyncOp.isDone);

        //navUpdateCallBack?.Invoke();
        //_updateNavMeshProcess = null;
        yield break;
    }
}
