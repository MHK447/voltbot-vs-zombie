using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using TMPro;


public class RobotComponent : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI RobotTimeText;

    [SerializeField]
    private Image RobotImg;

    private int RobotIdx = 0;

    public void Set(int idx)
    {
        RobotIdx = idx;

        // var robottd = Tables.Instance.GetTable<in>().GetData(RobotIdx);

        // if(robottd != null)
        // {
        //     RobotTimeText.text = $"{robottd.product_time / 100f}s";
        //     //RobotImg.sprite = AtlasManager.Instance.GetSprite(Atlas.Atals_UI_Gacha , robottd.image);
        // }
        
    }

}
