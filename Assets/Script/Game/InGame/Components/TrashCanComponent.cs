using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;

public class TrashCanComponent : MonoBehaviour
{

    private List<OtterBase> OtterList = new List<OtterBase>();
    private Dictionary<FishComponent, float> ProcessingFish = new Dictionary<FishComponent, float>();

    private float TrashTime = 0f;
    private float TrashCanTime = 0.01f;
    private float DestroyTimeout = 1.5f; // 물고기 삭제 타임아웃 시간

    [SerializeField]
    private Transform FishTr;

    [SerializeField]
    private Transform ConsumerTr;

    public Transform GetConsumerTr { get { return ConsumerTr; } }


    public void Init()
    {
        TrashCanTime = 0.01f;
        ProcessingFish.Clear();

        GameRoot.Instance.UISystem.LoadFloatingUI<UI_TrashCanBubble>((_progress) =>
        {
            _progress.Init(this.transform);
        });
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 오브젝트의 레이어를 확인합니다.
        if ((collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("CarryCasher")))
        {
            var getvalue = collision.gameObject.GetComponent<OtterBase>();

            if (getvalue != null)
            {
                if (!OtterList.Contains(getvalue))
                {
                    OtterList.Add(getvalue);
                }
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("CarryCasher"))
        {
            var getvalue = collision.gameObject.GetComponent<OtterBase>();

            if (getvalue != null)
            {
                if (OtterList.Contains(getvalue))
                {
                    OtterList.Remove(getvalue);
                }
            }
        }
    }


    public void Update()
    {
        // 물고기 처리 상태 체크 및 타임아웃 처리
        CheckProcessingFish();

        if (OtterList.Count == 0) return;

        for (int i = OtterList.Count - 1; i >= 0; i--)
        {
            if (OtterList[i] == null) continue; // Null 체크 추가
            
            TrashTime += Time.deltaTime;

            if (TrashCanTime <= TrashTime)
            {
                TrashTime = 0f;

                // 각 Otter의 모든 물고기를 삭제
                var fishList = OtterList[i].GetFishComponentList.ToList(); // 현재 Otter의 모든 물고기 리스트를 복사

                if (fishList.Count == 0) continue;

                // 역순으로 반복
                for (int j = fishList.Count - 1; j >= 0; j--)
                {
                    var fish = fishList[j];
                    
                    if (fish == null) continue; // null 체크 추가
                    
                    // 즉시 리스트에서 제거 (OtterBase에서 물고기 제거)
                    OtterList[i].RemoveFish(fish);
                    
                    // 물고기 처리 목록에 추가
                    if (!ProcessingFish.ContainsKey(fish))
                    {
                        ProcessingFish.Add(fish, Time.time);
                        
                        // 물고기를 처리하고 삭제
                        fish.FishInBucketAction(FishTr, (fishComp) =>
                        {
                            if (fishComp != null) // null 체크
                            {
                                fishComp.transform.SetParent(this.transform);
                                DestroyFish(fishComp);
                            }
                        }, 0.2f);
                    }
                }

                // 물고기를 모두 제거한 후, Otter가 비어 있으면 CarryEnd() 호출
                if (OtterList[i].GetFishComponentList.Count == 0)
                {
                    OtterList[i].CarryEnd();
                }
                
                // 안전하게 모든 물고기 제거 보장 - 만약 Otter의 물고기가 아직 남아 있다면, 모두 강제 제거
                if (OtterList[i].GetFishComponentList.Count > 0)
                {
                    ForceDestroyOtterFish(OtterList[i]);
                }
            }
        }
    }
    
    // 물고기를 확실히 삭제하는 메서드
    private void DestroyFish(FishComponent fish)
    {
        try
        {
            if (fish != null)
            {
                Destroy(fish.gameObject);
                Debug.Log("물고기 삭제됨: " + fish.name);
            }
            
            if (ProcessingFish.ContainsKey(fish))
            {
                ProcessingFish.Remove(fish);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("물고기 삭제 중 오류: " + e.Message);
        }
    }
    
    // 특정 Otter의 모든 물고기 강제 삭제
    private void ForceDestroyOtterFish(OtterBase otter)
    {
        if (otter == null) return;
        
        var fishList = otter.GetFishComponentList.ToList();
        foreach (var fish in fishList)
        {
            if (fish != null)
            {
                otter.RemoveFish(fish);
                DestroyFish(fish);
            }
        }
        
        // 혹시 남아있는 물고기가 있으면 다시 한번 확인
        if (otter.GetFishComponentList.Count > 0)
        {
            otter.GetFishComponentList.Clear();
        }
        
        otter.CarryEnd();
    }
    
    // 처리 중인 물고기 상태 체크
    private void CheckProcessingFish()
    {
        // 오래된 물고기 삭제 처리 (타임아웃 시)
        List<FishComponent> toRemove = new List<FishComponent>();
        
        foreach (var pair in ProcessingFish)
        {
            if (Time.time - pair.Value > DestroyTimeout)
            {
                if (pair.Key != null)
                {
                    Debug.LogWarning("물고기 삭제 타임아웃으로 강제 삭제: " + pair.Key.name);
                    DestroyFish(pair.Key);
                }
                toRemove.Add(pair.Key);
            }
        }
        
        // 타임아웃된 물고기 목록에서 제거
        foreach (var fish in toRemove)
        {
            if (ProcessingFish.ContainsKey(fish))
            {
                ProcessingFish.Remove(fish);
            }
        }
        
        // 매 업데이트마다 잠재적인 버그로 인해 발생할 수 있는 null 물고기 청소
        List<FishComponent> nullFish = new List<FishComponent>();
        foreach (var pair in ProcessingFish)
        {
            if (pair.Key == null)
            {
                nullFish.Add(pair.Key);
            }
        }
        
        foreach (var fish in nullFish)
        {
            ProcessingFish.Remove(fish);
        }
    }
    
    // 강제로 모든 물고기 삭제 (긴급 상황용)
    public void ForceDestroyAllFish()
    {
        foreach (var otter in OtterList)
        {
            ForceDestroyOtterFish(otter);
        }
        
        // 처리 중인 물고기도 모두 삭제
        List<FishComponent> allFish = new List<FishComponent>(ProcessingFish.Keys);
        foreach (var fish in allFish)
        {
            if (fish != null)
            {
                DestroyFish(fish);
            }
        }
        
        ProcessingFish.Clear();
    }
}
