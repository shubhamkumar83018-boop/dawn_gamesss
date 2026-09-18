using UnityEngine;

/// <summary>
/// 7 Minutes Before Dawn - Game Manager
/// Controls the full gameplay loop from 7:00 timer through ending and score.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public GamePhase CurrentPhase = GamePhase.RadioTransmission;
    public float TotalTime = 420f; // 7 minutes in seconds
    public float RemainingTime;
    public bool GameOver = false;
    public bool PlayerEscaped = false;

    [Header("Phase Flags")]
    public bool FuseFound = false;
    public bool GeneratorActivated = false;
    public bool SurvivorRescued = false;
    public bool RadioTowerReached = false;
    public bool HunterAware = false;

    [Header("Score")]
    public int Score = 0;
    public bool SurvivorSaved = false;
    public float TimeBonus = 0f;

    [Header("UI References")]
    [SerializeField] private TimerUI timerUI;
    [SerializeField] private PhaseUI phaseUI;
    [SerializeField] private EndScreen endScreen;

    [Header("Audio")]
    [SerializeField] private AudioSource radioSource;
    [SerializeField] private AudioClip radioTransmissionClip;
    [SerializeField] private AudioClip radioUpdateClip;
    [SerializeField] private AudioClip evacuationClip;

    [Header("Phase Transition")]
    [SerializeField] private float finalChaseThreshold = 30f;

    private bool finalChaseTriggered = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        RemainingTime = TotalTime;
        BeginPhase(GamePhase.RadioTransmission);
    }

    void Update()
    {
        if (GameOver) return;

        RemainingTime -= Time.deltaTime;
        timerUI?.UpdateTimer(RemainingTime);

        // Final 30-second chase trigger
        if (!finalChaseTriggered && RemainingTime <= finalChaseThreshold)
        {
            finalChaseTriggered = true;
            BeginPhase(GamePhase.FinalChase);
        }

        // Time's up - Hunter catches player
        if (RemainingTime <= 0f)
        {
            RemainingTime = 0f;
            TriggerBadEnding();
        }
    }

    public void BeginPhase(GamePhase phase)
    {
        CurrentPhase = phase;
        phaseUI?.ShowPhase(phase);

        switch (phase)
        {
            case GamePhase.RadioTransmission:
                PlayRadio(radioTransmissionClip);
                break;
            case GamePhase.RadioUpdate:
                PlayRadio(radioUpdateClip);
                break;
            case GamePhase.EvacuationRadio:
                PlayRadio(evacuationClip);
                break;
            case GamePhase.FinalChase:
                HunterController hunter = FindObjectOfType<HunterController>();
                hunter?.TriggerFinalChase();
                NoiseSystem.Instance?.SetChaseMode(true);
                break;
            case GamePhase.Dawn:
                TriggerDawn();
                break;
        }
    }

    void PlayRadio(AudioClip clip)
    {
        if (radioSource != null && clip != null)
        {
            radioSource.clip = clip;
            radioSource.Play();
        }
    }

    public void OnFuseFound()
    {
        FuseFound = true;
        Score += 100;
        BeginPhase(GamePhase.ExploreCampus);
    }

    public void OnGeneratorActivated()
    {
        GeneratorActivated = true;
        Score += 150;
        BeginPhase(GamePhase.RadioUpdate);
    }

    public void OnSurvivorChoiceMade(bool rescued)
    {
        SurvivorRescued = rescued;
        SurvivorSaved = rescued;
        Score += rescued ? 200 : 50;
        BeginPhase(GamePhase.EvacuationRadio);
    }

    public void OnRadioTowerReached()
    {
        RadioTowerReached = true;
        Score += 250;

        if (RemainingTime > finalChaseThreshold)
        {
            BeginPhase(GamePhase.FinalChase);
        }
    }

    public void OnPlayerEscaped()
    {
        PlayerEscaped = true;
        TimeBonus = Mathf.FloorToInt(RemainingTime) * 10;
        Score += (int)TimeBonus;
        BeginPhase(GamePhase.Dawn);
    }

    void TriggerDawn()
    {
        GameOver = true;
        endScreen?.ShowEnding(true, Score, SurvivorSaved, RemainingTime);
        DawnController dc = FindObjectOfType<DawnController>();
        dc?.PlayDawnSequence();
    }

    public void TriggerBadEnding()
    {
        GameOver = true;
        endScreen?.ShowEnding(false, Score, SurvivorSaved, 0f);
    }
}

public enum GamePhase
{
    RadioTransmission,
    FindGeneratorFuse,
    ExploreCampus,
    NoiseInvestigation,
    HunterInvestigation,
    Generator,
    RadioUpdate,
    Hospital,
    SurvivorChoice,
    EvacuationRadio,
    RadioTower,
    FinalChase,
    Dawn,
    Ending
}
