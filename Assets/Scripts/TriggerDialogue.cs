using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class TriggerDialogue : MonoBehaviour
{
    [Header("对话设置")]
    [TextArea, Tooltip("完整对话内容")]
    public string fullText = "你好，这是一个提示！";
    [Tooltip("逐字打印速度（秒/字）")]
    public float typeSpeed = 0.05f;
    [Tooltip("对话完全打印后自动消失的延迟（秒）")]
    public float disappearDelay = 5f;
    [Tooltip("触发冷却（游戏内小时），例如 12 小时后才可再次触发")]
    public float cooldownHours = 12f;
    [Tooltip("世界空间 UI 预制体，需包含 TextMeshProUGUI 子物体")]
    public GameObject dialogueUIPrefab;
    [Tooltip("对话框相对物体的偏移量")]
    public Vector3 uiOffset = new Vector3(0, 2f, 0);
    [Header("UI 父节点（在 Inspector 里拖入）")]
    public Transform uiParent;
    [Header("提示图标设置")]
    [Tooltip("玩家进入范围时显示的提示图标 Prefab")]
    public GameObject promptIconPrefab;
    [Tooltip("提示图标相对物体的偏移量")]
    public Vector3 promptIconOffset = new Vector3(0, 3f, 0);

    // —— internal state —— //
    private GameObject dialogueInstance;
    private TextMeshProUGUI dialogueText;
    private bool isShowing = false;
    private float lastTriggerGlobal = -Mathf.Infinity;
    private Coroutine showRoutine;
    private DayNightCycle dayNightCycle;
    private GameObject promptIconInstance;

    [System.Obsolete]
    void Awake()
    {
        // 确保 Collider2D 是触发器
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        // 拿到 DayNightCycle，用于冷却检测
        dayNightCycle = FindObjectOfType<DayNightCycle>();

        /*// 计算 UI 父节点，如果没有就挂到根
        if (transform.parent != null && transform.parent.parent != null)
            uiParent = transform.parent.parent;
        else
            uiParent = null;*/
    }

    void Update()
    {
        // 如果提示图标存在，就实时更新它的位置
        if (promptIconInstance != null)
        {
            Vector3 worldPos = transform.position + promptIconOffset;
            promptIconInstance.transform.position = worldPos;
        }

        // 如果对话框打开，也可以按需实时跟随（可选）
        if (dialogueInstance != null)
        {
            Vector3 dialogPos = transform.position + uiOffset;
            dialogueInstance.transform.position = dialogPos;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 1. 显示提示图标
        if (promptIconPrefab != null && promptIconInstance == null)
        {
            Vector3 iconPos = transform.position + promptIconOffset;
            promptIconInstance = Instantiate(
                promptIconPrefab,
                iconPos,
                Quaternion.identity,
                uiParent
            );
        }

        // 2. 冷却检查 & 弹对话
        if (isShowing || dialogueUIPrefab == null) return;
        if (dayNightCycle != null)
        {
            float nowGlobal = dayNightCycle.CurrentDay * 24f + dayNightCycle.CurrentHour;
            if (nowGlobal - lastTriggerGlobal < cooldownHours)
                return;
            lastTriggerGlobal = nowGlobal;
        }

        ShowDialogue();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 隐藏并销毁提示图标
        if (promptIconInstance != null)
        {
            Destroy(promptIconInstance);
            promptIconInstance = null;
        }

        // （可选）离开时也销毁对话
        // if (isShowing) HideDialogue();
    }

    private void ShowDialogue()
    {
        Vector3 pos = transform.position + uiOffset;
        dialogueInstance = Instantiate(
            dialogueUIPrefab,
            pos,
            Quaternion.identity,
            uiParent
        );

        dialogueText = dialogueInstance.GetComponentInChildren<TextMeshProUGUI>();
        if (dialogueText == null)
        {
            Debug.LogError("TriggerDialogue: Prefab 中缺少 TextMeshProUGUI 组件！");
            return;
        }

        isShowing = true;
        showRoutine = StartCoroutine(ShowAndAutoHide());
    }

    private IEnumerator ShowAndAutoHide()
    {
        dialogueText.text = "";
        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typeSpeed);
        }

        yield return new WaitForSecondsRealtime(disappearDelay);
        HideDialogue();
    }

    private void HideDialogue()
    {
        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
            showRoutine = null;
        }
        if (dialogueInstance != null)
        {
            Destroy(dialogueInstance);
            dialogueInstance = null;
        }
        isShowing = false;
    }
}
