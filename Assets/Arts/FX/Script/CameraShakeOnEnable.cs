using UnityEngine;

public class CameraShakeOnEnable : MonoBehaviour
{
    public AnimationCurve shakeCurve; // ����ũ�� Ŀ������ �� �ִ� �ִϸ��̼� Ŀ��
    public float duration = 1.0f; // ����ũ�� ���� �ð�

    private Transform cameraTransform; // ���� ī�޶��� Ʈ������
    private Vector3 originalPos; // ī�޶��� ���� ��ġ

    private void OnEnable()
    {
        // ���� ī�޶� �±׸� ���� ã��
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera != null)
        {
            cameraTransform = mainCamera.transform;
            originalPos = cameraTransform.localPosition;
            StartCoroutine(ShakeCamera());
        }
        else
        {
            Debug.LogWarning("MainCamera �±׸� ���� ī�޶� �������� �ʽ��ϴ�.");
        }
    }

    private System.Collections.IEnumerator ShakeCamera()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // elapsedTime�� duration�� ������ ���� ���� Ŀ���� X������ ���
            float curveTime = elapsedTime / duration;
            float strength = shakeCurve.Evaluate(curveTime);

            // ī�޶��� ��鸲�� X�� Y�࿡ �����ϰ� ����
            Vector3 shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * strength,
                Random.Range(-1f, 1f) * strength,
                0f // Z���� �״�� ����
            );

            cameraTransform.localPosition = originalPos + shakeOffset;

            elapsedTime += Time.deltaTime;

            yield return null; // ���� �����ӱ��� ���
        }

        // ī�޶��� ��ġ�� ������� ����
        cameraTransform.localPosition = originalPos;
    }

    private void OnDisable()
    {
        // ������Ʈ�� ��Ȱ��ȭ�� �� ī�޶� ��ġ�� ������� ����
        if (cameraTransform != null)
        {
            cameraTransform.localPosition = originalPos;
        }
    }
}
