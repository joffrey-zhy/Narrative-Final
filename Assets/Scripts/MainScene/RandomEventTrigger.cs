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

    [Header("�¼��б�")]
    public List<TriggerEvent> events = new List<TriggerEvent>();

    [Header("��������")]
    public KeyCode interactKey = KeyCode.E;
    public GameObject dialogueBox;         // �Ի������ Prefab
    public TextMeshProUGUI dialogueText;   // �Ի��ı����

    [Header("��ȴ����")]
    [Tooltip("��������Ҫ�ȴ�����Ϸ��Сʱ��")]
    public float cooldownHours = 10f;
    [Tooltip("��ȴ�ڼ����ʾ����")]
    [TextArea] public string cooldownMessage = "There is nothing left here for the time being, so please come back later!";

    // ���� �ڲ�״̬ ���� //
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

        // 1. ���㵱ǰȫ��ʱ�䣨Сʱ��
        float nowGlobal = dayNight.CurrentDay * 24f + dayNight.CurrentHour;

        // 2. ��ȴ�ж�
        if (nowGlobal - lastTriggerGlobal < cooldownHours)
        {
            // ��ȴ�У�ֻ����ʾ����
            StartCoroutine(ShowCooldownMessage());
        }
        else
        {
            // �ѹ���ȴ����������
            lastTriggerGlobal = nowGlobal;
            isHappening = true;
            TriggerRandomEvent();
        }
    }

    // ���� �����¼��߼�������ԭ��һ���� ���� //
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

        // Ӧ�����Ա仯
        var cat = FindObjectOfType<Cat>();
        cat?.ChangeHunger(ev.hungerDelta);
        cat?.ChangeThirst(ev.thirstDelta);
        cat?.ChangeHealth(ev.healthDelta);

        isHappening = false;
    }

    // ���� ��ȴ��ʾ�߼� ���� //
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
        // ��ʾ����󣬵ȼ�������һس�Ҳ��
        yield return new WaitForSecondsRealtime(1.5f);

        dialogueBox.SetActive(false);
        Time.timeScale = 1f;
        isHappening = false;
    }
}
