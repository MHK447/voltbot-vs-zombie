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
    private Image ItemImage;

    [SerializeField]
    private TextMeshProUGUI PriceText;

    private int SelectItemIdx = 0;

    [SerializeField]
    private GameObject CachedPrefab;


    [SerializeField]
    private Transform CachedRoot;


    private List<GameObject> CachedComponents = new List<GameObject>();


    public List<StoreBuyProductComponent> BuyProductComponentList = new List<StoreBuyProductComponent>();

    public void Set(int selectitemidx)
    {
        SelectItemIdx = selectitemidx;

        GetCachedObject().GetComponent<StoreBuyProductComponent>().Set(this);
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


}
