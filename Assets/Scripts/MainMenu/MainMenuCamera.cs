using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [Header("往返区间")]
    public float minX = 0f;      // 最小 X 值
    public float maxX = 16f;     // 最大 X 值

    [Header("移动速度")]
    public float speed = 1f;     // 控制往返的快慢

    private Vector3 _startPosition;

    void Start()
    {
        // 记录摄像机初始的 Y 和 Z 坐标
        _startPosition = transform.position;
    }

    void Update()
    {
        // 计算当前应在区间 [0, maxX - minX] 内的位置
        float offset = Mathf.PingPong(Time.time * speed, maxX - minX);
        float x = minX + offset;

        // 应用新的位置（保持原来的 y, z 不变）
        transform.position = new Vector3(x, _startPosition.y, _startPosition.z);
    }
}
