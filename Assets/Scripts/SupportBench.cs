using UnityEngine;
using System.Collections.Generic;

public class SupportBench : MonoBehaviour, IInteractable
{
    // ===== SLOTS =====
    public ItemHolder slot1;
    public ItemHolder slot2;
    public ItemHolder slot3;

    // ===== NOVO: DISTÂNCIA DE INTERAÇÃO =====
    public float interactDistance = 2.5f;

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

    // ===== COLOCAR ITEM =====
    void TryPlaceItem(Player player)
    {
        Item playerItem = player.GetHeldItem();
        if (playerItem == null) return;

        // bloqueia prato
        if (playerItem is PlateItem)
        {
            return;
        }

        ItemHolder closestSlot = GetClosestAvailableSlot(player.transform.position);

        // ===== VALIDA DISTÂNCIA =====
        if (closestSlot == null) return;

        float distance = Vector3.Distance(player.transform.position, closestSlot.GetHoldPoint().position);

        if (distance > interactDistance) return;

        playerItem.SetHolder(closestSlot);
    }

    // ===== PEGAR ITEM =====
    void TryTakeItem(Player player)
    {
        ItemHolder closestSlot = GetClosestOccupiedSlot(player.transform.position);

        // ===== VALIDA DISTÂNCIA =====
        if (closestSlot == null) return;

        float distance = Vector3.Distance(player.transform.position, closestSlot.GetHoldPoint().position);

        if (distance > interactDistance) return;

        closestSlot.GetItem().SetHolder(player);
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