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

    [Header("��ӡ��չʾ����")]
    [Tooltip("���ִ�ӡ��Ϻ���Ҫ��ʾ�� GameObject")]
    public GameObject objectToReveal;

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
        if (objectToReveal != null) objectToReveal.SetActive(false);
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
        if (!playerInRange || isHappening || !Input.GetKeyDown(interactKey))
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

    // ���� �����¼��߼� ���� //
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
        // 1) ��ղ����ִ�ӡ
        dialogueText.text = "";
        foreach (char c in ev.description)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(0.02f);
        }

        // 2) �ı���ӡ��Ϻ���ʾָ���� GameObject
        if (objectToReveal != null)
            objectToReveal.SetActive(true);

        // 3) �ȴ���Ұ��»س�
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

        // 4) �����Ǹ� GameObject
        if (objectToReveal != null)
            objectToReveal.SetActive(false);

        // 5) ���ضԻ��򲢻ָ�ʱ������
        dialogueBox.SetActive(false);
        Time.timeScale = 1f;

        // 6) Ӧ�����Ա仯
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

        // ��ʾ����󣬵�1.5��������
        yield return new WaitForSecondsRealtime(1.5f);

        dialogueBox.SetActive(false);
        Time.timeScale = 1f;
        isHappening = false;
    }
}
