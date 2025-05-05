using System.Collections;
using UnityEngine;
using UnityEngine.UI;            // 用于 Image
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class OpeningTypewriter : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("用于做淡入／淡出效果的全屏黑色 Image")]
    public Image fadeImage;
    [Tooltip("淡入淡出时长（秒）")]
    public float fadeDuration = 1f;

    [Header("Opening Text Settings")]
    public TMP_Text openingText;
    [TextArea]
    public string fullOpeningText = "";
    public float typeSpeed = 0.05f;

    [Header("Skip Prompt Settings")]
    [Tooltip("打字未完成时的跳过提示")]
    public TMP_Text continuePromptText;
    public string skipPrompt = "Press Enter to skip";

    [Header("Continue Prompt Settings")]
    [Tooltip("打字完成后的继续提示")]
    public string continuePrompt = "Press Enter to continue...";
    public float promptTypeSpeed = 0.03f;

    [Header("Audio Settings")]
    [Tooltip("循环播放的打字音效")]
    public AudioClip typingClip;
    [Tooltip("打字音效音量")]
    [Range(0f, 1f)]
    public float typingVolume = 1f;

    private AudioSource audioSource;
    private bool openingFinished = false;
    private bool promptFinished = false;
    private bool skipRequested = false;
    private bool transitioning = false;
    private Coroutine typingCoroutine;

    void Awake()
    {
        // 准备 AudioSource
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = typingClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = typingVolume;
    }

    void Start()
    {
        // 初始化文本
        if (openingText != null) openingText.text = "";
        if (continuePromptText != null) continuePromptText.text = "";

        // 淡入后开始打字，或直接开始
        if (fadeImage != null)
        {
            fadeImage.color = Color.black;
            fadeImage.gameObject.SetActive(true);
            StartCoroutine(FadeInThenStart());
        }
        else
        {
            typingCoroutine = StartCoroutine(TypeOpening());
        }
    }

    private IEnumerator FadeInThenStart()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadeImage.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
        fadeImage.gameObject.SetActive(false);

        typingCoroutine = StartCoroutine(TypeOpening());
    }

    void Update()
    {
        // 打字过程中，监听跳过
        if (!openingFinished)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                skipRequested = true;
            }
        }
        // 打字+提示都完成后，监听进入下一场景
        else if (openingFinished && promptFinished && !transitioning)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                transitioning = true;
                if (fadeImage != null)
                {
                    fadeImage.gameObject.SetActive(true);
                    StartCoroutine(FadeOutAndLoad("HeyiScene"));
                }
                else
                {
                    SceneManager.LoadScene("HeyiScene");
                }
            }
        }
    }

    private IEnumerator TypeOpening()
    {
        if (openingText == null) yield break;

        // 显示跳过提示
        if (continuePromptText != null)
            continuePromptText.text = skipPrompt;

        // 开始打字音效
        if (typingClip != null)
            audioSource.Play();

        // 逐字打印，响应skipRequested
        foreach (char c in fullOpeningText)
        {
            if (skipRequested) break;
            openingText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        // 确保完整显示
        openingText.text = fullOpeningText;
        if (typingClip != null)
            audioSource.Stop();

        openingFinished = true;

        // 如果是跳过而来，立即显示“继续”提示并标记完成
        if (skipRequested)
        {
            if (continuePromptText != null)
                continuePromptText.text = continuePrompt;
            promptFinished = true;
        }
        else
        {
            // 正常打字结束后，清空旧提示，再逐字打印“Press Enter to continue...”
            if (continuePromptText != null)
                continuePromptText.text = "";
            StartCoroutine(TypePrompt());
        }
    }

    private IEnumerator TypePrompt()
    {
        if (continuePromptText == null) yield break;
        yield return new WaitForSeconds(0.5f);
        foreach (char c in continuePrompt)
        {
            continuePromptText.text += c;
            yield return new WaitForSeconds(promptTypeSpeed);
        }
        promptFinished = true;
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        float timer = 0f;
        fadeImage.color = new Color(0f, 0f, 0f, 0f);

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeImage.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }
        fadeImage.color = new Color(0f, 0f, 0f, 1f);

        SceneManager.LoadScene(sceneName);
    }
}
