using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BirdNPC : MonoBehaviour
{
    [Header("Movement Bounds (world X positions)")]
    public float leftLimit = -5f;    // ��������� x ����
    public float rightLimit = 5f;    // �������Ҷ� x ����

    [Header("Random Distance Range")]
    public float minWalkDistance = 1f;  // ÿ�����ߵ���С����
    public float maxWalkDistance = 3f;  // ÿ�����ߵ�������

    [Header("Speed")]
    public float moveSpeed = 2f;      // �����ٶ�

    private float targetX;            // ����Ŀ�� x ����
    private int direction = 1;        // ��ǰ���߷���1 ���ң�-1 ����

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        PickNextTarget();
    }

    void Update()
    {
        // ��Ŀ���ƶ�
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
    /// ���ѡ����һ��Ŀ��㲢���ö���
    /// </summary>
    private void PickNextTarget()
    {
        // ��������´����߷���
        direction = (Random.value < 0.5f) ? -1 : 1;

        // ����������߾���
        float distance = Random.Range(minWalkDistance, maxWalkDistance);

        // ����Ŀ�� x���� clamp �����ұ߽�
        if (direction > 0)
            targetX = Mathf.Min(transform.position.x + distance, rightLimit);
        else
            targetX = Mathf.Max(transform.position.x - distance, leftLimit);

        // ���ݷ����л�����
        if (direction > 0)
        {
            animator.SetFloat("MoveX", +1f);
        }
        else
        {
            animator.SetFloat("MoveX", -1f);
        }
    }

    // Gizmos���� Scene ��ͼ�п��ӻ����ұ߽�
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(leftLimit, transform.position.y - 1, 0),
                        new Vector3(leftLimit, transform.position.y + 1, 0));
        Gizmos.DrawLine(new Vector3(rightLimit, transform.position.y - 1, 0),
                        new Vector3(rightLimit, transform.position.y + 1, 0));
    }
}
