using UnityEngine;
using TMPro;

/// <summary>
/// Interaction prompt UI - shows context-sensitive prompts when near interactables.
/// </summary>
public class InteractPromptUI : MonoBehaviour
{
    public static InteractPromptUI Instance { get; private set; }

    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI promptText;

    private float hideTimer = 0f;
    private bool timedHide = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        promptPanel?.SetActive(false);
    }

    void Update()
    {
        if (timedHide)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0f)
            {
                timedHide = false;
                HidePrompt();
            }
        }
    }

    public void ShowPrompt(string text)
    {
        timedHide = false;
        promptText.text = text;
        promptPanel?.SetActive(!string.IsNullOrEmpty(text));
    }

    public void ShowMessage(string text, float duration)
    {
        promptText.text = text;
        promptPanel?.SetActive(true);
        hideTimer = duration;
        timedHide = true;
    }

    public void HidePrompt()
    {
        promptPanel?.SetActive(false);
    }
}
