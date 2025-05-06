using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using DG.Tweening;

public abstract class RobotBase : MonoBehaviour
{
    public enum RobotStateType { Idle, Move, Attack, Dead }
    public enum DirectionType { Left, Right }


    [SerializeField]
    protected SpriteRenderer RobotSprite;

    [SerializeField]
    private protected Transform HpProgressTr;

    protected RobotStateType StateType = RobotStateType.Idle;

    protected DirectionType Direciton = DirectionType.Left;

    protected InGameEnemyHpProgress InGameHpProgress = null;

    public abstract void SetInfo();

    protected int UnitIdx = 0;

    public int GetUnitIdx { get { return UnitIdx; } }

    protected RobotBase Target = null;
    protected Tween stateTween; // 현재 상태 트윈

    private Vector3 originalScale;

    void Awake()
    {
        originalScale = RobotSprite.transform.localScale;
    }

    public virtual void Set(int unitidx)
    {
        UnitIdx = unitidx;

        PlayStateAnimation(RobotStateType.Move);
    }


    public virtual void Attack()
    {

    }


    public virtual void Damage(double damage)
    {

    }

    public virtual void Dead()
    {

    }

    public virtual void SetTarget()
    {

    }

    public virtual void SetDirection(DirectionType direction)
    {

    }


    public virtual void SetState(RobotStateType state)
    {
        StateType = state;
        PlayStateAnimation(state);
    }

    public virtual void PlayStateAnimation(RobotStateType state)
    {
        stateTween?.Kill(); // 기존 트윈 제거
        RobotSprite.transform.localScale = originalScale;

        switch (state)
        {
            case RobotStateType.Idle:
                stateTween = RobotSprite.transform
                    .DOScale(originalScale * 1.05f, 1.2f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
                break;

            case RobotStateType.Move:
                Vector3 leftLean = new Vector3(3f, -3f, -8f);  // 왼쪽 기울기 + 몸통 살짝 왼쪽으로 틀기
                Vector3 rightLean = new Vector3(-3f, 3f,8f);  // 오른쪽 기울기 + 몸통 살짝 오른쪽으로 틀기

                stateTween = RobotSprite.transform
                    .DOLocalRotate(rightLean, 0.35f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .From(leftLean)
                    .SetEase(Ease.InOutSine);
                break;

            case RobotStateType.Attack:
                stateTween = DOTween.Sequence()
                    .Append(RobotSprite.transform.DOShakePosition(0.2f, 0.1f, 10, 90, false))
                    .Join(RobotSprite.transform.DOScale(originalScale * 1.1f, 0.2f))
                    .Append(RobotSprite.transform.DOScale(originalScale, 0.2f));
                break;

            case RobotStateType.Dead:
                stateTween = RobotSprite.transform
                    .DOScale(Vector3.zero, 1.0f)
                    .SetEase(Ease.InBack);
                break;
        }
    }



    public class MeleeUnitData
    {
        public IReactiveProperty<double> CurHpProperty = new ReactiveProperty<double>(0);
        public IReactiveProperty<double> MaxHpProperty = new ReactiveProperty<double>(0);

        public float MoveSpeed = 0;


        public float AttackSpeed = 0;


        public double AttackDamage = 0;

        public MeleeUnitData(double starthp, float movespeed, float attackspeed, double attackdamage)
        {
            MaxHpProperty.Value = CurHpProperty.Value = starthp;
            MoveSpeed = movespeed;
            AttackSpeed = attackspeed;
            AttackDamage = attackdamage;
        }
    }
}
