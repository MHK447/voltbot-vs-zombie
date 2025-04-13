using UnityEngine;

public class CameraShakeOnEnable : MonoBehaviour
{
    public AnimationCurve shakeCurve; // 쉐이크를 커스텀할 수 있는 애니메이션 커브
    public float duration = 1.0f; // 쉐이크의 지속 시간

    private Transform cameraTransform; // 메인 카메라의 트랜스폼
    private Vector3 originalPos; // 카메라의 원래 위치

    private void OnEnable()
    {
        // 메인 카메라를 태그를 통해 찾기
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera != null)
        {
            cameraTransform = mainCamera.transform;
            originalPos = cameraTransform.localPosition;
            StartCoroutine(ShakeCamera());
        }
        else
        {
            Debug.LogWarning("MainCamera 태그를 가진 카메라가 존재하지 않습니다.");
        }
    }

    private System.Collections.IEnumerator ShakeCamera()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // elapsedTime이 duration의 비율로 계산된 값을 커브의 X축으로 사용
            float curveTime = elapsedTime / duration;
            float strength = shakeCurve.Evaluate(curveTime);

            // 카메라의 흔들림을 X와 Y축에 랜덤하게 적용
            Vector3 shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * strength,
                Random.Range(-1f, 1f) * strength,
                0f // Z축은 그대로 유지
            );

            cameraTransform.localPosition = originalPos + shakeOffset;

            elapsedTime += Time.deltaTime;

            yield return null; // 다음 프레임까지 대기
        }

        // 카메라의 위치를 원래대로 복구
        cameraTransform.localPosition = originalPos;
    }

    private void OnDisable()
    {
        // 오브젝트가 비활성화될 때 카메라 위치를 원래대로 복구
        if (cameraTransform != null)
        {
            cameraTransform.localPosition = originalPos;
        }
    }
}
