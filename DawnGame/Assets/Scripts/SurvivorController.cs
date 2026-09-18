using UnityEngine;

/// <summary>
/// Survivor NPC at the Hospital - player chooses to rescue or leave.
/// </summary>
public class SurvivorController : MonoBehaviour, IInteractable
{
    [SerializeField] private string approachPrompt = "Talk to Survivor [E]";
    [SerializeField] private SurvivorChoiceUI choiceUI;
    [SerializeField] private Animator survivorAnimator;

    private bool choiceMade = false;
    private bool isFollowing = false;

    [Header("Follow Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float followSpeed = 2.8f;
    [SerializeField] private float followDistance = 2f;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (!isFollowing || player == null) return;
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > followDistance)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * followSpeed * Time.deltaTime;
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            survivorAnimator?.SetBool("IsWalking", true);
        }
        else
        {
            survivorAnimator?.SetBool("IsWalking", false);
        }
    }

    public void Interact(PlayerController player)
    {
        if (choiceMade) return;
        if (GameManager.Instance.CurrentPhase != GamePhase.Hospital) return;
        choiceUI?.ShowChoice(OnRescueChosen, OnLeaveChosen);
    }

    void OnRescueChosen()
    {
        choiceMade = true;
        isFollowing = true;
        survivorAnimator?.SetTrigger("Stand");
        GameManager.Instance?.OnSurvivorChoiceMade(true);
    }

    void OnLeaveChosen()
    {
        choiceMade = true;
        GameManager.Instance?.OnSurvivorChoiceMade(false);
    }

    public string GetPrompt()
    {
        if (choiceMade) return "";
        return (GameManager.Instance?.CurrentPhase == GamePhase.Hospital) ? approachPrompt : "";
    }
}
