using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using DG.Tweening;

public class MeleeUnitBase : RobotBase
{
    protected MeleeUnitData UnitData = null;


    override public void Set(int unitidx)
    {
        UnitIdx = unitidx;

        base.Set(unitidx);

        SetInfo();

        AttackAction = Attack;

        if (InGameHpProgress != null)
        {
            ProjectUtility.SetActiveCheck(InGameHpProgress.gameObject, true);
            InGameHpProgress.SetHpText(UnitData.CurHpProperty.Value, UnitData.MaxHpProperty.Value);
            InGameHpProgress.Init(HpProgressTr);
        }
        else
        {
            GameRoot.Instance.UISystem.LoadFloatingUI<InGameEnemyHpProgress>(hpprogress =>
            {
                InGameHpProgress = hpprogress;
                hpprogress.Init(HpProgressTr);
                hpprogress.SetHpText(UnitData.CurHpProperty.Value, UnitData.MaxHpProperty.Value);
                ProjectUtility.SetActiveCheck(hpprogress.gameObject, true);
            });

        }
    }

    public override void SetInfo()
    {

    }


    public virtual void Update()
    {
        if (Target != null && Target.IsDeath)
        {
            Target = null;
        }

    }


    override public void SetState(RobotStateType state)
    {
        base.SetState(state);

        PlayStateAnimation(state);
    }
    override public void PlayStateAnimation(RobotStateType state)
    {
        base.PlayStateAnimation(state);

        switch (state)
        {

            case RobotStateType.Attack:
                stateTween = DOTween.Sequence()
                    .Append(RobotSprite.transform.DOShakePosition(0.2f, 0.1f, 10, 90, false))
                    .Join(RobotSprite.transform.DOScale(originalScale * 1.1f, 0.2f))
                    .AppendCallback(() =>
                    {
                        // 🟢 공격 데미지 적용 타이밍
                        if (Target != null && !Target.IsDeath)
                        {
                            AttackAction?.Invoke();
                        }
                    })
                    .Append(RobotSprite.transform.DOScale(originalScale, 0.2f))
                    .SetLoops(-1, LoopType.Restart); // 반복 설정 (-1: 무한 반복)
                break;

        }
    }

    public virtual void Attack()
    {
        if (Target != null)
        {
            Target.Damage(UnitData.AttackDamage);

            if (Target.IsDeath)
            {
                Target = null;

                SetState(RobotStateType.Move);
            }

        }
    }

}
