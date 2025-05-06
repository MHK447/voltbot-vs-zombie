using System.Collections;
using System.Collections.Generic;
using BanpoFri;
using UnityEngine;
using DG.Tweening;

public class MeleeEnemy : RobotBase
{
    public MeleeUnitData UnitData = null;


    override public void Set(int unitidx)
    {
        base.Set(unitidx); 

        SetInfo();

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

        PlayStateAnimation(state);
    }
    override public void PlayStateAnimation(RobotStateType state)
    {
        base.PlayStateAnimation(state);

        switch (state)
        {
            case RobotStateType.Attack:
                {
                    stateTween = DOTween.Sequence()
                        .Append(RobotSprite.transform.DOShakePosition(0.2f, 0.1f, 10, 90, false))
                        .Join(RobotSprite.transform.DOScale(1.1f, 0.2f))
                        .Append(RobotSprite.transform.DOScale(1f, 0.2f));

                }
                break;

        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (UnitData == null) return;

        Vector3 pos = transform.position;
        pos.x -= UnitData.MoveSpeed * Time.deltaTime;
        transform.position = pos;
    }
}
