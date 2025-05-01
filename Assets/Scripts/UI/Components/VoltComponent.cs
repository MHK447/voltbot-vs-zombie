using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using BanpoFri;
using UniRx;
using TMPro;
using UnityEngine.UI;

public class VoltComponent : MonoBehaviour
    
      // 추가됨
{
    [HideInInspector]
    public StoreBuyProductComponent ProductComponent;

    [SerializeField]
    private int Order = 0;

    public void Init()
    {

    }

    public void OnProduct(StoreBuyProductComponent productComponent)
    {
        ProductComponent = productComponent;
        ProductComponent.SetParentVoltComponent(this); // StoreBuyProductComponent에 VoltComponent 참조 설정
    }
}
