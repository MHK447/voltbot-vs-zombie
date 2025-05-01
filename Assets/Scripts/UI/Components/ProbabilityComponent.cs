using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;

public class ProbabilityComponent : MonoBehaviour
{
    [SerializeField]
    private List<Text> PercentTextList = new List<Text>();



    public void Init(int level)
    {
        var td = Tables.Instance.GetTable<UnitGradeInfo>().GetData(level);

        if(td != null)
        {
            for(int i = 0; i < td.gradepercent.Count; ++i)
            {
                PercentTextList[i].text = $"{td.gradepercent[i]}%";
            }
        }
    }
}
