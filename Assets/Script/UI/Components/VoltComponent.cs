using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using BanpoFri;
using UniRx;
using TMPro;
using UnityEngine.UI;

public class VoltComponent : MonoBehaviour, 
    IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler,
    IPointerEnterHandler, IPointerExitHandler // 추가됨
{
    [HideInInspector]
    private StoreBuyProductComponent ProductComponent;

    [SerializeField]
    private int Order = 0;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("VoltComponent OnPointerDown: " + gameObject.name);
        
        if (ProductComponent != null)
        {
            ProductComponent.HandleTouchDown(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("VoltComponent OnPointerUp: " + gameObject.name);
        
        if (ProductComponent != null)
        {
            ProductComponent.HandleTouchUp(eventData);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("VoltComponent OnPointerClick: " + gameObject.name);
        // 클릭 시 특정 동작 수행
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("VoltComponent OnDrag: " + gameObject.name);
        
        if (ProductComponent != null)
        {
            ProductComponent.HandleTouchDrag(eventData);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("VoltComponent OnPointerEnter (Hover Enter): " + gameObject.name);
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("VoltComponent OnPointerExit (Hover Exit): " + gameObject.name);
    }

    public void Init()
    {

    }

    public void OnProduct(StoreBuyProductComponent productComponent)
    {
        ProductComponent = productComponent;
        ProductComponent.transform.SetParent(transform);
        ProductComponent.transform.localPosition = Vector3.zero; // localPosition 사용
        ProductComponent.SetParentVoltComponent(this); // StoreBuyProductComponent에 VoltComponent 참조 설정
        ProductComponent.Init();
    }

    public void OffProduct()
    {
        ProductComponent = null;
    }
}
