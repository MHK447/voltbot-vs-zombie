using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.AddressableAssets;

[UIPath("UI/Page/PageLobbyBattle")]
public class PageLobbyBattle : UIBase
{
    public enum LobbyTab
    {
        Shop,
        CharacterUpgrade,
        Battle,
        SkillBook,
        Eletronic,
    }

    [SerializeField]
    private List<Toggle> lobbyToggles = new List<Toggle>();

    [SerializeField]
    private Button StartBtn;

    [SerializeField]
    private TextMeshProUGUI StageText;

    [SerializeField]
    private TextMeshProUGUI Reward1Text;

    [SerializeField]
    private TextMeshProUGUI Reward2Text;

    [SerializeField]
    private List<Image> SelectUnitImgList = new List<Image>();
    public LobbyTab CurrentTab { get; private set; } = LobbyTab.Battle;


    private bool IsInit = false;


    protected override void Awake()
    {
        base.Awake();
        StartBtn.onClick.AddListener(OnClickStart);

        IsInit = false;

        int iter = 0;
        foreach (var toggle in lobbyToggles)
        {
            var tabIdx = iter;
            toggle.isOn = false;
            toggle.onValueChanged.AddListener(on =>
            {
                ChangeTab((LobbyTab)tabIdx, on);
            });
            ++iter;
        }

    }


    public void OnClickStart()
    {
        Hide();

        //GameRoot.Instance.UISystem.OpenUI<PopupIngameBottom>(popup => popup.Init());
        GameRoot.Instance.InGameSystem.DeadCount.Value = 0;
        GameRoot.Instance.InGameSystem.LevelProperty.Value = 0;

        Addressables.InstantiateAsync("StageMap_01").Completed += (obj) =>
                      {
                          var inst = obj.Result;
                      };

        GameRoot.Instance.WaitTimeAndCallback(2f, () =>
        {
            StartBtn.interactable = true;
        });
    }


    public void SelectTab(LobbyTab tab)
    {

        // var labui = GameRoot.Instance.UISystem.GetUI<PageLobbyWorkShop>();

        // if (labui != null)
        // {
        //     labui.SortingRollBack();
        // }


        // var shopui = GameRoot.Instance.UISystem.GetUI<PageLobbyShop>();

        // if (shopui != null)
        // {
        //     shopui.SortingRollBack();
        // }


        // var cardui = GameRoot.Instance.UISystem.GetUI<PageLobbyCards>();

        // if (cardui != null)
        // {
        //     cardui.SortingRollBack();
        // }


        // var lobbybattleui = GameRoot.Instance.UISystem.GetUI<PageLobbyBattle>();

        // if (lobbybattleui != null)
        // {
        //     lobbybattleui.SortingRollBack();
        // }



        //var weaponbookui = GameRoot.Instance.UISystem.GetUI<PageWeaponBook>();

        //if (weaponbookui != null)
        //{
        //    weaponbookui.SortingRollBack();
        //}


        switch (tab)
        {
            case LobbyTab.Shop:

            case LobbyTab.CharacterUpgrade:
            case LobbyTab.Battle:

            case LobbyTab.Eletronic:
                break;
            case LobbyTab.SkillBook:
                break;
        }

        foreach (var toggle in lobbyToggles)
        {
            var toggleani = toggle.gameObject.GetComponent<Animator>();
            toggleani.SetTrigger("Normal");
        }

        var ani = lobbyToggles[(int)tab].gameObject.GetComponent<Animator>();
        if (ani != null)
        {
            SoundPlayer.Instance.PlaySound("btn");
            ani.SetTrigger("Selected");
        }

    }


    public override void CustomSortingOrder()
    {
        base.CustomSortingOrder();

        transform.GetComponent<Canvas>().sortingOrder = (int)UIBase.HUDTypeTopSorting.POPUPTOP;
    }


    public void SortingRollBack()
    {
        transform.GetComponent<Canvas>().sortingOrder = UISystem.START_PAGE_SORTING_NUMBER;
    }


    IEnumerator OnShowWaitOneFrame()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        IsInit = true;
        // var labui = GameRoot.Instance.UISystem.GetUI<PageLobbyWorkShop>();

        // if (labui != null)
        // {
        //     labui.Hide();
        // }

        // var shopui = GameRoot.Instance.UISystem.GetUI<PageLobbyShop>();

        // if (shopui != null)
        // {
        //     shopui.Hide();
        // }

        // var cardui = GameRoot.Instance.UISystem.GetUI<PageLobbyCards>();

        // if (cardui != null)
        // {
        //     cardui.Hide();
        // }

        //var bookui = GameRoot.Instance.UISystem.GetUI<PageWeaponBook>();

        //if (bookui != null)
        //{
        //    bookui.Hide();
        //}

        var ani = lobbyToggles[(int)LobbyTab.Battle].gameObject.GetComponent<Animator>();
        if (ani != null)
        {
            if (IsInit)
                SoundPlayer.Instance.PlaySound("btn");

            ani.SetTrigger("Selected");
        }


    }


    public override void OnShowAfter()
    {
        base.OnShowAfter();

        StartCoroutine(OnShowWaitOneFrame());
    }

    public override void OnShowBefore()
    {
        base.OnShowBefore();
        StartCoroutine(WaitOneFrame());
    }

    IEnumerator WaitOneFrame()
    {
        yield return new WaitForEndOfFrame();

        // var viewTab = defualtOption;
        // if (!lobbyToggles[(int)defualtOption].isOn)
        // {
        //     lobbyToggles[(int)defualtOption].isOn = true;
        // }
        // else
        // {
        //     var ani = lobbyToggles[(int)defualtOption].gameObject.GetComponent<Animator>();
        //     if (ani != null)
        //     {
        //         ani.SetTrigger("Selected");
        //     }
        //     ChangeTab(defualtOption, true);
        // }


        // foreach (var lockobj in LockObjList)
        // {
        //     TpUtility.SetActiveCheck(lockobj, false);
        // }

        // bool isshopopen = GameRoot.Instance.ContentsOpenSystem.ContentsOpenCheck(ContentsOpenSystem.ContentsOpenType.Shop);

        // lobbyToggles[(int)LobbyTab.Shop].interactable = isshopopen;

        // TpUtility.SetActiveCheck(LockObjList[(int)LobbyTab.Shop], !isshopopen);

        // bool islabopen = GameRoot.Instance.ContentsOpenSystem.ContentsOpenCheck(ContentsOpenSystem.ContentsOpenType.WorkShop);

        // TpUtility.SetActiveCheck(LockObjList[(int)LobbyTab.TrainingRoom], !islabopen);

        // lobbyToggles[(int)LobbyTab.TrainingRoom].interactable = islabopen;


        // bool iscardopen = GameRoot.Instance.ContentsOpenSystem.ContentsOpenCheck(ContentsOpenSystem.ContentsOpenType.SkillCardOpen);

        // ProjectUtility.SetActiveCheck(LockObjList[(int)LobbyTab.Card], !iscardopen);

        // lobbyToggles[(int)LobbyTab.Card].interactable = iscardopen;



        //bool isweaponbook = GameRoot.Instance.ContentsOpenSystem.ContentsOpenCheck(ContentsOpenSystem.ContentsOpenType.WeaponBook);

        //TpUtility.SetActiveCheck(LockObjList[(int)LobbyTab.WeaponBook], !isweaponbook);

        //lobbyToggles[(int)LobbyTab.WeaponBook].interactable = isweaponbook;
    }


    public void ChangeTab(LobbyTab tab, bool on)
    {
        if (CurrentTab == tab) return;

        CurrentTab = tab;

        if (on)
        {
            SelectTab(tab);
        }


        foreach (var toggle in lobbyToggles)
        {
            var toggleani = toggle.gameObject.GetComponent<Animator>();

            toggleani.SetTrigger("Normal");
        }

        var ani = lobbyToggles[(int)tab].gameObject.GetComponent<Animator>();
        if (ani != null)
        {
            if (on)
            {
                if (IsInit)
                    SoundPlayer.Instance.PlaySound("btn");

                if (!lobbyToggles[(int)tab].isOn)
                    lobbyToggles[(int)tab].isOn = true;
                ani.SetTrigger("Selected");
            }
            else
                ani.SetTrigger("Normal");
        }
    }

}
