using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BanpoFri;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] Image _joystickBack;
    [SerializeField] Image _joystickHead;

    RectTransform _backRectTransform;
    RectTransform _headRectTransform;

    InputHandler _inputHandler;

    //줌
    float zoomSpeed = 0.01f;
    float _defaultCamera = 10;
    float _minCamera;
    float _maxCamera;

    //조이스틱 전체 반지름, 소수화
    float _radius;
    float _radiusDecimal;

    //이동속도 보정 수치 (for default = 1)
    float _speedCorrection = 50f;

    bool _isTouch;
    bool _isDrag;
    Vector3 _vectorMove;

    OtterBase _player;

    float targetZoom;
    float zoomVelocity;

    public bool IsLock  = false;

    private void Start()
    {
        targetZoom = Camera.main.orthographicSize;
        _backRectTransform = _joystickBack.rectTransform;
        _headRectTransform = _joystickHead.rectTransform;

        _radius = _backRectTransform.rect.width * 0.5f;
        _radiusDecimal = 1 / (_radius * _speedCorrection);

        _inputHandler = new InputHandler();
    }

    public void Init()
    {
        _player = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().GetPlayer;
        Camera.main.orthographicSize = _defaultCamera;

        ProjectUtility.SetActiveCheck(this.gameObject, true);
        ProjectUtility.SetActiveCheck(_joystickBack.gameObject, false);

    }

    public void ActiveJoystice(bool active)
    {
        _joystickBack.enabled = active;
        _joystickHead.enabled = active;
    }

    // //터치 위치로 해당 오브젝트 이동 후, OnDrag 적용하기 위해 FixedUpdate로 작성
    private void FixedUpdate()
    {
        //터치 2개
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
            Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

            float prevDistance = (touch1PrevPos - touch2PrevPos).magnitude;
            float currentDistance = (touch1.position - touch2.position).magnitude;

            float deltaDistance = prevDistance - currentDistance;

            targetZoom += deltaDistance * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, _minCamera, _maxCamera);
        }

        if (_isTouch && !IsLock)
        {
            //Debug.Log("$ _vectorMove1 = " + _vectorMove);
            _vectorMove.Normalize();
            //Debug.Log("$ _vectorMove2 = " + _vectorMove);
            if (_player != null)
            {
                _player.MoveVector(_vectorMove * Time.fixedDeltaTime);
            }
        }

#if UNITY_EDITOR
        if (_player == null) { return; }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            _player.MoveVector(new Vector3(0f, 0.03f, -1f));
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            _player.MoveVector(new Vector3(0f, -0.03f, -1f));
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            _player.MoveVector(new Vector3(-0.03f, 0f, -1f));
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _player.MoveVector(new Vector3(0.03f, 0f, -1f));
        }


        //if (Input.GetKey(KeyCode.Backspace))
        //{
        //    _player.ProductStacker.Kill();
        //    _player.SetMaxStack();
        //}
#endif
    }

    void OnTouch(Vector2 vectorTouch)
    {
        Vector2 vec = vectorTouch - (Vector2)_backRectTransform.position;
        vec = Vector2.ClampMagnitude(vec, _radius);
        _headRectTransform.localPosition = vec;
        Vector2 vectorNormal = vec.normalized;

        //조이스틱 중앙과 조이스틱 헤드의 거리 (sqrMagnitude = 연산빠름) + 반지름 크기 비례
        float sqr = (Vector3.zero - _headRectTransform.transform.localPosition).sqrMagnitude * _radiusDecimal;
        //Debug.Log("$ sqr = " + sqr);

        _vectorMove = new Vector2(vectorNormal.x, vectorNormal.y) * sqr;
    }

    public void OnDrag(PointerEventData eventData)
    {
#if !UNITY_EDITOR
        if (Input.touchCount != 1) { return; }
#endif
        OnTouch(eventData.position);
        _isTouch = true;
        _isDrag = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(IsLock) return;

        ProjectUtility.SetActiveCheck(_joystickBack.gameObject, true);

#if !UNITY_EDITOR
        if (Input.touchCount != 1) { return; }
#endif
        _joystickBack.transform.position = Input.mousePosition;
        OnTouch(eventData.position);
        _isTouch = true;
        

        //UIManager.Instance.CloseTimePackageArea();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ProjectUtility.SetActiveCheck(_joystickBack.gameObject, false);

        if(_player != null)
        {
            _player.PlayAnimation(OtterBase.OtterState.Idle ,"idle", true);
        }

        if (_isTouch && !_isDrag)
        {
            _inputHandler.OnTouch(Input.mousePosition);
        }

        _headRectTransform.localPosition = Vector2.zero;
        _isTouch = false;
        _isDrag = false;

        //if (_player != null)
        //{
        //    _player.StopMove();
        //}
    }
}
