using UnityEngine;
using UnityEngine.UI;               // 用于 Image
using UnityEngine.SceneManagement;  // 用于场景切换
using System.Collections;

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
    [Tooltip("每秒恢复多少健康度（正常状态下）")]
    public float healthRecoveryPerSecond = 0.1f;
    [Tooltip("每秒掉多少生命值，当饥饿或口渴归零时生效")]
    public float healthDecayPerSecond = 1f;

    [HideInInspector] public float hunger;
    [HideInInspector] public float thirst;
    [HideInInspector] public float health;

    [Header("UI Bars (Image)")]
    [Tooltip("饱食度填充条 (Image.Type = Filled → Horizontal)")]
    public Image hungerFill;
    [Tooltip("口渴度填充条 (Image.Type = Filled → Horizontal)")]
    public Image thirstFill;
    [Tooltip("健康度填充条 (Image.Type = Filled → Horizontal)")]
    public Image healthFill;

    private Vector2 moveDir;
    public Image fadeImage;
    public float fadeDuration = 2f;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // 初始化属性为满值
        hunger = thirst = health = maxValue;
    }

    void Update()
    {
        // —— 1. 处理移动输入与动画 —— 
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        animator.SetFloat("MoveX", h);
        animator.SetFloat("MoveY", v);

        Vector2 raw = new Vector2(h, v);
        if (raw != Vector2.zero)
        {
            moveDir = Mathf.Abs(raw.x) > Mathf.Abs(raw.y)
                ? new Vector2(Mathf.Sign(raw.x), 0f)
                : new Vector2(0f, Mathf.Sign(raw.y));
        }
        else
        {
            moveDir = Vector2.zero;
        }

        // —— 2. 饥饿 & 口渴 持续衰减，Clamp 保证 ≥ 0 —— 
        hunger = Mathf.Clamp(hunger - hungerDecayPerSecond * Time.deltaTime, 0f, maxValue);
        thirst = Mathf.Clamp(thirst - thirstDecayPerSecond * Time.deltaTime, 0f, maxValue);

        // —— 3. 生命值：只有当 hunger>0 且 thirst>0 时才恢复，否则按衰减逻辑掉血 —— 
        if (hunger > 0f && thirst > 0f)
        {
            // 双项都恢复时，才正常恢复血量
            health = Mathf.Clamp(health + healthRecoveryPerSecond * Time.deltaTime, 0f, maxValue);
        }
        else
        {
            // 只要有一项归零，就开始掉血
            float decayRate = healthDecayPerSecond;
            // 两项都归零时，掉血速率翻倍
            if (hunger <= 0f && thirst <= 0f)
                decayRate *= 2f;

            health = Mathf.Clamp(health - decayRate * Time.deltaTime, 0f, maxValue);
        }

        // —— 4. 刷新 UI —— 
        if (hungerFill != null) hungerFill.fillAmount = hunger / maxValue;
        if (thirstFill != null) thirstFill.fillAmount = thirst / maxValue;
        if (healthFill != null) healthFill.fillAmount = health / maxValue;

        // —— 5. 血量归零，跳转到坏结局场景 —— 
        if (health <= 0f)
        {
            StartCoroutine(FadeOutAndLoad("DeathScene"));
        }
    }

    void FixedUpdate()
    {
        // 用物理速度驱动角色运动
        rb.linearVelocity = moveDir * moveSpeed;
    }

    // —— 对外接口：随时增减属性，并 Clamp 到 [0, maxValue] —— 
    public void ChangeHunger(float delta)
    {
        hunger = Mathf.Clamp(hunger + delta, 0f, maxValue);
        if (hungerFill != null) hungerFill.fillAmount = hunger / maxValue;
    }

    public void ChangeThirst(float delta)
    {
        thirst = Mathf.Clamp(thirst + delta, 0f, maxValue);
        if (thirstFill != null) thirstFill.fillAmount = thirst / maxValue;
    }

    public void ChangeHealth(float delta)
    {
        health = Mathf.Clamp(health + delta, 0f, maxValue);
        if (healthFill != null) healthFill.fillAmount = health / maxValue;
    }
    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        float timer = 0f;
        Color c = fadeImage.color;
        // 从 alpha=0 → alpha=1
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float a = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }
        // 确保完全黑屏
        fadeImage.color = new Color(c.r, c.g, c.b, 1f);
        SceneManager.LoadScene(sceneName);
    }
}
