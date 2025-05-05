using System.Collections;
using UnityEngine;
using UnityEngine.UI;            // ���� Image
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class OpeningTypewriter : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("���������룯����Ч����ȫ����ɫ Image")]
    public Image fadeImage;
    [Tooltip("���뵭��ʱ�����룩")]
    public float fadeDuration = 1f;

    [Header("Opening Text Settings")]
    public TMP_Text openingText;
    [TextArea]
    public string fullOpeningText = "";
    public float typeSpeed = 0.05f;

    [Header("Skip Prompt Settings")]
    [Tooltip("����δ���ʱ��������ʾ")]
    public TMP_Text continuePromptText;
    public string skipPrompt = "Press Enter to skip";

    [Header("Continue Prompt Settings")]
    [Tooltip("������ɺ�ļ�����ʾ")]
    public string continuePrompt = "Press Enter to continue...";
    public float promptTypeSpeed = 0.03f;

    [Header("Audio Settings")]
    [Tooltip("ѭ�����ŵĴ�����Ч")]
    public AudioClip typingClip;
    [Tooltip("������Ч����")]
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
        // ׼�� AudioSource
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = typingClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = typingVolume;
    }

    void Start()
    {
        // ��ʼ���ı�
        if (openingText != null) openingText.text = "";
        if (continuePromptText != null) continuePromptText.text = "";

        // �����ʼ���֣���ֱ�ӿ�ʼ
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
        // ���ֹ����У���������
        if (!openingFinished)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                skipRequested = true;
            }
        }
        // ����+��ʾ����ɺ󣬼���������һ����
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

        // ��ʾ������ʾ
        if (continuePromptText != null)
            continuePromptText.text = skipPrompt;

        // ��ʼ������Ч
        if (typingClip != null)
            audioSource.Play();

        // ���ִ�ӡ����ӦskipRequested
        foreach (char c in fullOpeningText)
        {
            if (skipRequested) break;
            openingText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        // ȷ��������ʾ
        openingText.text = fullOpeningText;
        if (typingClip != null)
            audioSource.Stop();

        openingFinished = true;

        // ���������������������ʾ����������ʾ��������
        if (skipRequested)
        {
            if (continuePromptText != null)
                continuePromptText.text = continuePrompt;
            promptFinished = true;
        }
        else
        {
            // �������ֽ�������վ���ʾ�������ִ�ӡ��Press Enter to continue...��
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
