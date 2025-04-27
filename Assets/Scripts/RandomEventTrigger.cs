using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomEventTrigger : MonoBehaviour
{
    [System.Serializable]
    public class TriggerEvent
    {
        [TextArea] public string description;     // ���ִ�ӡ���ı�
        public float hungerDelta;
        public float thirstDelta;
        public float healthDelta;
        [Range(0, 100)] public float probability; // �������ʣ�Ȩ�أ�
    }

    [Header("�¼��б�")]
    public List<TriggerEvent> events = new List<TriggerEvent>();

    [Header("��������")]
    public KeyCode interactKey = KeyCode.E;        // ��������
    public GameObject dialogueBox;                 // �����Ի�����壬��ʼ��Ϊ inactive
    public TextMeshProUGUI dialogueText;           // ����µ� TextMeshProUGUI

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
            Debug.LogWarning($"[{name}] ��� dialogueBox!");
        if (dialogueText == null)
            Debug.LogWarning($"[{name}] ��� dialogueText!");
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

        // 1. ���ѡ�¼�
        TriggerEvent ev = ChooseRandomEvent();

        // 2. ��ͣ��Ϸ���򿪶Ի���
        Time.timeScale = 0f;
        dialogueBox.SetActive(true);

        // 3. ���ŶԻ����Ȼس��ٽ�����Ӧ�����Ա仯
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
        // 3a. ���ִ�ӡ������ timeScale Ӱ�죩
        dialogueText.text = "";
        foreach (char c in ev.description)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(0.02f);
        }

        // 3b. �����ֺ󣬵���Ұ� Enter
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        dialogueText.text = "";

        // 4. �رնԻ��򡢻ָ���Ϸ
        dialogueBox.SetActive(false);
        Time.timeScale = 1f;

        // 5. ����������޸�����
        if (ev.hungerDelta != 0) cat.ChangeHunger(ev.hungerDelta);
        if (ev.thirstDelta != 0) cat.ChangeThirst(ev.thirstDelta);
        if (ev.healthDelta != 0) cat.ChangeHealth(ev.healthDelta);
        isHappen = false;
    }
}
