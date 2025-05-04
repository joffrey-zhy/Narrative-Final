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

    [Header("Continue Prompt Settings")]
    public TMP_Text continuePromptText;
    public string continuePrompt = "Press Enter to continue...";
    public float promptTypeSpeed = 0.03f;

    [Header("Audio Settings")]
    [Tooltip("循环播放的打字音效")]
    public AudioClip typingClip;
    [Tooltip("打字音效音量")]
    [Range(0f, 1f)] public float typingVolume = 1f;

    private AudioSource audioSource;
    private bool openingFinished = false;
    private bool promptFinished = false;
    private bool transitioning = false;  // 防止重复触发

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
        // 准备文本
        if (openingText != null) openingText.text = "";
        if (continuePromptText != null) continuePromptText.text = "";

        // 如果有 fadeImage，先做淡入，否则直接开始打字
        if (fadeImage != null)
        {
            fadeImage.color = Color.black;
            fadeImage.gameObject.SetActive(true);
            StartCoroutine(FadeInThenStart());
        }
        else
        {
            StartCoroutine(TypeOpening());
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

        StartCoroutine(TypeOpening());
    }

    void Update()
    {
        if (openingFinished && promptFinished && !transitioning)
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

    private IEnumerator TypeOpening()
    {
        if (openingText == null) yield break;

        // 开始打字时启动循环音效
        if (typingClip != null)
            audioSource.Play();

        foreach (char c in fullOpeningText)
        {
            openingText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        // 打字结束时停止音效
        if (typingClip != null)
            audioSource.Stop();

        openingFinished = true;
        StartCoroutine(TypePrompt());
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
}
