using UnityEngine;
using UnityEngine.UI;  // 用于 Image
using UnityEngine.Animations;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Cat : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("移动速度（单位：单位/秒）")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;

    [Header("Stats")]
    [Tooltip("三种属性的最大值")]
    public float maxValue = 100f;
    [Tooltip("每秒减少多少饱食度")]
    public float hungerDecayPerSecond = 1f;
    [Tooltip("每秒减少多少口渴度")]
    public float thirstDecayPerSecond = 1.2f;
    [Tooltip("每秒恢复多少健康度")]
    public float healthRecoveryPerSecond = 0.1f;

    public float hunger;
    public float thirst;
    public float health;

    [Header("UI Bars (Image)")]
    [Tooltip("饱食度填充条 (Image.Type = Filled → Horizontal)")]
    public Image hungerFill;
    [Tooltip("口渴度填充条 (Image.Type = Filled → Horizontal)")]
    public Image thirstFill;
    [Tooltip("健康度填充条 (Image.Type = Filled → Horizontal)")]
    public Image healthFill;

    // 用来缓存当前每帧的移动方向
    private Vector2 moveDir;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // 冻结旋转，防止碰撞时翻滚
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // 初始化三种属性为满值
        hunger = thirst = health = maxValue;
    }

    void Update()
    {
        // ——— 1. 读输入原始向量，让动画播放正确方向 ———
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        animator.SetFloat("MoveX", h);
        animator.SetFloat("MoveY", v);

        // 计算「主导轴优先」的目标移动方向
        Vector2 raw = new Vector2(h, v);
        if (raw != Vector2.zero)
        {
            if (Mathf.Abs(raw.x) > Mathf.Abs(raw.y))
                moveDir = new Vector2(Mathf.Sign(raw.x), 0f);
            else
                moveDir = new Vector2(0f, Mathf.Sign(raw.y));
        }
        else
        {
            moveDir = Vector2.zero;
        }

        // ——— 2. 属性随时间衰减／恢复 ———
        hunger = Mathf.Clamp(hunger - hungerDecayPerSecond * Time.deltaTime, 0f, maxValue);
        thirst = Mathf.Clamp(thirst - thirstDecayPerSecond * Time.deltaTime, 0f, maxValue);
        health = Mathf.Clamp(health + healthRecoveryPerSecond * Time.deltaTime, 0f, maxValue);

        // ——— 3. 同步到 UI 血条 ———
        if (hungerFill != null) hungerFill.fillAmount = hunger / maxValue;
        if (thirstFill != null) thirstFill.fillAmount = thirst / maxValue;
        if (healthFill != null) healthFill.fillAmount = health / maxValue;
    }

    void FixedUpdate()
    {
        // ——— 用物理速度驱动角色 ———
        rb.linearVelocity = moveDir * moveSpeed;
    }

    // ——— 对外接口：在游戏事件中调用，立即增减属性并刷新条 ———
    /// <summary>
    /// 改变饱食度，delta 正为增，负为减
    /// </summary>
    public void ChangeHunger(float delta)
    {
        hunger = Mathf.Clamp(hunger + delta, 0f, maxValue);
        if (hungerFill != null) hungerFill.fillAmount = hunger / maxValue;
    }

    /// <summary>
    /// 改变口渴度，delta 正为增，负为减
    /// </summary>
    public void ChangeThirst(float delta)
    {
        thirst = Mathf.Clamp(thirst + delta, 0f, maxValue);
        if (thirstFill != null) thirstFill.fillAmount = thirst / maxValue;
    }

    /// <summary>
    /// 改变健康度，delta 正为增，负为减
    /// </summary>
    public void ChangeHealth(float delta)
    {
        health = Mathf.Clamp(health + delta, 0f, maxValue);
        if (healthFill != null) healthFill.fillAmount = health / maxValue;
    }
}
