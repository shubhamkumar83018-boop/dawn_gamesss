using UnityEngine;

/// <summary>
/// Noise system - player actions generate noise that the Hunter can hear.
/// </summary>
public class NoiseSystem : MonoBehaviour
{
    public static NoiseSystem Instance { get; private set; }

    [Header("Noise Levels (0-1)")]
    [SerializeField] private float walkNoise = 0.3f;
    [SerializeField] private float runNoise = 0.8f;
    [SerializeField] private float crouchNoise = 0.1f;
    [SerializeField] private float interactNoise = 0.6f;

    [Header("State")]
    public bool IsChaseMode = false;
    public bool HunterAlerted = false;

    [Header("Noise Indicator UI")]
    [SerializeField] private NoiseIndicatorUI noiseIndicatorUI;

    private float currentNoiseLevel = 0f;
    private float noiseFadeRate = 2f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        // Fade noise over time
        currentNoiseLevel = Mathf.MoveTowards(currentNoiseLevel, 0f, noiseFadeRate * Time.deltaTime);
        noiseIndicatorUI?.UpdateNoise(currentNoiseLevel);
    }

    public void MakeNoise(NoiseType type, Vector3 position)
    {
        float level = GetNoiseLevelForType(type);
        currentNoiseLevel = Mathf.Max(currentNoiseLevel, level);

        // Notify Hunter
        HunterController hunter = HunterController.Instance;
        if (hunter != null)
        {
            hunter.InvestigateNoise(position, level);
        }
    }

    float GetNoiseLevelForType(NoiseType type)
    {
        switch (type)
        {
            case NoiseType.Walk:     return walkNoise;
            case NoiseType.Run:      return runNoise;
            case NoiseType.Crouch:   return crouchNoise;
            case NoiseType.Interact: return interactNoise;
            default:                 return 0.5f;
        }
    }

    public void SetChaseMode(bool active)
    {
        IsChaseMode = active;
    }

    public void SetHunterAlert(bool alerted)
    {
        HunterAlerted = alerted;
    }
}

public enum NoiseType
{
    Walk,
    Run,
    Crouch,
    Interact,
    Ambient
}
