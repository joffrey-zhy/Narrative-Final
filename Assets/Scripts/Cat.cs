using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Cat : MonoBehaviour
{
    [Tooltip("移动速度（单位：单位/秒）")]
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
        // 1) 读输入原始向量（-1/0/1）
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 raw = new Vector2(h, v);

        // 2) 将它直接传给 Animator 参数，让 Blend Tree 做插值
        animator.SetFloat("MoveX", raw.x);
        animator.SetFloat("MoveY", raw.y);
    }

    void FixedUpdate()
    {
        // 3) 同样用“主导轴优先”逻辑保证不斜向
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
