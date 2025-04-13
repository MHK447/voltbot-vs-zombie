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

    [SerializeField]
    private OtterBase Player;

    public OtterBase GetPlayer { get { return Player; } }


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

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var td = Tables.Instance.GetTable<StageInfo>().GetData(stageidx);

        if (td != null)
        {
            Addressables.InstantiateAsync($"InGame1_{stageidx}").Completed += (handle) =>
            {
                StartCoroutine(UpdateNavMeshProcess());
                curInGameStage = handle.Result.GetComponent<InGameStage>();
                if (curInGameStage != null)
                {
                    curInGameStage.Init();
                }

                Player.Init();

                GameRoot.Instance.WaitTimeAndCallback(1f, () =>
                {
                    var recordcount = GameRoot.Instance.UserData.GetRecordCount(Config.RecordCountKeys.Navi_Start);

                    if (GameRoot.Instance.UserData.CurMode.StageData.StageIdx == 1 && recordcount == 0)
                    {
                        GameRoot.Instance.UISystem.GetUI<HUDTotal>()?.GetUpgradeBtn.gameObject.SetActive(false);
                        GameRoot.Instance.UISystem.OpenUI<PopupDragTuto>(null, () =>
                        {
                            GameRoot.Instance.NaviSystem.FirstStartNavi();
                            GameRoot.Instance.NaviSystem.StarNexttNavi();
                        });
                    }
                });
            };
        }

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
