using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using BanpoFri;
using UniRx;
using TMPro;
using UnityEngine.UI;

public class StoreBuyProductComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private Image ItemImage;

    [SerializeField]
    private Vector2 Offset;

    [SerializeField]
    private Vector2 WeaponOffset;

    private RectTransform RecT;

    [HideInInspector]
    public bool IsDraggingStart = false;

    [HideInInspector]
    public VoltComponent ParentVoltComponent;

    public void Init()
    {
        RecT = ItemImage.transform as RectTransform;
    }

    public void SetParentVoltComponent(VoltComponent voltComponent)
    {
        ParentVoltComponent = voltComponent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDraggingStart = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsDraggingStart)
        {
            MoveToMousePosition(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDraggingStart = false;
    }

    // ParentVoltComponent에서 호출할 메소드
    public void HandleTouchDown(PointerEventData eventData)
    {
        IsDraggingStart = true;
        // 터치 다운 시 아이템 위치를 터치 위치로 이동
        MoveToMousePosition(eventData);
    }

    public void HandleTouchDrag(PointerEventData eventData)
    {
        if (IsDraggingStart)
        {
            MoveToMousePosition(eventData);
        }
    }

    public void HandleTouchUp(PointerEventData eventData)
    {
        IsDraggingStart = false;
    }

    private void MoveToMousePosition(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.transform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            RecT.anchoredPosition = localPoint - Offset - WeaponOffset;
            ItemImage.raycastTarget = false;
        }
    }
}
