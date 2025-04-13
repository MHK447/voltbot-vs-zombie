using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ShopSystem
{
    public ReactiveProperty<bool> IsVipProperty = new ReactiveProperty<bool>(false);

    private float curdeltatime = 0f;

    private float InterAdTime = 300f; // 기본값 4분
    private float currentInterAdTimer = 0f;
    private bool isInterAdReady = false;

    public void Create()
    {
        // 타이머 초기화
        currentInterAdTimer = 0f;

        // VIP 상태에 따라 광고 표시 여부 설정
        IsVipProperty.Subscribe(isVip =>
        {
            // VIP 사용자는 광고 표시 안함
            isInterAdReady = !isVip;
        });
    }

    public void UpdateOneTimeSecond()
    {
        // VIP가 아닐 때만 광고 타이머 증가
        if (isInterAdReady)
        {
            currentInterAdTimer += 1f;

            // 설정된 시간에 도달하면 광고 표시 준비
            if (currentInterAdTimer >= InterAdTime)
            {
                TryShowInterstitialAd();
            }
        }
    }

    // 인터스티셜 광고 표시 시도
    public void TryShowInterstitialAd()
    {
        // 튜토리얼 중이거나 UI가 활성화된 상태일 때는 광고 표시 안함
        if (GameRoot.Instance.TutorialSystem.IsActive()) return;
        if (!GameRoot.Instance.ContentsOpenSystem.ContentsOpenCheck(ContentsOpenSystem.ContentsOpenType.Interstitial)) return;

        currentInterAdTimer = 0f;
        // 광고 표시 및 타이머 초기화
        GameRoot.Instance.GetAdManager.ShowInterstitialAd(() =>
        {
            // 광고가 끝나면 타이머 초기화
            Debug.Log("인터스티셜 광고가 표시되었습니다.");
        });
    }

    // 광고 표시 시간 설정 (초 단위)
    public void SetInterAdTime(float seconds)
    {
        InterAdTime = seconds;
    }

    // 현재 타이머 리셋
    public void ResetInterAdTimer()
    {
        currentInterAdTimer = 0f;
    }

    // 광고 표시 강제 활성화/비활성화
    public void SetInterAdEnabled(bool enabled)
    {
        isInterAdReady = enabled;

        // 비활성화 시 타이머도 리셋
        if (!enabled)
        {
            currentInterAdTimer = 0f;
        }
    }
}
