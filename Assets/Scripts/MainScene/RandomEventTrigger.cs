using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomEventTrigger : MonoBehaviour
{
    [System.Serializable]
    public class TriggerEvent
    {
        [TextArea] public string description;
        public float hungerDelta;
        public float thirstDelta;
        public float healthDelta;
        [Range(0, 100)] public float probability;
    }

    [Header("事件列表")]
    public List<TriggerEvent> events = new List<TriggerEvent>();

    [Header("交互设置")]
    public KeyCode interactKey = KeyCode.E;
    public GameObject dialogueBox;         // 对话框面板 Prefab
    public TextMeshProUGUI dialogueText;   // 对话文本组件

    [Header("冷却设置")]
    [Tooltip("触发后需要等待的游戏内小时数")]
    public float cooldownHours = 10f;
    [Tooltip("冷却期间的提示文字")]
    [TextArea] public string cooldownMessage = "There is nothing left here for the time being, so please come back later!";

    // —— 内部状态 —— //
    private bool playerInRange = false;
    private bool isHappening = false;
    private DayNightCycle dayNight;
    private float lastTriggerGlobal = -Mathf.Infinity;

    [System.Obsolete]
    void Awake()
    {
        dayNight = FindObjectOfType<DayNightCycle>();
        if (dialogueBox != null) dialogueBox.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Cat>() != null)
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Cat>() != null)
            playerInRange = false;
    }

    [System.Obsolete]
    void Update()
    {
        if (!playerInRange || isHappening || Input.GetKeyDown(interactKey) == false)
            return;

        // 1. 计算当前全局时间（小时）
        float nowGlobal = dayNight.CurrentDay * 24f + dayNight.CurrentHour;

        // 2. 冷却判断
        if (nowGlobal - lastTriggerGlobal < cooldownHours)
        {
            // 冷却中：只弹提示文字
            StartCoroutine(ShowCooldownMessage());
        }
        else
        {
            // 已过冷却：正常触发
            lastTriggerGlobal = nowGlobal;
            isHappening = true;
            TriggerRandomEvent();
        }
    }

    // —— 正常事件逻辑（和你原来一样） —— //
    [System.Obsolete]
    void TriggerRandomEvent()
    {
        if (events.Count == 0) return;

        var ev = ChooseRandomEvent();
        Time.timeScale = 0f;
        dialogueBox.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(EventCoroutine(ev));
    }

    TriggerEvent ChooseRandomEvent()
    {
        float total = 0;
        events.ForEach(e => total += e.probability);
        float pick = Random.Range(0f, total), sum = 0;
        foreach (var e in events)
        {
            sum += e.probability;
            if (pick <= sum) return e;
        }
        return events[0];
    }

    [System.Obsolete]
    IEnumerator EventCoroutine(TriggerEvent ev)
    {
        dialogueText.text = "";
        foreach (char c in ev.description)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(0.02f);
        }
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

        dialogueBox.SetActive(false);
        Time.timeScale = 1f;

        // 应用属性变化
        var cat = FindObjectOfType<Cat>();
        cat?.ChangeHunger(ev.hungerDelta);
        cat?.ChangeThirst(ev.thirstDelta);
        cat?.ChangeHealth(ev.healthDelta);

        isHappening = false;
    }

    // —— 冷却提示逻辑 —— //
    IEnumerator ShowCooldownMessage()
    {
        isHappening = true;
        Time.timeScale = 0f;
        dialogueBox.SetActive(true);

        dialogueText.text = "";
        foreach (char c in cooldownMessage)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(0.02f);
        }
        // 提示读完后，等几秒或等玩家回车也行
        yield return new WaitForSecondsRealtime(1.5f);

        dialogueBox.SetActive(false);
        Time.timeScale = 1f;
        isHappening = false;
    }
}
