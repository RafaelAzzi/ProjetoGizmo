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
    public void Interact(Player player) 
       {
        if (!GameManager.Instance.IsGamePlaying()) return;
        
        // PLAYER TEM ITEM → coloca na bancada
        if (!HasItem() && player.HasItem())
        {
            Item item = player.GetItem();

            // apenas muda o holder (o item cuida do resto)
            item.SetHolder(this);
        }

        // PLAYER NÃO TEM ITEM → pega da bancada
        else if (HasItem() && !player.HasItem())
        {
            GetItem().SetHolder(player);
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