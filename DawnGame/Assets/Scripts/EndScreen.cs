using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// End screen - shows victory/defeat, score, and survivor status.
/// </summary>
public class EndScreen : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeBonusText;
    [SerializeField] private TextMeshProUGUI survivorStatusText;
    [SerializeField] private TextMeshProUGUI outcomeText;

    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip   victoryClip;
    [SerializeField] private AudioClip   defeatClip;

    void Awake()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel  != null) defeatPanel.SetActive(false);

        restartButton?.onClick.AddListener(RestartGame);
        mainMenuButton?.onClick.AddListener(GoToMainMenu);
    }

    public void ShowEnding(bool survived, int score, bool survivorSaved, float timeRemaining)
    {
        gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

        if (survived)
        {
            victoryPanel?.SetActive(true);
            defeatPanel?.SetActive(false);
            outcomeText.text = survivorSaved ? "You Escaped — And Saved a Life." : "You Escaped.";
            audioSource?.PlayOneShot(victoryClip);
        }
        else
        {
            defeatPanel?.SetActive(true);
            victoryPanel?.SetActive(false);
            outcomeText.text = "Caught Before Dawn.";
            audioSource?.PlayOneShot(defeatClip);
        }

        int timeBonus = Mathf.FloorToInt(timeRemaining) * 10;
        scoreText.text        = $"Score: {score + timeBonus}";
        timeBonusText.text    = $"Time Bonus: +{timeBonus}";
        survivorStatusText.text = survivorSaved ? "Survivor: Rescued ✓" : "Survivor: Left Behind";
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
