using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;


public class LegendCombineComponent : MonoBehaviour
{
    [SerializeField]
    private Image UnitImg;

    [SerializeField]
    private Button UnitBtn;

    private int UnitIdx = 0;

    private System.Action<int> UnitSelectAction;


    private void Awake()
    {
        UnitBtn.onClick.AddListener(OnClickUnit);
    }


    public void Set(int unitidx, System.Action<int> unitaction)
    {
        UnitIdx = unitidx;
        UnitSelectAction = unitaction;

    }

    public void OnClickUnit()
    {
        UnitSelectAction?.Invoke(UnitIdx);
    }


}
