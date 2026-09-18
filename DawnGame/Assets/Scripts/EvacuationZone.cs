using UnityEngine;

/// <summary>
/// Evacuation Zone trigger - player reaches this after the radio tower
/// to escape and trigger the Dawn ending.
/// </summary>
public class EvacuationZone : MonoBehaviour
{
    [SerializeField] private ParticleSystem dawningVFX;
    [SerializeField] private Light evacuationLight;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance == null || GameManager.Instance.GameOver) return;
        if (!GameManager.Instance.RadioTowerReached) return;

        dawningVFX?.Play();
        if (evacuationLight != null) evacuationLight.intensity = 5f;

        GameManager.Instance.OnPlayerEscaped();
    }
}
