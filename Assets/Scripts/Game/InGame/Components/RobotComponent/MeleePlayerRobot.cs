using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;

public class MeleePlayerRobot : MeleeUnitBase
{
    public override void Set(int unitidx)
    {
        base.Set(unitidx);
    }

    public override void SetInfo()
    {
        var td = Tables.Instance.GetTable<RobotInfo>().GetData(UnitIdx);

        if (td != null)
        {
            var starthp = td.base_hp;
            var attackspeed = td.base_attack_speed / 100f;
            var damage = td.base_attack_damage;
            var movespeed = td.base_move_speed / 100f;

            UnitData = new MeleeUnitData(starthp, movespeed, attackspeed, damage);
        }
    }


    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (UnitData == null) return;

        if (Battle == null) return;


        switch (StateType)
        {
            case RobotStateType.Move:
                {
                    if (Target == null)
                    {
                        Vector3 pos = transform.position;
                        pos.x += UnitData.MoveSpeed * Time.deltaTime;
                        transform.position = pos;
                    }
                    else
                    {
                        // 타겟이 공격 범위 밖에 있으면 이동
                        if (StateType != RobotStateType.Move)
                        {
                            SetState(RobotStateType.Move);
                        }

                        // 타겟 방향 계산
                        Vector3 direction = (Target.transform.position - transform.position).normalized;

                        // 로봇을 타겟 방향으로 이동
                        Vector3 newPosition = transform.position + direction * UnitData.MoveSpeed * Time.deltaTime;

                        // 로봇 위치 업데이트
                        transform.position = newPosition;
                    }
                }
                break;
            case RobotStateType.Attack:
                {
                    if(Target == null)
                    {
                        SetState(RobotStateType.Move);
                    }
                }
                break;
        }

        if (Target == null)
        {
            Target = Battle.GetStageMap.GetTarget(transform.position, InGameStageMap.UnitType.Enemy);
        }
        else
        {


            float distance = Vector3.Distance(this.transform.position, Target.transform.position);

            if (distance <= AttackRange)
            {
                if (StateType != RobotStateType.Attack)
                {
                    SetState(RobotStateType.Attack);
                }
            }
            else
            {
                if (StateType != RobotStateType.Move)
                {
                    SetState(RobotStateType.Move);
                }
            }
        }

    }


    public override void Damage(double damage)
    {
        base.Damage(damage);

        UnitData.CurHpProperty.Value -= damage;

        DamageColorEffect();

        InGameHpProgress?.SetHpText(UnitData.CurHpProperty.Value, UnitData.MaxHpProperty.Value);

        if (UnitData.CurHpProperty.Value <= 0)
        {
            Dead();
        }

    }



}
