using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;
using UnityEngine.AddressableAssets;

public class InGameStageMap : MonoBehaviour
{
    public enum UnitType
    {
        Enemy = 1,
        Player = 2,
    }

    private List<RobotBase> EnemyRobots = new List<RobotBase>();

    private List<RobotBase> PlayerRobots = new List<RobotBase>();

    [SerializeField]
    private List<Transform> EnemySpawnTr = new List<Transform>();

    [SerializeField]
    private List<Transform> PlayerSpawnTr = new List<Transform>();

    private bool IsStartBattle = false;

    private bool IsSpawnEnd = false;


    public IEnumerator StartWaveBattle(int waveidx)
    {
        IsStartBattle = true;

        var stageidx = GameRoot.Instance.UserData.CurMode.StageData.Stageidx.Value;

        var wavetd = Tables.Instance.GetTable<WaveInfo>().GetData(new KeyValuePair<int, int>(stageidx, waveidx));

        IsSpawnEnd = false;

        if (wavetd != null)
        {
            for (int i = 0; i < wavetd.enemy_idx.Count; ++i)
            {
                float wavetime = (float)wavetd.time[i] / 100f;
                for (int j = 0; j < wavetd.count[i]; ++j)
                {
                    CreateEnemyRobot(wavetd.enemy_idx[i]);

                    yield return new WaitForSeconds(wavetime);
                }
            }


            IsSpawnEnd = true;

            GameRoot.Instance.UserData.CurMode.StageData.Waveidx.Value++;

            GameRoot.Instance.UISystem.GetUI<PopupIngameBottom>()?.ActiveSelectOn();
        }
    }

    public void InitClear()
    {
    }

    public void SpawnEnemy(int unitidx)
    {
    }

    public void NextWave()
    {

    }


    public void Update()
    {

    }



    public void SetDamageUI(Transform damageuitr, int damage)
    {

    }

    public void CreatePlayerRobot(int unitidx, bool OnLoad = false)
    {
        var td = Tables.Instance.GetTable<RobotInfo>().GetData(unitidx);

        if (td != null)
        {
            var randvalue = Random.Range(0, PlayerSpawnTr.Count);

            var finddata = PlayerRobots.Find(x => x.GetUnitIdx == unitidx && x.gameObject.activeSelf == false);

            if (finddata == null || OnLoad)
            {
                Addressables.InstantiateAsync(td.prefab, PlayerSpawnTr[randvalue], false).Completed += (handle) =>
                           {
                               var getrobot = handle.Result.GetComponent<RobotBase>();

                               if (getrobot != null)
                               {
                                   getrobot.Set(unitidx);
                               }

                               ProjectUtility.SetActiveCheck(getrobot.gameObject, !OnLoad);

                               getrobot.transform.position = PlayerSpawnTr[randvalue].position;

                               PlayerRobots.Add(getrobot);
                           };
            }
            else
            {
                finddata.Set(unitidx);

                finddata.transform.position = PlayerSpawnTr[randvalue].position;

                ProjectUtility.SetActiveCheck(finddata.gameObject, !OnLoad);
            }
        }

    }


    public void CreateEnemyRobot(int enemyidx, bool OnLoad = false)
    {
        var td = Tables.Instance.GetTable<EnemyInfo>().GetData(enemyidx);

        if (td != null)
        {
            var randvalue = Random.Range(0, EnemySpawnTr.Count);

            var finddatta = EnemyRobots.Find(x => x.GetUnitIdx == enemyidx && x.gameObject.activeSelf == false);

            if (finddatta == null || OnLoad)
            {
                Addressables.InstantiateAsync(td.prefab, EnemySpawnTr[randvalue], false).Completed += (handle) =>
                            {
                                var getrobot = handle.Result.GetComponent<RobotBase>();

                                if (getrobot != null)
                                {
                                    getrobot.Set(enemyidx);
                                }

                                ProjectUtility.SetActiveCheck(getrobot.gameObject, !OnLoad);

                                getrobot.transform.position = EnemySpawnTr[randvalue].position;

                                EnemyRobots.Add(getrobot);
                            };
            }
            else
            {
                finddatta.Set(enemyidx);

                finddatta.transform.position = EnemySpawnTr[randvalue].position;

                ProjectUtility.SetActiveCheck(finddatta.gameObject, !OnLoad);
            }

        }
    }



    public virtual RobotBase GetTarget(UnityEngine.Vector3 pos, UnitType type, float unitradious = 999999999)
    {
        float distance = float.MaxValue;

        RobotBase target = null;

        switch (type)
        {
            case UnitType.Enemy:
                {
                    foreach (var enemyunit in EnemyRobots)
                    {
                        if (enemyunit.IsDeath) continue;

                        if (enemyunit.gameObject.activeSelf == false) continue;

                        var enemydistance = Vector3.Distance(pos, enemyunit.transform.position);

                        if (enemydistance > unitradious) continue;

                        if (distance > enemydistance)
                        {
                            distance = enemydistance;
                            target = enemyunit;
                        }
                    }
                }
                break;
            case UnitType.Player:
                {
                    float maxDistance = 0;

                    foreach (var playerrobot in PlayerRobots)
                    {
                        if (playerrobot.IsDeath) continue;

                        if (playerrobot.gameObject.activeSelf == false) continue;

                        float dsistanc1 = Vector3.Distance(pos, playerrobot.transform.position);

                        if (dsistanc1 > unitradious) continue;

                        if (dsistanc1 > maxDistance)
                        {
                            maxDistance = dsistanc1;
                            target = playerrobot;
                        }
                    }
                }
                break;
        }


        return target;
    }

}
