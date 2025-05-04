using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class AdoptionNPC : MonoBehaviour
{
    [Header("�Ի� UI")]
    [Tooltip("�Ի� Canvas������������TextMeshProUGUI������ʼ��Ϊ inactive��")]
    public GameObject dialogueCanvas;
    [Tooltip("�Ի��ı����")]
    public TMP_Text dialogueText;

    [Header("�Ի�����")]
    [TextArea] public string notReadyMessage = "��ʱ��û��������ʱ��";
    [TextArea] public string goodEndingMessage = "�����ڱ��������ˣ�";

    [Header("�ý�ֳ�����")]
    public string goodEndingScene = "GoodEndingScene";

    [Header("���ֻ�����")]
    [Tooltip("���ִ�ӡ�ٶȣ���/�֣���ʹ�� UnscaledTime")]
    public float typeSpeed = 0.02f;

    [Header("��������")]
    public KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;
    private bool isTalking = false;
    private Cat cat;

    [System.Obsolete]
    void Awake()
    {
        cat = FindObjectOfType<Cat>();
        if (dialogueCanvas != null)
            dialogueCanvas.SetActive(false);
        else
            Debug.LogWarning("AdoptionNPC: ��� dialogueCanvas");
        if (dialogueText == null)
            Debug.LogWarning("AdoptionNPC: ��� dialogueText");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Cat>() != null)
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Cat>() != null)
        {
            playerInRange = false;
            if (!isTalking && dialogueCanvas != null)
                dialogueCanvas.SetActive(false);
        }
    }

    void Update()
    {
        if (!playerInRange || isTalking || cat == null)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            if (cat.hunger > 0f || cat.thirst > 0f)
                StartCoroutine(DialogueRoutine(notReadyMessage, false));
            else
                StartCoroutine(DialogueRoutine(goodEndingMessage, true));
        }
    }

    private IEnumerator DialogueRoutine(string message, bool isGoodEnding)
    {
        isTalking = true;
        Time.timeScale = 0f;  // ��ͣ��Ϸ

        dialogueCanvas.SetActive(true);
        dialogueText.text = "";

        // ���ִ�ӡ��ʹ��δ����ʱ�䣬���� timeScale Ӱ�죩
        foreach (char c in message)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typeSpeed);
        }

        // ����󣬵ȴ���Ұ� Enter �رգ�����
        yield return new WaitUntil(() =>
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter)
        );

        dialogueCanvas.SetActive(false);
        Time.timeScale = 1f;  // �ָ���Ϸʱ��
        isTalking = false;

        if (isGoodEnding)
        {
            SceneManager.LoadScene(goodEndingScene);
        }
    }
}
