using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Linq;
using BanpoFri;

public enum CasherType
{
    CounterCasher = 1,
    CarryCasher = 2,
    FishingCasher = 3,
}

public class InGameStage : MonoBehaviour
{
    public bool IsLoadComplete { get; private set; }

    private CompositeDisposable disposable = new CompositeDisposable();

    [SerializeField]
    private List<Transform> WaitLineListTr = new List<Transform>();

    [SerializeField]
    private List<Transform> StartWayPointTrList = new List<Transform>();

    [SerializeField]
    private Transform EndTr;

    public Transform GetEndTr { get { return EndTr; } }

    [SerializeField]
    private Transform MiddleEndTr;

    public Transform GetMiddleEndTr { get { return MiddleEndTr; } }

    [SerializeField]
    private AssetReference FishRef;

    [SerializeField]
    private AssetReference ConsumerRef;

    [SerializeField]
    private List<Transform> AdVehicleTrList = new List<Transform>();


    public void Init()
    {
        IsLoadComplete = false;
        disposable.Clear();
        
        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;
    }


    public void ReturnMainScreen()
    {
        GameRoot.Instance.UserData.CurMode.GachaCoin.Value = 0;
    }

    private void OnDestroy()
    {
        disposable.Clear();
    }




    public Transform GetWaitLine(int order)
    {
        if (WaitLineListTr.Count > order)
        {
            return WaitLineListTr[order];
        }

        return WaitLineListTr.First();
    }



}
