using System.Collections;
using System.Collections.Generic;
using BanpoFri;
using UnityEngine;
using DG.Tweening;

public class MeleeEnemy : MeleeUnitBase
{
    override public void Set(int unitidx)
    {
        base.Set(unitidx);

        GameRoot.Instance.WaitTimeAndCallback(0.5f, () =>
        {
            Target = Battle.GetStageMap.GetTarget(transform.position, InGameStageMap.UnitType.Player);
        });
    }

    public override void SetInfo()
    {
        var td = Tables.Instance.GetTable<EnemyInfo>().GetData(UnitIdx);

        var stagetd = Tables.Instance.GetTable<StageInfo>().GetData(GameRoot.Instance.UserData.CurMode.StageData.Stageidx.Value);



        if (td != null && stagetd != null)
        {

            double starthp = td.base_hp + ProjectUtility.PercentCalc(td.base_hp, stagetd.enemy_increase_hp);

            float movespeed = (float)td.base_move_speed / 100f;

            float attackspeed = (float)td.base_attack_speed / 100f;

            double attackdamage = td.base_attack_damage + ProjectUtility.PercentCalc(td.base_attack_damage, stagetd.enemy_increase_attack);

            UnitData = new MeleeUnitData(starthp, movespeed, attackspeed, attackdamage);

        }

    }


    override public void SetState(RobotStateType state)
    {
        base.SetState(state);
    }
    override public void PlayStateAnimation(RobotStateType state)
    {
        base.PlayStateAnimation(state);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public override void Update()
    {
        if (UnitData == null) return;

        if (Battle == null) return;

        if (Target != null && Target.IsDeath)
        {
            Target = null;
        }


        switch (StateType)
        {
            case RobotStateType.Move:
                {
                    if (Target == null)
                    {
                        Vector3 pos = transform.position;
                        pos.x -= UnitData.MoveSpeed * Time.deltaTime;
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
        }

        if (Target == null)
        {
            Target = Battle.GetStageMap.GetTarget(transform.position, InGameStageMap.UnitType.Player);
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

            GameRoot.Instance.UserData.Stagedata.Enemycount.Value -= 1;


            if (GameRoot.Instance.UserData.Stagedata.Enemycount.Value <= 0)
            {
                Battle.GetStageMap.NextWave();
            }
        }
    }


    public void Attack()
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
