using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaviRegister : MonoBehaviour
{
    [SerializeField]
    private NaviSystem.NaviType NaviType;

    void Awake()
    {
        GameRoot.Instance.NaviSystem.NaviArrowList.Add(NaviType,this.gameObject);
        ProjectUtility.SetActiveCheck(this.gameObject , false);
    }
}
