using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using TMPro;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    [Header("时间设置")]
    [Tooltip("单日时长（秒），默认 5 分钟 = 300 秒）")]
    public float dayDuration = 300f;
    [Tooltip("循环多少天后重置（游戏周期）")]
    public int totalDays = 3;
    [Tooltip("游戏开始时间，对应真实钟点，将 t=0 映射到这个小时")]
    [Range(0f, 24f)] public float startHour = 7f;

    [Header("光源引用")]
    [Tooltip("场景中的 Global Light 2D")]
    public Light2D globalLight;
    [Tooltip("夜晚时点亮的猫局部光源（可选）")]
    public Light2D catLight;

    [Header("UI 引用")]
    [Tooltip("显示“第几天  HH:MM”的 TextMeshProUGUI")]
    public TextMeshProUGUI DayLabel;
    public TextMeshProUGUI timeLabel;

    //—— 私有状态 ——//
    private float timeInDay = 0f;
    private int currentDay = 1;

    void Update()
    {
        // —— 1. 推进游戏时钟 —— //
        timeInDay += Time.deltaTime;
        if (timeInDay >= dayDuration)
        {
            timeInDay -= dayDuration;
            currentDay++;
            if (currentDay > totalDays) currentDay = 1;
        }

        // —— 2. 归一化 t (0→1) —— //
        float t = timeInDay / dayDuration;

        // —— 3. 计算当前“真实”小时 (0-24) —— //
        float hour = (startHour + t * 24f) % 24f;

        // —— 4. 计算并应用全局光照强度 —— //
        float intensity;
        if (hour >= 17f || hour < 2f)
        {
            // 17:00→24:00→(0→2:00)，共 9 小时
            float hrsSince17 = hour >= 17f ? hour - 17f : hour + 24f - 17f;
            intensity = Mathf.Lerp(1f, 0f, hrsSince17 / 9f);
        }
        else if (hour >= 2f && hour < 12f)
        {
            // 2:00→12:00，共 10 小时
            float hrsSince2 = hour - 2f;
            intensity = Mathf.Lerp(0f, 1f, hrsSince2 / 10f);
        }
        else
        {
            // 12:00→17:00 保持最亮
            intensity = 1f;
        }

        if (globalLight != null)
            globalLight.intensity = intensity;

        // —— 5. 夜晚开/关猫局部光源 —— //
        if (catLight != null)
            catLight.enabled = (hour >= 22f || hour < 4f);

        // —— 6. 刷新 UI 时间显示 —— //
        if (timeLabel != null)
        {
            int H = Mathf.FloorToInt(hour);
            int M = Mathf.FloorToInt((hour - H) * 60f);
            DayLabel.text = $"Day {currentDay}";
            timeLabel.text = $"{H:00}:{M:00}";
        }
    }
}
