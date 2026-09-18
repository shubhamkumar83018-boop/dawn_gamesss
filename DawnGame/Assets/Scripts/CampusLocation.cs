using UnityEngine;

/// <summary>
/// Campus location zone - tags a physical area with a GamePhase.
/// When the player enters, the GameManager may advance to that phase.
/// </summary>
public class CampusLocation : MonoBehaviour
{
    [SerializeField] private string      locationName;
    [SerializeField] private GamePhase   activatesPhase;
    [SerializeField] private bool        requiresPreviousPhase = true;
    [SerializeField] private GamePhase   requiredPhase;
    [SerializeField] private GameObject  mapMarker;

    [Header("Minimap Icon")]
    [SerializeField] private SpriteRenderer minimapIcon;
    [SerializeField] private Color          visitedColor = Color.grey;

    private bool visited = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (visited) return;

        if (requiresPreviousPhase &&
            GameManager.Instance != null &&
            GameManager.Instance.CurrentPhase < requiredPhase) return;

        visited = true;
        if (minimapIcon != null) minimapIcon.color = visitedColor;
        if (mapMarker    != null) mapMarker.SetActive(false);

        GameManager.Instance?.BeginPhase(activatesPhase);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        BoxCollider bc = GetComponent<BoxCollider>();
        if (bc != null)
            Gizmos.DrawWireCube(transform.position + bc.center, bc.size);
    }
}
