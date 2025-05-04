using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BirdNPC : MonoBehaviour
{
    [Header("Movement Bounds (world X positions)")]
    public float leftLimit = -5f;    // 区间最左端 x 坐标
    public float rightLimit = 5f;    // 区间最右端 x 坐标

    [Header("Random Distance Range")]
    public float minWalkDistance = 1f;  // 每次行走的最小距离
    public float maxWalkDistance = 3f;  // 每次行走的最大距离

    [Header("Speed")]
    public float moveSpeed = 2f;      // 行走速度

    private float targetX;            // 本次目标 x 坐标
    private int direction = 1;        // 当前行走方向：1 向右，-1 向左

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        PickNextTarget();
    }

    void Update()
    {
        // 朝目标移动
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
    /// 随机选择下一个目标点并设置动画
    /// </summary>
    private void PickNextTarget()
    {
        // 随机决定下次行走方向
        direction = (Random.value < 0.5f) ? -1 : 1;

        // 随机决定行走距离
        float distance = Random.Range(minWalkDistance, maxWalkDistance);

        // 计算目标 x，并 clamp 到左右边界
        if (direction > 0)
            targetX = Mathf.Min(transform.position.x + distance, rightLimit);
        else
            targetX = Mathf.Max(transform.position.x - distance, leftLimit);

        // 根据方向切换动画
        if (direction > 0)
        {
            animator.SetFloat("MoveX", +1f);
        }
        else
        {
            animator.SetFloat("MoveX", -1f);
        }
    }

    // Gizmos：在 Scene 视图中可视化左右边界
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(leftLimit, transform.position.y - 1, 0),
                        new Vector3(leftLimit, transform.position.y + 1, 0));
        Gizmos.DrawLine(new Vector3(rightLimit, transform.position.y - 1, 0),
                        new Vector3(rightLimit, transform.position.y + 1, 0));
    }
}
