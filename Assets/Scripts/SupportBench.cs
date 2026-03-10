using UnityEngine;

public class SupportBench : MonoBehaviour, IInteractable
{
    // ===== SLOTS DA BANCADA =====
    public ItemHolder slot1; 
    public ItemHolder slot2; 

    // ===== INTERAÇÃO =====
    public void Interact(Player player)
    {
        Debug.Log("SupportBench Interact");

        // Se o jogador está segurando item
        if (player.HasItem())
        {
            TryPlaceItem(player);
        }
        else
        {
            TryTakeItem(player);
        }
    }

    // ===== TENTAR COLOCAR ITEM =====
    void TryPlaceItem(Player player)
    {
        Item playerItem = player.GetItem();

        // tenta colocar no slot1
        if (!slot1.HasItem())
        {
            playerItem.SetHolder(slot1);
            return;
        }

        // tenta colocar no slot2
        if (!slot2.HasItem())
        {
            playerItem.SetHolder(slot2);
            return;
        }

        Debug.Log("SupportBench cheia!");
    }

    // ===== TENTAR PEGAR ITEM =====
    void TryTakeItem(Player player)
    {
        // pega primeiro do slot1
        if (slot1.HasItem())
        {
            slot1.GetItem().SetHolder(player);
            return;
        }

        // depois slot2
        if (slot2.HasItem())
        {
            slot2.GetItem().SetHolder(player);
            return;
        }

        Debug.Log("SupportBench vazia!");
    }
}