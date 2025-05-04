using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Tooltip("要跟随的目标（你的猫）")]
    public Transform target;

    [Tooltip("相机与目标的偏移，Z 轴一般设为 -10 保证相机在 2D 场景外侧")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Tooltip("平滑过渡时间，越小跟得越紧，越大越慢")]
    public float smoothTime = 0.2f;

    // 内部追踪速度，不要改
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // 目标位置 + 偏移
        Vector3 targetPos = target.position + offset;

        // 平滑插值到目标位置
        transform.position = Vector3.SmoothDamp(
            current: transform.position,
            target: targetPos,
            currentVelocity: ref velocity,
            smoothTime: smoothTime
        );
    }
}
