using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Countdown timer UI for the 7-minute game clock.
/// </summary>
public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image           timerFill;
    [SerializeField] private Color           normalColor  = Color.white;
    [SerializeField] private Color           warningColor = Color.yellow;
    [SerializeField] private Color           dangerColor  = Color.red;
    [SerializeField] private float           warningThreshold = 120f;
    [SerializeField] private float           dangerThreshold  = 30f;

    private float totalTime = 420f;
    private bool pulseActive = false;
    private float pulseTimer = 0f;

    void Start()
    {
        if (GameManager.Instance != null)
            totalTime = GameManager.Instance.TotalTime;
    }

    public void UpdateTimer(float remainingSeconds)
    {
        int minutes = Mathf.FloorToInt(remainingSeconds / 60f);
        int seconds = Mathf.FloorToInt(remainingSeconds % 60f);
        timerText.text = string.Format("{0}:{1:D2}", minutes, seconds);

        // Fill bar
        if (timerFill != null)
            timerFill.fillAmount = remainingSeconds / totalTime;

        // Color
        if (remainingSeconds <= dangerThreshold)
        {
            timerText.color = dangerColor;
            if (timerFill != null) timerFill.color = dangerColor;
            PulseTimer();
        }
        else if (remainingSeconds <= warningThreshold)
        {
            timerText.color = warningColor;
            if (timerFill != null) timerFill.color = warningColor;
        }
        else
        {
            timerText.color = normalColor;
            if (timerFill != null) timerFill.color = normalColor;
        }
    }

    void PulseTimer()
    {
        pulseTimer += Time.deltaTime;
        float scale = 1f + Mathf.Sin(pulseTimer * Mathf.PI * 2f) * 0.05f;
        timerText.transform.localScale = Vector3.one * scale;
    }
}
