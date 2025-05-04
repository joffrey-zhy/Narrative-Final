using System.Collections;
using UnityEngine;
using UnityEngine.UI;            // 用于 Image
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Fade-Out Settings")]
    [Tooltip("全屏黑色 Image，用于场景切换时的淡出")]
    public GameObject fade;
    public Image fadeImage;
    [Tooltip("淡出时长（秒）")]
    public float fadeDuration = 1f;

    /// <summary>
    /// 在“开始游戏”按钮的 OnClick() 里调用此方法
    /// </summary>
    private void Awake()
    {
        fade.SetActive(false);
    }
    public void OnStartButton()
    {
        fade.SetActive(true);
        // 如果配置了 fadeImage，就先淡出再切换场景
        if (fadeImage != null)
        {
            // 确保一开始透明
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.gameObject.SetActive(true);
            StartCoroutine(FadeOutAndLoad("Prologue"));
        }
        else
        {
            // 没配置则直接加载
            SceneManager.LoadScene("Prologue");
        }
    }

    /// <summary>
    /// 在“退出游戏”按钮的 OnClick() 里调用此方法
    /// </summary>
    public void OnQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        float timer = 0f;
        Color c = fadeImage.color;
        // 从 alpha=0 → alpha=1
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float a = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }
        // 确保完全黑屏
        fadeImage.color = new Color(c.r, c.g, c.b, 1f);
        SceneManager.LoadScene(sceneName);
    }
}
