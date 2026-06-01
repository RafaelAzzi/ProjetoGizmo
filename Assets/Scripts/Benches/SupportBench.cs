using UnityEngine;
using System.Collections.Generic;

public class SupportBench : MonoBehaviour, IInteractable
{
  // ===== SLOTS =====
    public List<ItemHolder> slots = new List<ItemHolder>();

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

        ItemHolder closestSlot = GetClosestAvailableSlot(player.transform.position);

        // ===== VALIDA DISTÂNCIA =====
        if (closestSlot == null) return;

        float distance = Vector3.Distance(player.transform.position, closestSlot.GetHoldPoint().position);

        if (distance > interactDistance) return;

        // Verifica se já existe um prato no slot
        PlateItem plate = closestSlot.GetItem() as PlateItem;

        if (plate != null)
        {
            // Procura um ponto livre no prato
            Transform freePoint = null;

            foreach (Transform point in plate.slotPoints)
            {
                if (point.childCount == 0)
                {
                    freePoint = point;
                    break;
                }
            }

            if (freePoint == null)
                return;

            // Usa a mesma lógica já existente da PlateBench
            bool added = plate.AddItem(playerItem, freePoint);

            if (!added)
                return;

            return;
        }

        // Slot vazio → comportamento original
        playerItem.SetHolder(closestSlot);

        playerItem.ShowIcon();
    }

    // ===== PEGAR ITEM =====
    void TryTakeItem(Player player)
    {
        ItemHolder closestSlot = GetClosestOccupiedSlot(player.transform.position);

        // ===== VALIDA DISTÂNCIA =====
        if (closestSlot == null) return;

        float distance = Vector3.Distance(player.transform.position, closestSlot.GetHoldPoint().position);

        if (distance > interactDistance) return;

        // pega o item antes de remover do slot
        Item item = closestSlot.GetItem();

        if (item == null) return;

        // move o item para o player
        item.SetHolder(player);

        // mostra o ícone
        item.ShowIcon();
    }

    // ===== SLOT VAZIO MAIS PRÓXIMO =====
    ItemHolder GetClosestAvailableSlot(Vector3 playerPos)
    {
        ItemHolder closest = null;
        float minDistance = Mathf.Infinity;

        foreach (ItemHolder slot in slots)
        {
            // Agora considera:
            // slot vazio
            // prato com espaço
            if (CanReceiveItem(slot))
            {
                float distance = Vector3.Distance(
                    playerPos,
                    slot.transform.position
                );

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

    // Retorna se o slot pode receber um item
    bool CanReceiveItem(ItemHolder slot)
    {
        // Slot vazio
        if (!slot.HasItem())
        {
            return true;
        }

        // Verifica se o item do slot é um prato
        PlateItem plate = slot.GetItem() as PlateItem;

        if (plate != null)
        {
            // Só pode receber se ainda tiver espaço
            return plate.CanAddItem();
        }

        // Item normal ocupa o slot
        return false;
    }
}