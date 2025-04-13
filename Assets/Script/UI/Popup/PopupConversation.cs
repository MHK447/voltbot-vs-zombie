using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using TMPro;

[UIPath("UI/Popup/PopupConversation")]
public class PopupConversation : UIBase
{
    public enum OtterType
    {
        Happy,
        Idle,

    }

    [SerializeField]
    private TextMeshProUGUI ConversationText;

    [SerializeField]
    private Animator OtterAnim;


    public void Set(string text, OtterType type)
    {
        ConversationText.text = text;
    
        GameRoot.Instance.WaitTimeAndCallback(0.2f, () =>
        {
            OtterAnim.Play(type.ToString(), 0, 0f);
        });
    }


    public override void Hide()
    {
        base.Hide();
    }

}
