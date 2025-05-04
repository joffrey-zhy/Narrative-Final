using System.Collections;
using UnityEngine;
using UnityEngine.UI;            // ���� Image
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Fade-Out Settings")]
    [Tooltip("ȫ����ɫ Image�����ڳ����л�ʱ�ĵ���")]
    public GameObject fade;
    public Image fadeImage;
    [Tooltip("����ʱ�����룩")]
    public float fadeDuration = 1f;

    /// <summary>
    /// �ڡ���ʼ��Ϸ����ť�� OnClick() ����ô˷���
    /// </summary>
    private void Awake()
    {
        fade.SetActive(false);
    }
    public void OnStartButton()
    {
        fade.SetActive(true);
        // ��������� fadeImage�����ȵ������л�����
        if (fadeImage != null)
        {
            // ȷ��һ��ʼ͸��
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.gameObject.SetActive(true);
            StartCoroutine(FadeOutAndLoad("Prologue"));
        }
        else
        {
            // û������ֱ�Ӽ���
            SceneManager.LoadScene("Prologue");
        }
    }

    /// <summary>
    /// �ڡ��˳���Ϸ����ť�� OnClick() ����ô˷���
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
        // �� alpha=0 �� alpha=1
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float a = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }
        // ȷ����ȫ����
        fadeImage.color = new Color(c.r, c.g, c.b, 1f);
        SceneManager.LoadScene(sceneName);
    }
}
