using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Bancada de bebidas com tempo
public class DrinkStation : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class DrinkSlot : IItemHolder
    {
        public Transform holdPoint; // posição visual

        private Item currentItem;

        public float timer;
        public float maxTime;

        public bool isProcessing;
        public bool isReady;

        // ===== IItemHolder =====

        public Transform GetHoldPoint()
        {
            return holdPoint;
        }

        public void SetItem(Item item)
        {
            currentItem = item;
        }

        public Item GetItem()
        {
            return currentItem;
        }

        public void ClearItem()
        {
            currentItem = null;
        }

        public bool HasItem()
        {
            return currentItem != null;
        }
    }

    public List<DrinkSlot> slots = new List<DrinkSlot>();

    [Header("Configuração")]
    public float processTime = 5f;

    [Header("Itens permitidos")]
    public List<ItemType> allowedItems;

    // ===== DISTÂNCIA DE INTERAÇÃO =====
    public float interactDistance = 2.5f;

    private void Update()
    {
        ProcessSlots();
    }

    // ===== PROCESSAMENTO =====
    void ProcessSlots()
    {
        foreach (var slot in slots)
        {
            if (slot.HasItem() && slot.isProcessing)
            {
                slot.timer += Time.deltaTime;

                if (slot.timer >= slot.maxTime)
                {
                    slot.isProcessing = false;
                    slot.isReady = true;

                    // marca o item como já processado
                    slot.GetItem().isProcessed = true;

                    Debug.Log("Item pronto!");
                }
            }
        }
    }

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
        Item playerItem = player.GetItem();
        if (playerItem == null) return;

        // verifica se é permitido e se ainda não foi processado
        if (!allowedItems.Contains(playerItem.itemType) || playerItem.isProcessed)
            return;

        DrinkSlot closestSlot = GetClosestAvailableSlot(player.transform.position);

        if (closestSlot == null) return;

        float distance = Vector3.Distance(player.transform.position, closestSlot.holdPoint.position);

        if (distance > interactDistance) return;

        // coloca item no slot
        playerItem.SetHolder(closestSlot);

        closestSlot.timer = 0f;
        closestSlot.maxTime = processTime;
        closestSlot.isProcessing = true;
        closestSlot.isReady = false;
    }

    // ===== PEGAR ITEM =====
    void TryTakeItem(Player player)
    {
        DrinkSlot closestSlot = GetClosestReadySlot(player.transform.position);

        if (closestSlot == null) return;

        float distance = Vector3.Distance(player.transform.position, closestSlot.holdPoint.position);

        if (distance > interactDistance) return;

        Item item = closestSlot.GetItem();

        item.SetHolder(player);

        closestSlot.ClearItem();
        closestSlot.timer = 0f;
        closestSlot.isReady = false;
    }

    // ===== SLOT VAZIO MAIS PRÓXIMO =====
    DrinkSlot GetClosestAvailableSlot(Vector3 playerPos)
    {
        DrinkSlot closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var slot in slots)
        {
            if (!slot.HasItem())
            {
                float distance = Vector3.Distance(playerPos, slot.holdPoint.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = slot;
                }
            }
        }

        return closest;
    }

    // ===== SLOT PRONTO MAIS PRÓXIMO =====
    DrinkSlot GetClosestReadySlot(Vector3 playerPos)
    {
        DrinkSlot closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var slot in slots)
        {
            if (slot.HasItem() && slot.isReady)
            {
                float distance = Vector3.Distance(playerPos, slot.holdPoint.position);

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