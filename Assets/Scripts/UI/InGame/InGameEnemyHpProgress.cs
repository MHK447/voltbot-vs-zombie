using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;

[UIPath("UI/InGame/InGameEnemyHpProgress")]
[FloatingDepth((int)Config.FloatingUIDepth.HpProgress)]
public class InGameEnemyHpProgress : InGameFloatingUI
{
    [SerializeField]
    private Slider HpSlider;

    [SerializeField]
    private Slider DelayHealthBar;

    public float updatespeed = 1f;


    private double CurHp;

    private double MaxHp;

    private Coroutine Col;

    private void Start()
    {
        updatespeed = 1f;
        HpSlider.value = DelayHealthBar.value = 1f;
    }

    public void SetHpText(double curhp, double maxhp)
    {
        CurHp = curhp;

        MaxHp = maxhp;

        var curhpvalue = (float)curhp / (float)maxhp;

        HpSlider.value = (float)curhp / (float)maxhp;


        if (this.gameObject.activeSelf)
        {
            if (Col != null)
                GameRoot.Instance.StopCoroutine(Col);

            Col = GameRoot.Instance.StartCoroutine(UpdateDelayedHealthBar(curhpvalue));
        }
    }

    private void OnDisable()
    {
        if (Col != null)
            GameRoot.Instance.StopCoroutine(Col);
    }


    private IEnumerator UpdateDelayedHealthBar(double hp)
    {
        float preChangePct = DelayHealthBar.value;
        float elapsed = 0f;

        while (elapsed < updatespeed)
        {
            elapsed += Time.deltaTime;
            DelayHealthBar.value = Mathf.Lerp(preChangePct, (float)hp, elapsed / updatespeed);
            yield return null;
        }

        DelayHealthBar.value = (float)hp;
    }

}
