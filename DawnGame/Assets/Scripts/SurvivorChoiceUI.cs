using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Survivor choice UI - shown when the player reaches the hospital survivor.
/// </summary>
public class SurvivorChoiceUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button     rescueButton;
    [SerializeField] private Button     leaveButton;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private Action onRescue;
    private Action onLeave;

    void Awake()
    {
        panel?.SetActive(false);
        rescueButton?.onClick.AddListener(OnRescueClicked);
        leaveButton?.onClick.AddListener(OnLeaveClicked);
    }

    public void ShowChoice(Action rescueCallback, Action leaveCallback)
    {
        onRescue = rescueCallback;
        onLeave  = leaveCallback;
        dialogueText.text = "\"Please... don't leave me here. I can walk.\"";
        panel?.SetActive(true);
        Time.timeScale = 0f; // Pause while choosing
    }

    void OnRescueClicked()
    {
        Time.timeScale = 1f;
        panel?.SetActive(false);
        onRescue?.Invoke();
    }

    void OnLeaveClicked()
    {
        Time.timeScale = 1f;
        panel?.SetActive(false);
        onLeave?.Invoke();
    }
}
