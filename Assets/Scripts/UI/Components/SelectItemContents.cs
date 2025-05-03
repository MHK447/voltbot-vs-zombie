using System.Collections;
using System.Collections.Generic;
using BanpoFri;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class SelectItemContents : MonoBehaviour
{
    [SerializeField]
    private List<SelectItemComponent> ItemComponents = new List<SelectItemComponent>();

    [SerializeField]
    private Button RerollButton;

    [SerializeField]
    private Button FightButton;


    void Awake()
    {
        RerollButton.onClick.AddListener(OnClickReroll);
        FightButton.onClick.AddListener(OnClickFight); 
    }

    public void Init()
    {
        
        foreach (var item in ItemComponents)
        {
            item.Set(1);
        }
    }

    public void ActiveSelectOn()
    {
        
    }


    public void OnClickReroll()
    {
        
    }

    public void OnClickFight()
    {
        var waveidx = GameRoot.Instance.UserData.CurMode.StageData.Waveidx.Value;
        GameRoot.Instance.StartCoroutine(GameRoot.Instance.InGameSystem.GetInGame<InGameBaseStage>().curInGameBattle.GetStageMap.StartWaveBattle(waveidx));
        ProjectUtility.SetActiveCheck(this.gameObject , false);
    }
}
