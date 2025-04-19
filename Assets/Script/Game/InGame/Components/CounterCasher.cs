using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterCasher : OtterBase
{

    private int FacilityIdx = 0;

    public int GetFacilityIdx { get { return FacilityIdx; } }

    public override void Init()
    {
        base.Init();

        CasherMoveSpeed = 4f;

        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;

        _navMeshAgent.enabled = true;

        GameRoot.Instance.StartCoroutine(WaitOneFrame());

        CurState = OtterState.Idle;

        CurStage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;

        GameRoot.Instance.WaitTimeAndCallback(1f, () => { StartWork(); });
    }

    public void CalcFish(bool iscalc)
    {
        if (iscalc)
        {
            PlayAnimation(OtterState.Counter, "counter", true);
        }
        else
        {
            PlayAnimation(OtterState.Idle, "counter", true);
        }
    }


    public void StartWork()
    {
        SetDestination(CurStage.CounterCasherTr, () =>
        {
            ReachProcess();
        });
    }

    public override void Update()
    {
        base.Update();
    }
}
