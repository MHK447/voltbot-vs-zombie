using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using TMPro;

[UIPath("UI/Popup/PopupShop")]
public class PopupShop : UIBase
{
    [System.Serializable]
    public class ShopItem
    {
        public string productId;
        public Button purchaseButton;
        public TextMeshProUGUI priceText;
        public TextMeshProUGUI rewardText;
        public int rewardAmount;
    }

    [SerializeField]
    private List<ShopItem> shopItems = new List<ShopItem>();

    [SerializeField]
    private Button closeButton;

    [SerializeField]
    private Button restorePurchaseButton;  // iOS용 구매 복원 버튼

    private void Awake()
    {
        closeButton.onClick.AddListener(() => Hide());

        if (restorePurchaseButton != null)
        {
            restorePurchaseButton.onClick.AddListener(OnRestorePurchase);
            
            // 안드로이드에서는 복원 버튼 숨기기
            restorePurchaseButton.gameObject.SetActive(Application.platform == RuntimePlatform.IPhonePlayer || 
                                                      Application.platform == RuntimePlatform.OSXPlayer);
        }

        // 각 상품 버튼에 이벤트 리스너 연결
        foreach (var item in shopItems)
        {
            if (item.purchaseButton != null)
            {
                string productId = item.productId; // 클로저에서 사용하기 위한 로컬 변수
                item.purchaseButton.onClick.AddListener(() => OnPurchaseItem(productId));
            }
        }
    }

    public override void Show()
    {
        base.Show();
        UpdatePrices();
    }

    // 상품 가격 업데이트
    private void UpdatePrices()
    {
        var purchaseManager = GameRoot.Instance.GetInAppPurchaseManager;
        if (purchaseManager == null || !purchaseManager.IsInitialized)
        {
            Debug.LogWarning("인앱 결제 시스템이 초기화되지 않았습니다.");
            return;
        }

        foreach (var item in shopItems)
        {
            if (!string.IsNullOrEmpty(item.productId) && item.priceText != null)
            {
                string price = purchaseManager.GetLocalizedPrice(item.productId);
                item.priceText.text = price;
                
                // 비소모품이 이미 구매된 경우 버튼 비활성화
                if (item.productId == InAppPurchaseManager.ProductIDs.REMOVE_ADS)
                {
                    bool isPurchased = purchaseManager.IsProductPurchased(item.productId);
                    if (isPurchased)
                    {
                        item.purchaseButton.interactable = false;
                        item.priceText.text = "구매완료";
                    }
                }
            }
        }
    }

    // 상품 구매 요청
    private void OnPurchaseItem(string productId)
    {
        var purchaseManager = GameRoot.Instance.GetInAppPurchaseManager;
        if (purchaseManager == null || !purchaseManager.IsInitialized)
        {
            GameRoot.Instance.UISystem.OpenUI<PopupToastmessage>(popup => {
                popup.Show("구매 오류", "인앱 결제 시스템을 초기화할 수 없습니다.");
            });
            return;
        }

        // 로딩 표시
        GameRoot.Instance.Loading.Show(true);

        // 구매 요청
        purchaseManager.PurchaseProduct(productId, (result, message) => {
            // 로딩 숨기기
            GameRoot.Instance.Loading.Hide(true);

            if (result == InAppPurchaseManager.Result.Success)
            {
                // 구매 성공 처리
                GameRoot.Instance.UISystem.OpenUI<PopupToastmessage>(popup => {
                    popup.Show("구매 완료", "상품 구매가 완료되었습니다.");
                });
                
                // 가격 갱신
                UpdatePrices();
            }
            else if (result == InAppPurchaseManager.Result.Failure)
            {
                // 구매 실패 처리
                GameRoot.Instance.UISystem.OpenUI<PopupToastmessage>(popup => {
                    popup.Show("구매 실패", message);
                });
            }
        });
    }

    // 구매 복원 (iOS용)
    private void OnRestorePurchase()
    {
        var purchaseManager = GameRoot.Instance.GetInAppPurchaseManager;
        if (purchaseManager == null || !purchaseManager.IsInitialized)
        {
            GameRoot.Instance.UISystem.OpenUI<PopupToastmessage>(popup => {
                popup.Show("오류", "인앱 결제 시스템을 초기화할 수 없습니다.");
            });
            return;
        }

        // 로딩 표시
        GameRoot.Instance.Loading.Show(true);

        // 구매 복원 요청
        purchaseManager.RestorePurchases((result) => {
            // 로딩 숨기기
            GameRoot.Instance.Loading.Hide(true);

            if (result == InAppPurchaseManager.Result.Success)
            {
                // 복원 성공 처리
                GameRoot.Instance.UISystem.OpenUI<PopupToastmessage>(popup => {
                    popup.Show("복원 완료", "구매 내역 복원이 완료되었습니다.");
                });
                
                // 가격 갱신
                UpdatePrices();
            }
            else
            {
                // 복원 실패 처리
                GameRoot.Instance.UISystem.OpenUI<PopupToastmessage>(popup => {
                    popup.Show("복원 실패", "구매 내역을 복원할 수 없습니다.");
                });
            }
        });
    }
} 