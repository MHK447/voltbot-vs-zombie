using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;

[UIPath("UI/InGame/ConsumerOrderUI", false)]
public class ConsumerOrderUI : InGameFloatingUI
{
    public enum ConsumerState
    {
        Food = 0,
        Counter,
        Pay,
    }

    [SerializeField]
    private Image OrderImg;

    [SerializeField]
    private Text CountText;

    [SerializeField]
    private Image SliderValue;

    [SerializeField]
    private List<GameObject> ConsumerStateList = new List<GameObject>();

    private Consumer Consumer;

    private int Facilityidx = 0;

    private int MaxCount = 0; 

    public void Set(Consumer targetconsumer , int facilityidx , int count  , int maxcount)
    {
        Consumer = targetconsumer;

        Facilityidx = facilityidx;

        MaxCount = maxcount;

        CountText.text = $"{count}/{maxcount}";

        SliderValue.fillAmount = 0f;

        SetFacilityImg(facilityidx);
    }

    public void SetFacilityImg(int facilityidx)
    {
        var facilitytd = Tables.Instance.GetTable<FacilityInfo>().GetData(facilityidx);

        if (facilitytd != null)
        {
            OrderImg.sprite = Config.Instance.GetIngameImg(facilitytd.image);
        }


        if (facilityidx == 1000) //계산대
        {
            SetImage(ConsumerOrderUI.ConsumerState.Counter);
        }
        else if (facilityidx > 0) //기본 물품대 
        {
            SetImage(ConsumerOrderUI.ConsumerState.Food);
        }
    }


    public void SetImage(ConsumerState state)
    {
        foreach(var obj in ConsumerStateList)
        {
            ProjectUtility.SetActiveCheck(obj, false);
        }

        ProjectUtility.SetActiveCheck(ConsumerStateList[(int)state], true);
    }


    public void SetCountText(int count)
    {
        SliderValue.fillAmount = (float)count / (float)MaxCount;
        CountText.text = $"{count}/{MaxCount}";
    }

    
}
