using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BirdNPC : MonoBehaviour
{
    [Header("Movement Bounds (offsets from parent X)")]
    public float leftLimit = -5f;    // 相对于父物体 X 的最左偏移
    public float rightLimit = 5f;    // 相对于父物体 X 的最右偏移

    [Header("Random Distance Range")]
    public float minWalkDistance = 1f;  // 每次行走的最小距离
    public float maxWalkDistance = 3f;  // 每次行走的最大距离

    [Header("Speed")]
    public float moveSpeed = 2f;      // 行走速度

    private float targetX;            // 本次目标世界 X 坐标
    private int direction = 1;        // 当前行走方向：1 向右，-1 向左

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        PickNextTarget();
    }

    void Update()
    {
        // 按当前方向移动
        float step = moveSpeed * Time.deltaTime * direction;
        transform.Translate(step, 0f, 0f);

        // 检查是否到达或越过目标点
        if ((direction > 0 && transform.position.x >= targetX) ||
            (direction < 0 && transform.position.x <= targetX))
        {
            PickNextTarget();
        }
    }

    /// <summary>
    /// 随机选择下一个目标点（基于父物体坐标），并设置动画
    /// </summary>
    private void PickNextTarget()
    {
        // 1) 确定基准 X（父物体的世界坐标）
        float baseX = transform.parent != null
            ? transform.parent.position.x
            : transform.position.x;

        // 2) 随机行走方向
        direction = (Random.value < 0.5f) ? -1 : 1;

        // 3) 随机行走距离
        float distance = Random.Range(minWalkDistance, maxWalkDistance);

        // 4) 计算相对于 baseX 的最小／最大限位
        float minX = baseX + leftLimit;
        float maxX = baseX + rightLimit;

        // 5) 根据方向决定目标点，并 clamp 在 [minX, maxX] 内
        if (direction > 0)
            targetX = Mathf.Min(baseX + distance, maxX);
        else
            targetX = Mathf.Max(baseX - distance, minX);

        // 6) 切换动画参数
        animator.SetFloat("MoveX", direction);
    }

    // 在 Scene 视图中可视化左右边界（相对于父物体）
    void OnDrawGizmosSelected()
    {
        if (transform.parent == null) return;

        float baseX = transform.parent.position.x;
        float y = transform.parent.position.y;
        float minX = baseX + leftLimit;
        float maxX = baseX + rightLimit;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(minX, y - 1f, 0f), new Vector3(minX, y + 1f, 0f));
        Gizmos.DrawLine(new Vector3(maxX, y - 1f, 0f), new Vector3(maxX, y + 1f, 0f));
    }
}
