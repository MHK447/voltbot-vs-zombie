using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BanpoFri;
using UnityEngine;

public class UpgradeProductComponentGroup : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> CachedComponents = new List<GameObject>();

    [SerializeField]
    private GameObject CachedPrefab;

    [SerializeField]
    private Transform CachedRoot;

    public void Init()
    {
        foreach (var cachedobj in CachedComponents)
        {
            ProjectUtility.SetActiveCheck(cachedobj, false);
        }

        var curstageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

        var tdlist = Tables.Instance.GetTable<FacilityUpgrade>().DataList.ToList().FindAll(x => x.stageidx == curstageidx);

        foreach (var td in tdlist)
        {
            var finddata = GameRoot.Instance.UserData.CurMode.StageData.FindFacilityData(td.openfacilitycheck);

            if (finddata != null && finddata.IsOpen)
            {
                var getobj = GetCachedObject().GetComponent<UpgradeFacilityComponent>();

                if (getobj != null)
                {
                    ProjectUtility.SetActiveCheck(getobj.gameObject, true);
                    getobj.Set(td.facilityidx);
                }
            }
        }

    }

    public GameObject GetCachedObject()
    {
        var inst = CachedComponents.Find(x => !x.activeSelf);
        if (inst == null)
        {
            inst = GameObject.Instantiate(CachedPrefab);
            inst.transform.SetParent(CachedRoot);
            inst.transform.localScale = Vector3.one;
            CachedComponents.Add(inst);
        }

        return inst;
    }
}
