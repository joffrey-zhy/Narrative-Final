using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoodSceneController : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("用于做淡入／淡出效果的全屏黑色 Image")]
    public Image fadeImage;
    [Tooltip("淡入／淡出时长（秒）")]
    public float fadeDuration = 1f;

    void Start()
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
    public void OnQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void OnMainMenuButton()
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
