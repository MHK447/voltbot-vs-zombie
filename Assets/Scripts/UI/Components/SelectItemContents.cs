using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField]
    private GameObject ItemGroupObj;


    private int Cost = 0;




    void Awake()
    {
        RerollButton.onClick.AddListener(OnClickReroll);
        FightButton.onClick.AddListener(OnClickFight);
    }

    public void Init()
    {
        ItemComponents[0].Set(1);
        ItemComponents[1].Set(101);
        ItemComponents[2].Set(1);
    }


    public void RandSelectItem()
    {
        for (int i = 0; i < ItemComponents.Count; i++)
        {
            var rand = Random.Range(0, ItemComponents.Count);

            ItemComponents[i].Set(rand);
        }
    }

    public void ActiveSelectOn()
    {
        ProjectUtility.SetActiveCheck(ItemGroupObj.gameObject , true);
    }


    public void OnClickReroll()
    {

    }

    public void OnClickFight()
    {
        GameRoot.Instance.InGameSystem.IsWaveStartBattle.Value = true;
        var waveidx = GameRoot.Instance.UserData.CurMode.StageData.Waveidx.Value;
        GameRoot.Instance.StartCoroutine(GameRoot.Instance.InGameSystem.GetInGame<InGameBaseStage>().curInGameBattle.GetStageMap.StartWaveBattle(waveidx));
        ProjectUtility.SetActiveCheck(ItemGroupObj.gameObject , false);
        
    }
}
