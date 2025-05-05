using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BirdNPC : MonoBehaviour
{
    [Header("Movement Bounds (offsets from parent X)")]
    public float leftLimit = -5f;    // ����ڸ����� X ������ƫ��
    public float rightLimit = 5f;    // ����ڸ����� X ������ƫ��

    [Header("Random Distance Range")]
    public float minWalkDistance = 1f;  // ÿ�����ߵ���С����
    public float maxWalkDistance = 3f;  // ÿ�����ߵ�������

    [Header("Speed")]
    public float moveSpeed = 2f;      // �����ٶ�

    private float targetX;            // ����Ŀ������ X ����
    private int direction = 1;        // ��ǰ���߷���1 ���ң�-1 ����

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        PickNextTarget();
    }

    void Update()
    {
        // ����ǰ�����ƶ�
        float step = moveSpeed * Time.deltaTime * direction;
        transform.Translate(step, 0f, 0f);

        // ����Ƿ񵽴��Խ��Ŀ���
        if ((direction > 0 && transform.position.x >= targetX) ||
            (direction < 0 && transform.position.x <= targetX))
        {
            PickNextTarget();
        }
    }

    /// <summary>
    /// ���ѡ����һ��Ŀ��㣨���ڸ��������꣩�������ö���
    /// </summary>
    private void PickNextTarget()
    {
        // 1) ȷ����׼ X����������������꣩
        float baseX = transform.parent != null
            ? transform.parent.position.x
            : transform.position.x;

        // 2) ������߷���
        direction = (Random.value < 0.5f) ? -1 : 1;

        // 3) ������߾���
        float distance = Random.Range(minWalkDistance, maxWalkDistance);

        // 4) ��������� baseX ����С�������λ
        float minX = baseX + leftLimit;
        float maxX = baseX + rightLimit;

        // 5) ���ݷ������Ŀ��㣬�� clamp �� [minX, maxX] ��
        if (direction > 0)
            targetX = Mathf.Min(baseX + distance, maxX);
        else
            targetX = Mathf.Max(baseX - distance, minX);

        // 6) �л���������
        animator.SetFloat("MoveX", direction);
    }

    // �� Scene ��ͼ�п��ӻ����ұ߽磨����ڸ����壩
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
