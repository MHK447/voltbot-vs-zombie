using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using BanpoFri;
using UniRx;
using TMPro;


public class SelectItemComponent : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI PriceText;

    private int SelectItemIdx = 0;

    [SerializeField]
    private GameObject CachedPrefab;


    [SerializeField]
    private Transform CachedRoot;



    private List<GameObject> CachedComponents = new List<GameObject>();


    public List<StoreBuyProductComponent> BuyProductComponentList = new List<StoreBuyProductComponent>();

    private CompositeDisposable disposables = new CompositeDisposable();

    public void Set(int selectitemidx)
    {
        disposables.Clear();

        SelectItemIdx = selectitemidx;

        var td = Tables.Instance.GetTable<InGameBuyProductInfo>().GetData(SelectItemIdx);

        if (td != null)
        {
            PriceText.text = td.price.ToString();
            var getobj = GetCachedObject().GetComponent<StoreBuyProductComponent>();

            if (getobj != null)
            {
                getobj.Set(SelectItemIdx, this);
                ProjectUtility.SetActiveCheck(getobj.gameObject, true);
            }


            GameRoot.Instance.UserData.Money.Subscribe(x =>
            {
                PriceText.color = x >= td.price ? Color.white : Color.red;
            }).AddTo(disposables);
        }
    }



    public GameObject GetCachedObject()
    {
        var inst = CachedComponents.Find(x => !x.activeSelf);
        if (inst == null)
        {
            inst = GameObject.Instantiate(CachedPrefab);
            inst.transform.SetParent(CachedRoot);
            inst.transform.localScale = Vector3.one;
            inst.transform.position = this.transform.position;
            CachedComponents.Add(inst);
        }

        return inst;
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
