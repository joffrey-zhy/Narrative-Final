using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathSceneController : MonoBehaviour
{
    [Header("被激活的 UI Canvas（开始时保持禁用）")]
    public Canvas targetCanvas;

    [Header("按键提示 UI 游戏对象")]
    public GameObject keyUIPrompt;

    [Header("Fade Settings")]
    [Tooltip("用于做淡入／淡出效果的全屏黑色 Image")]
    public Image fadeImage;
    [Tooltip("淡入／淡出时长（秒）")]
    public float fadeDuration = 1f;

    // 玩家是否在触发区内
    private bool playerInRange = false;

    private void Awake()
    {
        // 确保一开始目标 Canvas 是隐藏的
        if (targetCanvas != null)
            targetCanvas.gameObject.SetActive(false);
    }

    private void Start()
    {
        // 如果配置了 fadeImage，就先从黑屏淡入，再允许后续互动
        if (fadeImage != null)
        {
            // 一开始全黑
            fadeImage.color = Color.black;
            fadeImage.gameObject.SetActive(true);
            StartCoroutine(FadeInThenEnable());
        }
    }

    private IEnumerator FadeInThenEnable()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadeImage.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }

        // 完全透明并隐藏
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
        fadeImage.gameObject.SetActive(false);
        // 淡入结束后，后续 Update/触发检测才生效
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    private void Update()
    {
        // 玩家在范围内按 E，就打开 Canvas 并隐藏提示
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (targetCanvas != null)
                targetCanvas.gameObject.SetActive(true);
            if (keyUIPrompt != null)
                keyUIPrompt.SetActive(false);
        }
    }

    public void OnQuitButton()
    {
        // 点击退出调用淡出再切场景
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            StartCoroutine(FadeOutAndLoad("MainMenu"));
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        float timer = 0f;
        // 从透明开始
        fadeImage.color = new Color(0f, 0f, 0f, 0f);

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeImage.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }

        // 确保完全黑屏
        fadeImage.color = new Color(0f, 0f, 0f, 1f);
        SceneManager.LoadScene(sceneName);
    }
}
