using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RobotBase : MonoBehaviour
{

    [SerializeField]
    private Animator RobotAnim;
    
    [SerializeField]
    private SpriteRenderer RobotSprite;


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
}
