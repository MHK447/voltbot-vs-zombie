using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;

[UIPath("UI/InGame/UI_AmountBubble", false)]
public class UI_AmountBubble : InGameFloatingUI
{
    [SerializeField]
    private Image FacilityIconImg;

    [SerializeField]
    private Text AmountCountText;

    [SerializeField]
    private Image SliderValue;

        public void Set(int fishidx)
    {
        var td = Tables.Instance.GetTable<FishInfo>().GetData(fishidx);

        if (td != null)
        {
            var facilitydata = GameRoot.Instance.UserData.CurMode.StageData.FindFacilityData(td.fish_facility_idx);

            var facilitytd = Tables.Instance.GetTable<FacilityInfo>().GetData(td.fish_facility_idx);

            if (facilitytd != null)
            {
                int capacitycount = facilitydata == null ? 0 : facilitydata.CapacityCountProperty.Value;

                FacilityIconImg.sprite = Config.Instance.GetIngameImg(td.icon);
                AmountCountText.text = $"{capacitycount}/{facilitytd.start_capacity}";
            }
        }
    }


    public void SetValue(int count, int curmaxcapacity)
    {
        AmountCountText.text = $"{count}/{curmaxcapacity}";
        //ProjectUtility.SetActiveCheck(this.gameObject, count > 0);
        SliderValue.fillAmount = (float)count / (float)curmaxcapacity;
    }

}
