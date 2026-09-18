using UnityEngine;

/// <summary>
/// Radio Tower trigger - player reaches it to begin final 30-second chase
/// and trigger the evacuation radio broadcast.
/// </summary>
public class RadioTower : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt = "Activate Radio Tower [E]";
    [SerializeField] private AudioSource towerAudio;
    [SerializeField] private AudioClip   broadcastClip;
    [SerializeField] private Light[]     towerLights;
    [SerializeField] private ParticleSystem signalVFX;

    private bool activated = false;

    public void Interact(PlayerController player)
    {
        if (activated) return;
        if (GameManager.Instance.CurrentPhase < GamePhase.EvacuationRadio)
        {
            InteractPromptUI.Instance?.ShowMessage("The tower needs power first.", 2f);
            return;
        }

        activated = true;
        towerAudio?.PlayOneShot(broadcastClip);
        signalVFX?.Play();
        foreach (Light l in towerLights) if (l != null) l.enabled = true;

        GameManager.Instance?.OnRadioTowerReached();
    }

    public string GetPrompt()
    {
        if (activated) return "";
        return (GameManager.Instance?.CurrentPhase >= GamePhase.EvacuationRadio) ? prompt : "";
    }
}
