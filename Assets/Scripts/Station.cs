using UnityEngine;

// Classe base de todas as bancadas
// Segurar itens (IItemHolder)
public class Station : MonoBehaviour, IInteractable, IItemHolder
{
    // Ponto onde o item ficará em cima da bancada
    public Transform itemPoint;

    // Item atual da bancada
    private Item currentItem;

    // ===== INTERAÇÃO COM O PLAYER =====
    public virtual void Interact(Player player)
    {
        // Se a estação não tem item e o player tem → colocar item
        if (!HasItem() && player.HasItem())
        {
            Item item = player.GetItem();

            player.ClearItem();

            SetItem(item);

            item.transform.SetParent(itemPoint);
            item.transform.position = itemPoint.position;
        }

        // Se estação tem item e player não tem → pegar item
        else if (HasItem() && !player.HasItem())
        {
            Item item = GetItem();

            ClearItem();

            player.PickupItem(item);
        }
    }

    // ===== IMPLEMENTAÇÃO DO ITEM HOLDER =====

    // Retorna o ponto onde o item ficará
    public Transform GetHoldPoint()
    {
        return itemPoint;
    }

    // Define item da estação
    public void SetItem(Item item)
    {
        currentItem = item;
    }

    // Retorna item atual
    public Item GetItem()
    {
        return currentItem;
    }

    // Remove item da estação
    public void ClearItem()
    {
        currentItem = null;
    }

    // Verifica se possui item
    public bool HasItem()
    {
        return currentItem != null;
    }
}