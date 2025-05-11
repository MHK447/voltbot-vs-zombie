using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;

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
            InGameHpProgress.transform.position = new Vector2(20000, 20000);
            InGameHpProgress.SetHpText(UnitData.CurHpProperty.Value, UnitData.MaxHpProperty.Value);
            GameRoot.Instance.WaitTimeAndCallback(0.25f, () =>
            {
                InGameHpProgress.Init(HpProgressTr);
                ProjectUtility.SetActiveCheck(InGameHpProgress.gameObject, true);
            });
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


    public override void Update()
    {
        base.Update();

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

                {
                    var directionpos = Direciton == DirectionType.Left ? 15f : -15f;
                    stateTween = DOTween.Sequence()
                    .Append(RobotSprite.transform.DORotate(new Vector3(0, 0, directionpos), 0.1f)) // 고개 앞으로
                    .Join(RobotSprite.transform.DOShakePosition(0.2f, 0.1f, 10, 90, false))
                    .Join(RobotSprite.transform.DOScale(RobotImgScaleVec * 1.1f, 0.2f))
                    .AppendCallback(() =>
                    {
                        // 🟢 공격 데미지 적용 타이밍
                        if (Target != null && !Target.IsDeath)
                        {
                            AttackAction?.Invoke();
                        }
                    })
                    .Append(RobotSprite.transform.DORotate(Vector3.zero, 0.1f)) // 고개 복원
                    .Append(RobotSprite.transform.DOScale(RobotImgScaleVec, 0.2f))
                    .SetLoops(-1, LoopType.Restart); // 반복 설정 (-1: 무한 반복)
                    break;
                }

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
