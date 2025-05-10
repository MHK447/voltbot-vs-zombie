using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using BanpoFri;
using UniRx;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using System.Net.NetworkInformation;
using System;

public class StoreBuyProductComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private SelectItemComponent SelectItemComponent;

    [SerializeField]
    private Image ProductOnImg;

    [SerializeField]
    private Image ProductOffImg;

    [SerializeField]
    private TextMeshProUGUI OveLevelText;

    [SerializeField]
    private TextMeshProUGUI CoolTimeText;


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


    private int SelectItemIdx = 0;

    private int Cost = 0;

    private int ProdcutType = 0;



    //equipdata 

    private float CoolTime = 0f;

    private float NextCreateProductTime = 0f;

    private CompositeDisposable disposables = new CompositeDisposable();




    public void Set(int idx, SelectItemComponent itemcomponent)
    {
        SelectItemIdx = idx;

        var td = Tables.Instance.GetTable<InGameBuyProductInfo>().GetData(SelectItemIdx);

        if (td != null)
        {
            ProdcutType = td.type;
            Cost = td.price;

            ProjectUtility.SetActiveCheck(OveLevelText.gameObject, false);

            ProjectUtility.SetActiveCheck(CoolTimeText.gameObject, false);

            ProjectUtility.SetActiveCheck(ProductOffImg.gameObject, false);

            switch (ProdcutType)
            {
                case (int)Config.ProdcutType.ElectCore:
                    {
                        OveLevelText.text = td.product_idx.ToString();
                        ProductOnImg.sprite = ProductOffImg.sprite = AtlasManager.Instance.GetSprite(Atlas.Atlas_Common, td.image);
                        ProjectUtility.SetActiveCheck(OveLevelText.gameObject, true);
                    }
                    break;
                case (int)Config.ProdcutType.Robot:
                    {
                        ProductOnImg.sprite = ProductOffImg.sprite = AtlasManager.Instance.GetSprite(Atlas.Atlas_Robot, td.image);
                        ProjectUtility.SetActiveCheck(CoolTimeText.gameObject, true);
                    }
                    break;
                case (int)Config.ProdcutType.SkillBook:
                    break;
            }

            ProjectUtility.SetActiveCheck(CoolTimeText.gameObject, false);

            if (td != null)
            {
                CoolTime = td.cooltime / 100f;
                CoolTimeText.text = $"{CoolTime.ToString()}s";
            }
        }


        if (graphicRaycaster == null)
            graphicRaycaster = GetComponentInParent<GraphicRaycaster>();

        if (eventSystem == null)
            eventSystem = GameRoot.Instance.GetComponentInChildren<EventSystem>(true); // true: 비활성화 포함


        SelectItemComponent = itemcomponent;

        Isbatch = false;

        RecT = this.transform as RectTransform;

        disposables.Clear();

        GameRoot.Instance.InGameSystem.IsWaveStartBattle.SkipLatestValueOnSubscribe().Subscribe(WaveStatus).AddTo(disposables);
    }

    public void SetParentVoltComponent(VoltComponent voltComponent)
    {
        EquipVoltComponent = voltComponent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(GameRoot.Instance.InGameSystem.IsWaveStartBattle.Value) return;

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
        ProductOnImg.raycastTarget = ProductOffImg.raycastTarget = true;

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

        if (hitVolt != null && GameRoot.Instance.UserData.Money.Value >= Cost && !Isbatch)
        {
            GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, -Cost);
            EquipVoltComponent = hitVolt;
            EquipVoltComponent.ProductComponent = this;
            Isbatch = true;

            ProjectUtility.SetActiveCheck(CoolTimeText.gameObject, true);
        }
        else if (hitVolt != null && Isbatch)
        {
            EquipVoltComponent = hitVolt;
            EquipVoltComponent.ProductComponent = this;
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

    public void WaveStatus(bool iswavestart)
    {
        ProjectUtility.SetActiveCheck(this.gameObject, Isbatch);

        if (iswavestart)
        {
            ProductOffImg.fillAmount = 1f;
        }

        ProjectUtility.SetActiveCheck(ProductOffImg.gameObject, iswavestart && ProdcutType == (int)Config.ProdcutType.Robot);
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
                ProductOnImg.raycastTarget = ProductOffImg.raycastTarget = false;
            }
        }
    }


    void Update()
    {
        // 전투 중이면 실행하지 않음
        if (!GameRoot.Instance.InGameSystem.IsWaveStartBattle.Value) return;

        if (ProdcutType != (int)Config.ProdcutType.Robot) return;

        // 현재 남은 쿨타임 계산
        float remainingTime = (float)(NextCreateProductTime - Time.time);
        float totalCoolTime = (float)CoolTime; // 전체 쿨타임 (초 단위)

        // UI 표시 (1.0 -> 0.0으로 감소)
        float fillAmount = Mathf.Clamp01(remainingTime / totalCoolTime);
        ProductOffImg.fillAmount = fillAmount;

        // 쿨타임이 끝났으면 새 프로덕트 생성
        if (Time.time >= NextCreateProductTime)
        {
            CreateProduct();
            NextCreateProductTime = Time.time + totalCoolTime; // 다음 생성 시간 설정
        }
    }



    public void CreateProduct()
    {
        // 현재 게임 오브젝트 또는 특정 자식 오브젝트 (애니메이션을 적용할 대상)
        Transform targetTransform = this.transform; // 또는 원하는 자식 오브젝트 Transform

        // 원래 스케일 저장
        Vector3 originalScale = targetTransform.localScale;

        // 애니메이션 시퀀스 생성
        Sequence scaleSequence = DOTween.Sequence();

        // 1. 빠르게 커지는 애니메이션 (0.15초 동안 1.3배로 커짐)
        scaleSequence.Append(targetTransform.DOScale(originalScale * 1.3f, 0.15f).SetEase(Ease.OutQuad));

        // 2. 다시 원래 크기로 돌아오는 애니메이션 (0.2초 동안)
        scaleSequence.Append(targetTransform.DOScale(originalScale, 0.2f).SetEase(Ease.InOutQuad));

        // 애니메이션 시작
        scaleSequence.Play();

        // 여기에 프로덕트 생성 관련 기존 로직 추가
        // ...

        GameRoot.Instance.InGameSystem.GetInGame<InGameBaseStage>().curInGameBattle.GetStageMap.CreatePlayerRobot(SelectItemIdx);
    }

    void OnDestroy()
    {
        disposables.Clear();
    }


    void OnDisable()
    {
        disposables.Clear();
    }



}
