using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoodSceneController : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("���������룯����Ч����ȫ����ɫ Image")]
    public Image fadeImage;
    [Tooltip("���룯����ʱ�����룩")]
    public float fadeDuration = 1f;

    void Start()
    {
        // ��������� fadeImage�����ȴӺ������룬�������������
        if (fadeImage != null)
        {
            // һ��ʼȫ��
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

        // ��ȫ͸��������
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
        fadeImage.gameObject.SetActive(false);
        // ��������󣬺��� Update/����������Ч
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
        // ����˳����õ������г���
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
        // ��͸����ʼ
        fadeImage.color = new Color(0f, 0f, 0f, 0f);

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeImage.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }

        // ȷ����ȫ����
        fadeImage.color = new Color(0f, 0f, 0f, 1f);
        SceneManager.LoadScene(sceneName);
    }
}
