using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public abstract class RobotBase : MonoBehaviour
{
    public enum RobotStateType { Idle, Move, Attack, Dead }
    public enum DirectionType { Left, Right }

    [SerializeField]
    protected float AttackRange = 2;


    [SerializeField]
    protected SpriteRenderer RobotSprite;

    [SerializeField]
    private protected Transform HpProgressTr;

    protected RobotStateType StateType = RobotStateType.Idle;

    protected DirectionType Direciton = DirectionType.Left;

    protected InGameEnemyHpProgress InGameHpProgress = null;

    protected InGameBattle Battle = null;

    public abstract void SetInfo();

    protected int UnitIdx = 0;

    public int GetUnitIdx { get { return UnitIdx; } }

    public bool IsDeath { get { return StateType == RobotStateType.Dead; } }

    private bool IsDamageDirect = false;

    protected RobotBase Target = null;
    protected Tween stateTween; // 현재 상태 트윈

    protected Vector3 originalScale;

    protected System.Action AttackAction = null;

    void Awake()
    {
        originalScale = RobotSprite.transform.localScale;
    }

    public virtual void Set(int unitidx)
    {
        RobotSprite.material = Config.Instance.DefaultSpriteMaterial;

        // 알파값 초기화
        Color color = RobotSprite.material.GetColor("_Color");
        color.a = 1f;
        RobotSprite.material.SetColor("_Color", color);

        IsDamageDirect = false;

        UnitIdx = unitidx;

        StateType = RobotStateType.Move;

        PlayStateAnimation(RobotStateType.Move);

        Battle = GameRoot.Instance.InGameSystem.GetInGame<InGameBaseStage>().curInGameBattle;
    }

    public virtual void Damage(double damage)
    {


    }

    public virtual void Dead()
    {
        if(InGameHpProgress != null)
        {
            ProjectUtility.SetActiveCheck(InGameHpProgress.gameObject, false);
        }

        ProjectUtility.SetActiveCheck(this.gameObject, false);

        SetState(RobotStateType.Dead);
    }

    public virtual void SetTarget()
    {

    }

    public virtual void SetDirection(DirectionType direction)
    {

    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // 기즈모 색상 설정
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, AttackRange);

        Gizmos.color = Color.blue;
    }
#endif


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
                Vector3 rightLean = new Vector3(-3f, 3f, 8f);  // 오른쪽 기울기 + 몸통 살짝 오른쪽으로 틀기

                stateTween = RobotSprite.transform
                    .DOLocalRotate(rightLean, 0.35f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .From(leftLean)
                    .SetEase(Ease.InOutSine);
                break;
            case RobotStateType.Dead:
                stateTween = RobotSprite.transform
                    .DOScale(Vector3.zero, 1.0f)
                    .SetEase(Ease.InBack);
                break;
        }
    }

    public void DamageColorEffect()
    {
        if (!IsDamageDirect)
        {
            IsDamageDirect = true;

            RobotSprite.material = Config.Instance.DamageEffectMaterial;


            // 알파값 설정 (기존 코드와 동일한 효과)
            Color color = RobotSprite.material.GetColor("_Color");
            color.a = 1f;
            RobotSprite.material.SetColor("_Color", color);

            // _SelfIllum 값도 설정 (기존 코드 참고)
            RobotSprite.material.SetFloat("_SelfIllum", 1f);
            RobotSprite.material.SetFloat("_FlashAmount", 0.7f);

            GameRoot.Instance.WaitTimeAndCallback(0.15f, () =>
            {
                if (this != null)
                {

                    RobotSprite.material = Config.Instance.DefaultSpriteMaterial;

                    // 알파값 초기화
                    Color color = RobotSprite.material.GetColor("_Color");
                    color.a = 1f;
                    RobotSprite.material.SetColor("_Color", color);

                    IsDamageDirect = false;
                }
            });
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
