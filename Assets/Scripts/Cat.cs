using UnityEngine;
using UnityEngine.UI;  // ���� Image
using UnityEngine.Animations;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Cat : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("�ƶ��ٶȣ���λ����λ/�룩")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;

    [Header("Stats")]
    [Tooltip("�������Ե����ֵ")]
    public float maxValue = 100f;
    [Tooltip("ÿ����ٶ��ٱ�ʳ��")]
    public float hungerDecayPerSecond = 1f;
    [Tooltip("ÿ����ٶ��ٿڿʶ�")]
    public float thirstDecayPerSecond = 1.2f;
    [Tooltip("ÿ��ָ����ٽ�����")]
    public float healthRecoveryPerSecond = 0.5f;

    [HideInInspector] public float hunger;
    [HideInInspector] public float thirst;
    [HideInInspector] public float health;

    [Header("UI Bars (Image)")]
    [Tooltip("��ʳ������� (Image.Type = Filled �� Horizontal)")]
    public Image hungerFill;
    [Tooltip("�ڿʶ������ (Image.Type = Filled �� Horizontal)")]
    public Image thirstFill;
    [Tooltip("����������� (Image.Type = Filled �� Horizontal)")]
    public Image healthFill;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;

        // ��ʼ����������Ϊ��ֵ
        hunger = thirst = health = maxValue;
    }

    void Update()
    {
        // ������ 1. ������ԭʼ�������ö���������ȷ���� ������
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        animator.SetFloat("MoveX", h);
        animator.SetFloat("MoveY", v);

        // ������ 2. ������ʱ��˥�����ָ� ������
        hunger = Mathf.Clamp(hunger - hungerDecayPerSecond * Time.deltaTime, 0f, maxValue);
        thirst = Mathf.Clamp(thirst - thirstDecayPerSecond * Time.deltaTime, 0f, maxValue);
        health = Mathf.Clamp(health + healthRecoveryPerSecond * Time.deltaTime, 0f, maxValue);

        // ������ 3. ͬ���� UI Ѫ�� ������
        if (hungerFill != null) hungerFill.fillAmount = hunger / maxValue;
        if (thirstFill != null) thirstFill.fillAmount = thirst / maxValue;
        if (healthFill != null) healthFill.fillAmount = health / maxValue;
    }

    void FixedUpdate()
    {
        // ������ �����������ȡ��ƶ��߼���������Խ� ������
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

    // ������ ����ӿڣ�����Ϸ�¼��е��ã������������Բ�ˢ���� ������
    /// <summary>
    /// �ı䱥ʳ�ȣ�delta ��Ϊ������Ϊ��
    /// </summary>
    public void ChangeHunger(float delta)
    {
        hunger = Mathf.Clamp(hunger + delta, 0f, maxValue);
        if (hungerFill != null) hungerFill.fillAmount = hunger / maxValue;
    }

    /// <summary>
    /// �ı�ڿʶȣ�delta ��Ϊ������Ϊ��
    /// </summary>
    public void ChangeThirst(float delta)
    {
        thirst = Mathf.Clamp(thirst + delta, 0f, maxValue);
        if (thirstFill != null) thirstFill.fillAmount = thirst / maxValue;
    }

    /// <summary>
    /// �ı佡���ȣ�delta ��Ϊ������Ϊ��
    /// </summary>
    public void ChangeHealth(float delta)
    {
        health = Mathf.Clamp(health + delta, 0f, maxValue);
        if (healthFill != null) healthFill.fillAmount = health / maxValue;
    }
}
