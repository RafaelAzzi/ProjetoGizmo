using UnityEngine;
using System.Collections.Generic;

public class SupportBench : MonoBehaviour, IInteractable
{
    // ===== SLOTS =====
    public ItemHolder slot1;
    public ItemHolder slot2;
    public ItemHolder slot3;

    // ===== INTERAÇÃO =====
    public void Interact(Player player)
    {
        if (player.HasItem())
        {
            TryPlaceItem(player);
        }
        else
        {
            TryTakeItem(player);
        }
    }

    // ===== COLOCAR ITEM (slot mais próximo vazio) =====
    void TryPlaceItem(Player player)
    {
        Item playerItem = player.GetHeldItem();
        if (playerItem == null) return;

        ItemHolder closestSlot = GetClosestAvailableSlot(player.transform.position);

        if (closestSlot != null)
        {
            playerItem.SetHolder(closestSlot);
        }
        else
        {
            Debug.Log("SupportBench cheia!");
        }
    }

    // ===== PEGAR ITEM (slot mais próximo com item) =====
    void TryTakeItem(Player player)
    {
        ItemHolder closestSlot = GetClosestOccupiedSlot(player.transform.position);

        if (closestSlot != null)
        {
            closestSlot.GetItem().SetHolder(player);
        }
        else
        {
            Debug.Log("SupportBench vazia!");
        }
    }

    // ===== SLOT VAZIO MAIS PRÓXIMO =====
    ItemHolder GetClosestAvailableSlot(Vector3 playerPos)
    {
        List<ItemHolder> slots = new List<ItemHolder> { slot1, slot2, slot3 };

        ItemHolder closest = null;
        float minDistance = Mathf.Infinity;

        foreach (ItemHolder slot in slots)
        {
            if (!slot.HasItem())
            {
                float distance = Vector3.Distance(playerPos, slot.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = slot;
                }
            }
        }

        return closest;
    }

    // ===== SLOT COM ITEM MAIS PRÓXIMO =====
    ItemHolder GetClosestOccupiedSlot(Vector3 playerPos)
    {
        List<ItemHolder> slots = new List<ItemHolder> { slot1, slot2, slot3 };

        ItemHolder closest = null;
        float minDistance = Mathf.Infinity;

        foreach (ItemHolder slot in slots)
        {
            if (slot.HasItem())
            {
                float distance = Vector3.Distance(playerPos, slot.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = slot;
                }
            }
        }

        return closest;
    }
}