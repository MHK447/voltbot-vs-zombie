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
    private SelectItemComponent SelectItemComponent;

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
    public VoltComponent EquipVoltComponent;

    // 캔버스에 달려 있는 GraphicRaycaster가 필요
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;

    private bool Isbatch = false;

    public bool GetBatch { get { return Isbatch; } }

    public void Set(SelectItemComponent itemcomponent)
    {

        if (graphicRaycaster == null)
            graphicRaycaster = GetComponentInParent<GraphicRaycaster>();

        if (eventSystem == null)
            eventSystem = GameRoot.Instance.GetComponentInChildren<EventSystem>(true); // true: 비활성화 포함


        SelectItemComponent = itemcomponent;

        Isbatch = false;

        RecT = ItemImage.transform as RectTransform;
    }

    public void SetParentVoltComponent(VoltComponent voltComponent)
    {
        EquipVoltComponent = voltComponent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDraggingStart = true;

        this.transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsDraggingStart)
        {
            MoveToMousePosition(eventData);
            GameRoot.Instance.VoltSystem.SelectCurBuyProdct = this;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDraggingStart = false;
        ItemImage.raycastTarget = true;

        if (EquipVoltComponent != null)
        {
            EquipVoltComponent.ProductComponent = null;    // 드래그 종료 지점에서 VoltComponent 탐색
        }

        VoltComponent hitVolt = TryGetVoltComponentFromUI(eventData.position);

        StoreBuyProductComponent buyproductcomponent = TryGetBuyComponentFromUI(eventData.position);

        if (buyproductcomponent != null)
        {
            if (EquipVoltComponent == null)
            {
                this.transform.position = SelectItemComponent.transform.position;
                return;
            }
            else
            {
                SwapProductComponent(buyproductcomponent);
            }

            return;
        }

        if (hitVolt != null)
        {
            EquipVoltComponent = hitVolt;
            EquipVoltComponent.ProductComponent = this;
            Isbatch = true;
        }

        if (EquipVoltComponent != null && GameRoot.Instance.VoltSystem.CurVoltComponent != null)
        {
            EquipVoltComponent = GameRoot.Instance.VoltSystem.CurVoltComponent;
            EquipVoltComponent.ProductComponent = this;

            this.transform.position = EquipVoltComponent.transform.position;
        }
        else if (GameRoot.Instance.VoltSystem.CurVoltComponent == null && EquipVoltComponent != null)
        {
            this.transform.position = EquipVoltComponent.transform.position;
        }
        else
        {
            this.transform.position = SelectItemComponent.transform.position;
        }
    }


    public void SwapProductComponent(StoreBuyProductComponent swapproduct)
    {
        if (swapproduct == null || swapproduct == this)
            return;

        // 현재 제품과 스왑할 제품의 VoltComponent 임시 저장
        VoltComponent myVolt = this.EquipVoltComponent;
        VoltComponent swapVolt = swapproduct.EquipVoltComponent;

        // 위치 정보 임시 저장
        Vector3 myPos = this.EquipVoltComponent.transform.position;
        Vector3 swapPos = swapproduct.EquipVoltComponent.transform.position;

        // 제품 위치 스왑
        this.transform.position = swapPos;
        swapproduct.transform.position = myPos;

        // VoltComponent 연결 스왑
        // 1. 기존 VoltComponent에서 제품 참조 제거
        if (myVolt != null)
            myVolt.ProductComponent = null;

        if (swapVolt != null)
            swapVolt.ProductComponent = null;

        // 2. 제품의 VoltComponent 스왑
        this.EquipVoltComponent = swapVolt;
        swapproduct.EquipVoltComponent = myVolt;

        // 3. 새 VoltComponent에 제품 참조 설정
        if (this.EquipVoltComponent != null)
            this.EquipVoltComponent.ProductComponent = this;

        if (swapproduct.EquipVoltComponent != null)
            swapproduct.EquipVoltComponent.ProductComponent = swapproduct;

        Debug.Log($"제품 스왑 완료: {this.name} <-> {swapproduct.name}");
    }

    private VoltComponent TryGetVoltComponentFromUI(Vector2 screenPosition)
    {
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        foreach (var result in results)
        {
            VoltComponent volt = result.gameObject.GetComponent<VoltComponent>();
            if (volt != null)
                return volt;
        }

        return null;
    }
    private StoreBuyProductComponent TryGetBuyComponentFromUI(Vector2 screenPosition)
    {
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        foreach (var result in results)
        {

            StoreBuyProductComponent storebuycomponent = result.gameObject.GetComponent<StoreBuyProductComponent>();
            if (storebuycomponent != null && this != storebuycomponent)
                return storebuycomponent;
        }

        return null;
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
        GameRoot.Instance.VoltSystem.SelectCurBuyProdct = null;
        GameRoot.Instance.VoltSystem.CurVoltComponent = null;
    }

    private void MoveToMousePosition(PointerEventData eventData)
    {
        if (IsDraggingStart)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.transform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
            {
                RecT.anchoredPosition = localPoint - Offset - WeaponOffset;
                ItemImage.raycastTarget = false;
            }
        }
    }
}
