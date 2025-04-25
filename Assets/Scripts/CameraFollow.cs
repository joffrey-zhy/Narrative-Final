using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Tooltip("Ҫ�����Ŀ�꣨���è��")]
    public Transform target;

    [Tooltip("�����Ŀ���ƫ�ƣ�Z ��һ����Ϊ -10 ��֤����� 2D �������")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Tooltip("ƽ������ʱ�䣬ԽС����Խ����Խ��Խ��")]
    public float smoothTime = 0.2f;

    // �ڲ�׷���ٶȣ���Ҫ��
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // Ŀ��λ�� + ƫ��
        Vector3 targetPos = target.position + offset;

        // ƽ����ֵ��Ŀ��λ��
        transform.position = Vector3.SmoothDamp(
            current: transform.position,
            target: targetPos,
            currentVelocity: ref velocity,
            smoothTime: smoothTime
        );
    }
}
