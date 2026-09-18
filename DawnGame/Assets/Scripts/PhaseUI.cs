using UnityEngine;
using TMPro;

/// <summary>
/// Phase UI - displays current objective/phase name on screen briefly.
/// </summary>
public class PhaseUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI phaseText;
    [SerializeField] private Animator        phaseAnimator;
    [SerializeField] private float           displayDuration = 3f;

    private static readonly int ShowTrigger = Animator.StringToHash("Show");

    public void ShowPhase(GamePhase phase)
    {
        phaseText.text = GetPhaseLabel(phase);
        phaseAnimator?.SetTrigger(ShowTrigger);
    }

    string GetPhaseLabel(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.RadioTransmission: return "Radio Transmission Received";
            case GamePhase.FindGeneratorFuse: return "Find the Generator Fuse";
            case GamePhase.ExploreCampus:     return "Explore the Campus";
            case GamePhase.NoiseInvestigation:return "Stay Quiet - Hunter Nearby";
            case GamePhase.HunterInvestigation: return "Hunter Investigates!";
            case GamePhase.Generator:         return "Generator Online";
            case GamePhase.RadioUpdate:       return "Radio Update Received";
            case GamePhase.Hospital:          return "Reach the Hospital";
            case GamePhase.SurvivorChoice:    return "Survivor Found - Choose";
            case GamePhase.EvacuationRadio:   return "Evacuation Route Broadcast";
            case GamePhase.RadioTower:        return "Activate the Radio Tower";
            case GamePhase.FinalChase:        return "RUN!";
            case GamePhase.Dawn:              return "Dawn Breaks";
            case GamePhase.Ending:            return "";
            default:                          return "";
        }
    }
}
