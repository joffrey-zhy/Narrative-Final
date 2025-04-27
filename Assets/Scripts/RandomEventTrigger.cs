using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomEventTrigger : MonoBehaviour
{
    [System.Serializable]
    public class TriggerEvent
    {
        [TextArea] public string description;     // 逐字打印的文本
        public float hungerDelta;
        public float thirstDelta;
        public float healthDelta;
        [Range(0, 100)] public float probability; // 发生概率（权重）
    }

    [Header("事件列表")]
    public List<TriggerEvent> events = new List<TriggerEvent>();

    [Header("交互设置")]
    public KeyCode interactKey = KeyCode.E;        // 交互按键
    public GameObject dialogueBox;                 // 整个对话框面板，初始设为 inactive
    public TextMeshProUGUI dialogueText;           // 面板下的 TextMeshProUGUI

    private bool playerInRange = false;
    private Cat cat;
    private bool isHappen = false;

    [System.Obsolete]
    void Awake()
    {
        cat = FindObjectOfType<Cat>();
        if (dialogueBox != null)
            dialogueBox.SetActive(false);
        else
            Debug.LogWarning($"[{name}] 请绑定 dialogueBox!");
        if (dialogueText == null)
            Debug.LogWarning($"[{name}] 请绑定 dialogueText!");
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

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey) && isHappen == false){
            isHappen = true;
            TriggerRandomEvent(); 
        }
    }

    void TriggerRandomEvent()
    {
        if (events.Count == 0 || cat == null) return;

        // 1. 随机选事件
        TriggerEvent ev = ChooseRandomEvent();

        // 2. 暂停游戏、打开对话框
        Time.timeScale = 0f;
        dialogueBox.SetActive(true);

        // 3. 播放对话，等回车再结束并应用属性变化
        StopAllCoroutines();
        StartCoroutine(EventCoroutine(ev));
    }

    TriggerEvent ChooseRandomEvent()
    {
        float total = 0;
        foreach (var e in events) total += e.probability;
        float roll = Random.Range(0f, total);
        float sum = 0;
        foreach (var e in events)
        {
            sum += e.probability;
            if (roll <= sum) return e;
        }
        return events[0];
    }

    IEnumerator EventCoroutine(TriggerEvent ev)
    {
        // 3a. 逐字打印（不受 timeScale 影响）
        dialogueText.text = "";
        foreach (char c in ev.description)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(0.02f);
        }

        // 3b. 打完字后，等玩家按 Enter
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        dialogueText.text = "";

        // 4. 关闭对话框、恢复游戏
        dialogueBox.SetActive(false);
        Time.timeScale = 1f;

        // 5. 最后再真正修改属性
        if (ev.hungerDelta != 0) cat.ChangeHunger(ev.hungerDelta);
        if (ev.thirstDelta != 0) cat.ChangeThirst(ev.thirstDelta);
        if (ev.healthDelta != 0) cat.ChangeHealth(ev.healthDelta);
        isHappen = false;
    }
}
