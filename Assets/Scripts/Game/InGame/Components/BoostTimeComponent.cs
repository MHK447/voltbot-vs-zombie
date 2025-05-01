using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class BoostTimeComponent : MonoBehaviour
{
    [SerializeField]
    private Button BoostBtn;

    [SerializeField]
    private TextMeshProUGUI BoostTimeText;

    [SerializeField]
    private GameObject AdObj;


    void Awake()
    {
        BoostBtn.onClick.AddListener(OnClickBoost);

        GameRoot.Instance.UserData.CurMode.BoostTime.Subscribe(SetTimeText).AddTo(this);
        GameRoot.Instance.BoostSystem.IsBoostOnProperty.Subscribe(IsBoostCheck).AddTo(this);
    }


    public void IsBoostCheck(bool isboost)
    {
        if (!isboost)
        {
            BoostTimeText.text = ProjectUtility.GetTimeStringFormattingShort(GameRoot.Instance.BoostSystem.boost_time);
        }

        ProjectUtility.SetActiveCheck(AdObj, !isboost);
    }

    public void SetTimeText(int time)
    {
        BoostTimeText.text = ProjectUtility.GetTimeStringFormattingShort(time);
    }


    public void OnClickBoost()
    {
        if (!GameRoot.Instance.BoostSystem.IsBoostOnProperty.Value)
        {
            GameRoot.Instance.GetAdManager.ShowRewardedAd(() =>
            {
                GameRoot.Instance.BoostSystem.AddBoosTime();
            });
        }

    }
}
