using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using BanpoFri;
using Spine.Unity;

public class Chaser : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _navMeshAgent;

    [SerializeField]
    protected SkeletonAnimation skeletonAnimation;

    List<Vector2> _wayPoints = new List<Vector2>();

    [SerializeField] private float reachRadius = 0.60f;

    Action _navMeshUpdateEvent;

    protected bool _isMoving = false;

    protected InGameStage Stage;

    float _agentDrift = 0.0001f;

    protected string CurAnimName;

    Vector3 _destinationPosition;
    protected Action _destinationReachedEvent;
    Coroutine _currentMoveProcess;

    WaitForSeconds _waitTick;

    private float CurrentMoveSpeed = 5f;

    private int UnitIdx = 0;

    public Transform TargetTr;

    [HideInInspector]
    public bool IsCarry = false;

    public virtual void Init(int idx)
    {
        UnitIdx = idx;

        var td = Tables.Instance.GetTable<ConsumerInfo>().GetData(idx);

        Stage = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;

        CurrentMoveSpeed = td.speed;

            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;

        _navMeshAgent.enabled = true;

        GameRoot.Instance.StartCoroutine(WaitOneFrame());
    }


    public IEnumerator WaitOneFrame()
    {
        yield return new WaitForEndOfFrame();


        if (this.gameObject.activeSelf)
        {
            SetWayPoints();
        }
    }


    public virtual void Kill(bool killByScene = false)
    {
        _navMeshAgent.enabled = false;

        _wayPoints.Clear();

        if (_currentMoveProcess != null)
        {
            StopCoroutine(_currentMoveProcess);
            _currentMoveProcess = null;
        }
    }

    void SetWayPoints()
    {
        if (TargetTr == null) return;
        
        Vector2 targetPosition = TargetTr.transform.position;
        _wayPoints.Clear();

        NavMeshPath navMeshPath = new NavMeshPath();
        if (_navMeshAgent.CalculatePath(targetPosition, navMeshPath))
        {
            // 우회 경로 생성
            Vector2[] wayPoints = SmoothPath(navMeshPath.corners);

            for (int i = 0; i < wayPoints.Length; i++)
            {
                _wayPoints.Add(wayPoints[i]);
            }
        }
        else
        {
            Debug.Log("(!) Fail to NavMeshAgent's CalculatePath : " + this.gameObject.name);
            this.transform.position = Stage.GetStartWayPoint.position;
        }

        _wayPoints.Add(targetPosition);
    }

    Vector2[] SmoothPath(Vector3[] pathCorners)
    {
        List<Vector2> smoothPath = new List<Vector2>();

        for (int i = 0; i < pathCorners.Length - 1; i++)
        {
            Vector2 start = pathCorners[i];
            Vector2 end = pathCorners[i + 1];

            // 현재 세그먼트의 시작 지점을 추가
            smoothPath.Add(start);

            // 현재 세그먼트의 길이를 계산
            float segmentLength = Vector2.Distance(start, end);

            // 현재 세그먼트를 충분히 세분화하여 추가
            int divisions = Mathf.CeilToInt(segmentLength / reachRadius);
            for (int j = 1; j <= divisions; j++)
            {
                float t = j / (float)divisions;
                smoothPath.Add(Vector2.Lerp(start, end, t));
            }
        }

        return smoothPath.ToArray();
    }


    public void SetDestination(Transform destination , System.Action arrivedaction)
    {
        TargetTr = destination;

        _isMoving = true;

        if (((Vector2)transform.position - (Vector2)destination.position).magnitude < 0.1f)
        {
            ReachProcess();
        }
        else
        {
            var driftPos = destination.position;
            if (Mathf.Abs(transform.position.x - destination.position.x) < _agentDrift)
            {
                driftPos = destination.position + new Vector3(_agentDrift, 0f, 0f);
            }

            _destinationPosition = driftPos;
            SetWayPoints();

            if (_currentMoveProcess == null)
            {
                _currentMoveProcess = StartCoroutine(MoveProcess(arrivedaction));
            }
        }
    }

    void ReachProcess()
    {
        _isMoving = false;
        PlayAnimation("idle", true);
    }


    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        //Navigate.DrawGizmos(_navMeshAgent, _showPath, _showAhead);

        if (_wayPoints.Count > 0)
        {
            Debug.DrawLine(this.transform.position, _wayPoints[0], Color.green);
        }
#endif
    }


    public void PlayAnimation(string newAnimationName, bool isLooping)
    {
        if (CurAnimName == newAnimationName) return;

        var animationname = newAnimationName;

        if (IsCarry)
        {
            switch (animationname)
            {
                case "idle":
                    {
                        animationname = "carryidle";
                    }
                    break;
                case "move":
                    {
                        animationname = "carry";
                    }
                    break;

            }
        }


        CurAnimName = animationname;

        if (skeletonAnimation != null)
        {
            skeletonAnimation.state.SetAnimation(0, animationname, isLooping);
        }
    }




    IEnumerator MoveProcess(System.Action arrivedaction = null)
    {
        while (_wayPoints.Count > 0)
        {
            var currentWayPoint = _wayPoints[0];
            if (((Vector2)transform.position - currentWayPoint).magnitude < reachRadius)
            {
                _wayPoints.RemoveAt(0);
            }
            else
            {
                _isMoving = true;

                var animname = IsCarry ? "carry" : "move";

                PlayAnimation(animname, true);

                transform.localScale = new Vector3(transform.position.x - currentWayPoint.x > 0 ? 1f : -1f, 1f, 1f);
                transform.position = Vector2.MoveTowards(transform.position, currentWayPoint, Time.deltaTime * CurrentMoveSpeed);
            }

            yield return _waitTick;
        }

        _currentMoveProcess = null;
        arrivedaction?.Invoke();
        ReachProcess();

        yield break;
    }


}
