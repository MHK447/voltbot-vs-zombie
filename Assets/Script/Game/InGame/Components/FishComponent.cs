using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using BanpoFri;

public class FishComponent : MonoBehaviour
{
    public enum State
    {
        None,
        Bucket,
        Rack,
        Cook,
    }

    public System.Action<bool> OnEnd = null;

    [SerializeField]
    private SpriteRenderer FishIcon;

    [SerializeField]
    private Animator Anim;

    private int FishIdx = 0;

    public int GetFishIdx { get { return FishIdx; } }

    private State CurState;

    private Transform Target;

    private bool IsTracking = false;

    private float TargetYPos = 0f;

    private int LivingType = 0;

    public void Set(int fishidx, State startstate)
    {
        this.transform.DOKill(); // 기존 Tween 제거
        IsTracking = false;      // 상태 초기화
        Target = null;
        Anim.enabled = false;

        FishIdx = fishidx;
        CurState = startstate;

        var td = Tables.Instance.GetTable<FishInfo>().GetData(FishIdx);

        if(td != null)
        {   
            FishIcon.sprite = Config.Instance.GetIngameImg(td.icon);
            LivingType = td.living_type;
        }
    }

    public void FishInBucketAction(Transform tr, System.Action<FishComponent> fishaction = null, float time = 1f, float ypos = 0f)
    {
        TargetYPos = ypos;
        IsTracking = false;
        Target = tr;    

        this.transform.DOJump(new Vector3(tr.position.x, tr.position.y + ypos, tr.position.z), 3f, 1, time)
            .SetEase(Ease.InOutQuad)
            .SetAutoKill(true)
            .OnComplete(() =>
            {
                IsTracking = true;
                this.transform.SetParent(Target);   
                this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                this.transform.localScale = new Vector3(1f, 1f, 1f);
                fishaction?.Invoke(this);
            });
    }

    public void LivingFishAnim(bool isactive)
    {
        if(LivingType == 0)
        {
            Anim.enabled = false;
            return;
        }

        Anim.enabled = isactive;
    }


    public void ClearObj()
    {
        this.transform.DOKill(); // 기존 Tween 제거
        IsTracking = false;
        Target = null;
        OnEnd?.Invoke(true);
        OnEnd = null;
    }

    private void Update()
    {
        if (Target != null && IsTracking)
        {
            var targety = Target.position.y + TargetYPos;
            this.transform.position = new Vector3(Target.position.x, targety, Target.position.z);
        }
    }
}