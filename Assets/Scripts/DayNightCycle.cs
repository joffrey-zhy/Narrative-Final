using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using TMPro;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    [Header("ʱ������")]
    [Tooltip("����ʱ�����룩��Ĭ�� 5 ���� = 300 �룩")]
    public float dayDuration = 300f;
    [Tooltip("ѭ������������ã���Ϸ���ڣ�")]
    public int totalDays = 3;
    [Tooltip("��Ϸ��ʼʱ�䣬��Ӧ��ʵ�ӵ㣬�� t=0 ӳ�䵽���Сʱ")]
    [Range(0f, 24f)] public float startHour = 7f;

    [Header("��Դ����")]
    [Tooltip("�����е� Global Light 2D")]
    public Light2D globalLight;
    [Tooltip("ҹ��ʱ������è�ֲ���Դ����ѡ��")]
    public Light2D catLight;

    [Header("UI ����")]
    [Tooltip("��ʾ���ڼ���  HH:MM���� TextMeshProUGUI")]
    public TextMeshProUGUI DayLabel;
    public TextMeshProUGUI timeLabel;

    //���� ˽��״̬ ����//
    private float timeInDay = 0f;
    private int currentDay = 1;

    void Update()
    {
        // ���� 1. �ƽ���Ϸʱ�� ���� //
        timeInDay += Time.deltaTime;
        if (timeInDay >= dayDuration)
        {
            timeInDay -= dayDuration;
            currentDay++;
            if (currentDay > totalDays) currentDay = 1;
        }

        // ���� 2. ��һ�� t (0��1) ���� //
        float t = timeInDay / dayDuration;

        // ���� 3. ���㵱ǰ����ʵ��Сʱ (0-24) ���� //
        float hour = (startHour + t * 24f) % 24f;

        // ���� 4. ���㲢Ӧ��ȫ�ֹ���ǿ�� ���� //
        float intensity;
        if (hour >= 17f || hour < 2f)
        {
            // 17:00��24:00��(0��2:00)���� 9 Сʱ
            float hrsSince17 = hour >= 17f ? hour - 17f : hour + 24f - 17f;
            intensity = Mathf.Lerp(1f, 0f, hrsSince17 / 9f);
        }
        else if (hour >= 2f && hour < 12f)
        {
            // 2:00��12:00���� 10 Сʱ
            float hrsSince2 = hour - 2f;
            intensity = Mathf.Lerp(0f, 1f, hrsSince2 / 10f);
        }
        else
        {
            // 12:00��17:00 ��������
            intensity = 1f;
        }

        if (globalLight != null)
            globalLight.intensity = intensity;

        // ���� 5. ҹ��/��è�ֲ���Դ ���� //
        if (catLight != null)
            catLight.enabled = (hour >= 22f || hour < 4f);

        // ���� 6. ˢ�� UI ʱ����ʾ ���� //
        if (timeLabel != null)
        {
            int H = Mathf.FloorToInt(hour);
            int M = Mathf.FloorToInt((hour - H) * 60f);
            DayLabel.text = $"Day {currentDay}";
            timeLabel.text = $"{H:00}:{M:00}";
        }
    }
}
