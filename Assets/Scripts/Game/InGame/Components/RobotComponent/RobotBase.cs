using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class RobotBase : MonoBehaviour
{
    public enum RobotStateType
    {
        Idle,
        Move,
        Attack,
        Dead,
    }

    public enum DirectionType
    {
        Left,
        Right,
    }

    [SerializeField]
    private Animator RobotAnim;

    [SerializeField]
    private SpriteRenderer RobotSprite;

    protected RobotStateType StateType = RobotStateType.Idle;

    protected DirectionType Direciton = DirectionType.Left;

    public abstract void SetInfo();

    protected int UnitIdx = 0;

    public int GetUnitIdx { get { return UnitIdx; } }

    protected RobotBase Target = null;



    public virtual void Set(int unitidx)
    {
        UnitIdx = unitidx;
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


        switch (StateType)
        {
            case RobotStateType.Idle:
                RobotAnim.SetBool("Idle", true);
                break;
            case RobotStateType.Move:
                {
                    var movedirection = Direciton == DirectionType.Left ? "Left" : "Right";
                    RobotAnim.SetBool(movedirection, false);
                }
                break;
            case RobotStateType.Attack:
                RobotAnim.SetBool("Attack", false);
                break;

        }
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
