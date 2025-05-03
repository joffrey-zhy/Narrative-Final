using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class TriggerDialogue : MonoBehaviour
{
    [Header("�Ի�����")]
    [TextArea, Tooltip("�����Ի�����")]
    public string fullText = "��ã�����һ����ʾ��";
    [Tooltip("���ִ�ӡ�ٶȣ���/�֣�")]
    public float typeSpeed = 0.05f;
    [Tooltip("�Ի���ȫ��ӡ���Զ���ʧ���ӳ٣��룩")]
    public float disappearDelay = 5f;
    [Tooltip("������ȴ����Ϸ��Сʱ�������� 12 Сʱ��ſ��ٴδ���")]
    public float cooldownHours = 12f;
    [Tooltip("����ռ� UI Ԥ���壬����� TextMeshProUGUI ������")]
    public GameObject dialogueUIPrefab;
    [Tooltip("�Ի�����������ƫ����")]
    public Vector3 uiOffset = new Vector3(0, 2f, 0);
    [Header("UI ���ڵ㣨�� Inspector �����룩")]
    public Transform uiParent;
    [Header("��ʾͼ������")]
    [Tooltip("��ҽ��뷶Χʱ��ʾ����ʾͼ�� Prefab")]
    public GameObject promptIconPrefab;
    [Tooltip("��ʾͼ����������ƫ����")]
    public Vector3 promptIconOffset = new Vector3(0, 3f, 0);

    // ���� internal state ���� //
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
        // ȷ�� Collider2D �Ǵ�����
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        // �õ� DayNightCycle��������ȴ���
        dayNightCycle = FindObjectOfType<DayNightCycle>();

        /*// ���� UI ���ڵ㣬���û�о͹ҵ���
        if (transform.parent != null && transform.parent.parent != null)
            uiParent = transform.parent.parent;
        else
            uiParent = null;*/
    }

    void Update()
    {
        // �����ʾͼ����ڣ���ʵʱ��������λ��
        if (promptIconInstance != null)
        {
            Vector3 worldPos = transform.position + promptIconOffset;
            promptIconInstance.transform.position = worldPos;
        }

        // ����Ի���򿪣�Ҳ���԰���ʵʱ���棨��ѡ��
        if (dialogueInstance != null)
        {
            Vector3 dialogPos = transform.position + uiOffset;
            dialogueInstance.transform.position = dialogPos;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 1. ��ʾ��ʾͼ��
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

        // 2. ��ȴ��� & ���Ի�
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

        // ���ز�������ʾͼ��
        if (promptIconInstance != null)
        {
            Destroy(promptIconInstance);
            promptIconInstance = null;
        }

        // ����ѡ���뿪ʱҲ���ٶԻ�
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
            Debug.LogError("TriggerDialogue: Prefab ��ȱ�� TextMeshProUGUI �����");
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
