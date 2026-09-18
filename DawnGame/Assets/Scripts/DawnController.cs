using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

/// <summary>
/// Dawn Controller - drives the dawn skybox transition, post-processing,
/// and directional light color change for the ending sequence.
/// </summary>
public class DawnController : MonoBehaviour
{
    [Header("Skybox Transition")]
    [SerializeField] private Material nightSkybox;
    [SerializeField] private Material dawnSkybox;
    [SerializeField] private float transitionDuration = 6f;

    [Header("Directional Light")]
    [SerializeField] private Light sunLight;
    [SerializeField] private Color nightColor  = new Color(0.1f, 0.1f, 0.3f);
    [SerializeField] private Color dawnColor   = new Color(1f, 0.6f, 0.3f);
    [SerializeField] private float nightIntensity = 0.2f;
    [SerializeField] private float dawnIntensity  = 1.5f;

    [Header("Post Processing")]
    [SerializeField] private Volume postVolume;
    private ColorAdjustments colorAdj;

    [Header("Audio")]
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioClip   dawnAmbienceClip;

    public void PlayDawnSequence()
    {
        StartCoroutine(DawnTransition());
    }

    IEnumerator DawnTransition()
    {
        // Play dawn ambience
        if (ambienceSource != null && dawnAmbienceClip != null)
        {
            ambienceSource.clip = dawnAmbienceClip;
            ambienceSource.Play();
        }

        // Post processing color grading
        if (postVolume != null)
            postVolume.profile.TryGet(out colorAdj);

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;

            // Blend skybox exposure
            if (nightSkybox != null && dawnSkybox != null)
                RenderSettings.skybox.Lerp(nightSkybox, dawnSkybox, t);

            // Light color and intensity
            if (sunLight != null)
            {
                sunLight.color     = Color.Lerp(nightColor, dawnColor, t);
                sunLight.intensity = Mathf.Lerp(nightIntensity, dawnIntensity, t);
            }

            // Color adjustments
            if (colorAdj != null)
            {
                colorAdj.saturation.value = Mathf.Lerp(-30f, 10f, t);
                colorAdj.postExposure.value = Mathf.Lerp(-1f, 0.5f, t);
            }

            DynamicGI.UpdateEnvironment();
            yield return null;
        }
    }
}
