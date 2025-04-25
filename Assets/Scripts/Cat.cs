using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Cat : MonoBehaviour
{
    [Tooltip("�ƶ��ٶȣ���λ����λ/�룩")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // 1) ������ԭʼ������-1/0/1��
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 raw = new Vector2(h, v);

        // 2) ����ֱ�Ӵ��� Animator �������� Blend Tree ����ֵ
        animator.SetFloat("MoveX", raw.x);
        animator.SetFloat("MoveY", raw.y);
    }

    void FixedUpdate()
    {
        // 3) ͬ���á����������ȡ��߼���֤��б��
        Vector2 raw = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        Vector2 moveDir = Vector2.zero;
        if (raw != Vector2.zero)
        {
            if (Mathf.Abs(raw.x) > Mathf.Abs(raw.y))
                moveDir = new Vector2(Mathf.Sign(raw.x), 0f);
            else
                moveDir = new Vector2(0f, Mathf.Sign(raw.y));
        }

        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }
}
