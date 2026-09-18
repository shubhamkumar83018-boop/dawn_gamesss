using UnityEngine;

/// <summary>
/// Generator Fuse item - player must find it to activate the generator.
/// Implements IInteractable.
/// </summary>
public class GeneratorFuse : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt = "Pick up Fuse [E]";
    private bool collected = false;

    public void Interact(PlayerController player)
    {
        if (collected) return;
        collected = true;
        GameManager.Instance?.OnFuseFound();
        gameObject.SetActive(false);
    }

    public string GetPrompt() => collected ? "" : prompt;
}
