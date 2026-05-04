using UnityEngine;

// Representa um ponto de interação + highlight
public class HoldPoint : MonoBehaviour, IInteractable
{
    // Referência da bancada
    public MonoBehaviour parentInteractable;

    // Objeto visual do highlight (DebugPoint)
    public GameObject highlightVisual;

    // ===== INTERAÇÃO =====
    public void Interact(Player player)
    {
        if (parentInteractable == null)
        {
            Debug.LogWarning("HoldPoint sem parentInteractable!");
            return;
        }

        if (parentInteractable is IInteractable interactable)
        {
            interactable.Interact(player);
        }
    }

    // ===== ATIVAR HIGHLIGHT =====
    public void ShowHighlight()
    {
        if (highlightVisual != null)
            highlightVisual.SetActive(true);
    }

    // ===== DESATIVAR HIGHLIGHT =====
    public void HideHighlight()
    {
        if (highlightVisual != null)
            highlightVisual.SetActive(false);
    }
}