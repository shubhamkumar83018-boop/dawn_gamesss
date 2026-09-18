using UnityEngine;

/// <summary>
/// Generator interactable - requires the fuse before it can be activated.
/// </summary>
public class Generator : MonoBehaviour, IInteractable
{
    [SerializeField] private string needFusePrompt  = "Need Generator Fuse";
    [SerializeField] private string activatePrompt  = "Activate Generator [E]";
    [SerializeField] private string activePrompt    = "Generator Running";

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip   startupClip;
    [SerializeField] private Light[]     powerLights;
    [SerializeField] private ParticleSystem sparks;

    private bool activated = false;

    public void Interact(PlayerController player)
    {
        if (activated) return;
        if (!GameManager.Instance.FuseFound)
        {
            InteractPromptUI.Instance?.ShowMessage(needFusePrompt, 2f);
            return;
        }

        activated = true;

        // Visual / audio feedback
        audioSource?.PlayOneShot(startupClip);
        sparks?.Play();
        foreach (Light l in powerLights) if (l != null) l.enabled = true;

        // Advance game
        GameManager.Instance?.OnGeneratorActivated();
        GameManager.Instance?.BeginPhase(GamePhase.Generator);
    }

    public string GetPrompt()
    {
        if (activated) return activePrompt;
        return GameManager.Instance.FuseFound ? activatePrompt : needFusePrompt;
    }
}
