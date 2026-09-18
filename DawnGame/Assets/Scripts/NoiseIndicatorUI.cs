using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Noise indicator UI - shows current noise level as a radial meter.
/// </summary>
public class NoiseIndicatorUI : MonoBehaviour
{
    [SerializeField] private Image     noiseFill;
    [SerializeField] private Color     silentColor = Color.green;
    [SerializeField] private Color     loudColor   = Color.red;

    public void UpdateNoise(float normalizedLevel)
    {
        if (noiseFill == null) return;
        noiseFill.fillAmount = normalizedLevel;
        noiseFill.color = Color.Lerp(silentColor, loudColor, normalizedLevel);
    }
}
