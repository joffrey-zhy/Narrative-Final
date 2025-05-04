using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class AdoptionNPC : MonoBehaviour
{
    [Header("对话 UI")]
    [Tooltip("对话 Canvas（包含背景、TextMeshProUGUI），初始设为 inactive）")]
    public GameObject dialogueCanvas;
    [Tooltip("对话文本组件")]
    public TMP_Text dialogueText;

    [Header("对话内容")]
    [TextArea] public string notReadyMessage = "暂时还没到收养的时候。";
    [TextArea] public string goodEndingMessage = "你终于被我收养了！";

    [Header("好结局场景名")]
    public string goodEndingScene = "GoodEndingScene";

    [Header("打字机设置")]
    [Tooltip("逐字打印速度（秒/字），使用 UnscaledTime")]
    public float typeSpeed = 0.02f;

    [Header("交互按键")]
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
            Debug.LogWarning("AdoptionNPC: 请绑定 dialogueCanvas");
        if (dialogueText == null)
            Debug.LogWarning("AdoptionNPC: 请绑定 dialogueText");
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
        Time.timeScale = 0f;  // 暂停游戏

        dialogueCanvas.SetActive(true);
        dialogueText.text = "";

        // 逐字打印（使用未缩放时间，不受 timeScale 影响）
        foreach (char c in message)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typeSpeed);
        }

        // 打完后，等待玩家按 Enter 关闭／继续
        yield return new WaitUntil(() =>
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter)
        );

        dialogueCanvas.SetActive(false);
        Time.timeScale = 1f;  // 恢复游戏时间
        isTalking = false;

        if (isGoodEnding)
        {
            SceneManager.LoadScene(goodEndingScene);
        }
    }
}
