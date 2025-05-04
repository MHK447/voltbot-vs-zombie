using System.Collections;
using System.Collections.Generic;
using BanpoFri;
using UnityEngine;

public class MeleeEnemy : RobotBase
{
    public MeleeUnitData UnitData = null;

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
