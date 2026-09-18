using UnityEngine;

/// <summary>
/// Ambient sound zone - plays looping audio when player is inside.
/// Used for rain, wind, building interiors, etc.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AmbientZone : MonoBehaviour
{
    [SerializeField] private AudioClip ambientClip;
    [SerializeField] private float     fadeSpeed = 1.5f;
    [SerializeField] private float     maxVolume = 1f;

    private AudioSource audioSource;
    private bool playerInside = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip   = ambientClip;
        audioSource.loop   = true;
        audioSource.volume = 0f;
        audioSource.Play();
    }

    void Update()
    {
        float target = playerInside ? maxVolume : 0f;
        audioSource.volume = Mathf.MoveTowards(audioSource.volume, target, fadeSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInside = false;
    }
}
