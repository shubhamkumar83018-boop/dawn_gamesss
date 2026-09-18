using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls the Hunter AI - patrols, investigates noise, pursues player.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class HunterController : MonoBehaviour
{
    public static HunterController Instance { get; private set; }

    [Header("State")]
    public HunterState State = HunterState.Patrol;

    [Header("Detection")]
    [SerializeField] private float sightRange = 15f;
    [SerializeField] private float sightAngle = 70f;
    [SerializeField] private float hearingRange = 12f;
    [SerializeField] private float losePlayerTime = 5f;
    [SerializeField] private LayerMask sightMask;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2.5f;
    [SerializeField] private float investigateSpeed = 3.5f;
    [SerializeField] private float chaseSpeed = 6f;
    [SerializeField] private float finalChaseSpeed = 8f;
    [SerializeField] private Transform[] patrolPoints;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private AudioClip alertClip;
    [SerializeField] private AudioClip investigateClip;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;

    private NavMeshAgent agent;
    private int patrolIndex = 0;
    private float loseTimer = 0f;
    private Vector3 lastNoisePosition;
    private bool hasNoiseTarget = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        agent = GetComponent<NavMeshAgent>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
        agent.speed = patrolSpeed;
        SetState(HunterState.Patrol);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.GameOver) return;

        switch (State)
        {
            case HunterState.Patrol:     UpdatePatrol(); break;
            case HunterState.Investigate: UpdateInvestigate(); break;
            case HunterState.Chase:      UpdateChase(); break;
            case HunterState.FinalChase: UpdateFinalChase(); break;
        }

        CheckSight();
    }

    void UpdatePatrol()
    {
        if (patrolPoints.Length == 0) return;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }

    void UpdateInvestigate()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Reached noise point - look around then resume patrol
            loseTimer += Time.deltaTime;
            if (loseTimer > 4f)
            {
                loseTimer = 0f;
                hasNoiseTarget = false;
                SetState(HunterState.Patrol);
                GameManager.Instance?.BeginPhase(GamePhase.ExploreCampus);
            }
        }
    }

    void UpdateChase()
    {
        if (player == null) return;
        agent.SetDestination(player.position);

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist < 1.5f)
        {
            CatchPlayer();
            return;
        }

        if (!CanSeePlayer())
        {
            loseTimer += Time.deltaTime;
            if (loseTimer >= losePlayerTime)
            {
                loseTimer = 0f;
                SetState(HunterState.Investigate);
                lastNoisePosition = player.position;
                agent.SetDestination(lastNoisePosition);
            }
        }
        else
        {
            loseTimer = 0f;
        }
    }

    void UpdateFinalChase()
    {
        if (player == null) return;
        agent.SetDestination(player.position);
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist < 1.5f) CatchPlayer();
    }

    void CheckSight()
    {
        if (player == null || State == HunterState.FinalChase) return;
        if (CanSeePlayer() && State != HunterState.Chase)
        {
            SetState(HunterState.Chase);
            PlayAudio(alertClip);
            GameManager.Instance?.BeginPhase(GamePhase.HunterInvestigation);
            NoiseSystem.Instance?.SetHunterAlert(true);
        }
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;
        Vector3 dir = player.position - transform.position;
        float dist = dir.magnitude;
        if (dist > sightRange) return false;
        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > sightAngle) return false;
        if (Physics.Raycast(transform.position + Vector3.up, dir.normalized, dist, sightMask)) return false;
        return true;
    }

    public void InvestigateNoise(Vector3 position, float noiseLevel)
    {
        if (State == HunterState.Chase || State == HunterState.FinalChase) return;
        float dist = Vector3.Distance(transform.position, position);
        if (dist <= hearingRange * noiseLevel)
        {
            lastNoisePosition = position;
            hasNoiseTarget = true;
            SetState(HunterState.Investigate);
            agent.SetDestination(lastNoisePosition);
            PlayAudio(investigateClip);
            GameManager.Instance?.BeginPhase(GamePhase.HunterInvestigation);
        }
    }

    public void TriggerFinalChase()
    {
        SetState(HunterState.FinalChase);
        agent.speed = finalChaseSpeed;
    }

    void SetState(HunterState newState)
    {
        State = newState;
        switch (newState)
        {
            case HunterState.Patrol:     agent.speed = patrolSpeed; break;
            case HunterState.Investigate: agent.speed = investigateSpeed; break;
            case HunterState.Chase:      agent.speed = chaseSpeed; break;
            case HunterState.FinalChase: agent.speed = finalChaseSpeed; break;
        }
        animator?.SetInteger("HunterState", (int)newState);
    }

    void PlayAudio(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void CatchPlayer()
    {
        GameManager.Instance?.TriggerBadEnding();
    }
}

public enum HunterState
{
    Patrol = 0,
    Investigate = 1,
    Chase = 2,
    FinalChase = 3
}
