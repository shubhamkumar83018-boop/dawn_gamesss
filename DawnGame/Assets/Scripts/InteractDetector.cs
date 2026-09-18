using UnityEngine;

/// <summary>
/// Proximity trigger - shows interact prompt and detects the closest interactable in range.
/// Attach to the player. Uses OverlapSphere each frame for lightweight detection.
/// </summary>
public class InteractDetector : MonoBehaviour
{
    [SerializeField] private float       detectionRadius = 2.5f;
    [SerializeField] private LayerMask   interactLayer;

    private IInteractable currentTarget;

    void Update()
    {
        currentTarget = null;
        float closestDist = Mathf.Infinity;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, interactLayer);
        foreach (Collider hit in hits)
        {
            IInteractable ia = hit.GetComponent<IInteractable>();
            if (ia == null) continue;
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                currentTarget = ia;
            }
        }

        if (currentTarget != null)
            InteractPromptUI.Instance?.ShowPrompt(currentTarget.GetPrompt());
        else
            InteractPromptUI.Instance?.HidePrompt();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
