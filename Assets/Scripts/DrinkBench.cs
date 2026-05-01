using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Bancada de bebidas com tempo
public class DrinkBench : MonoBehaviour, IInteractable
{
    [Header("UI")]
    public Vector3 progressBarOffset = new Vector3(0, 1f, 0);

    [System.Serializable]
    public class DrinkSlot : IItemHolder
    {
        public Transform holdPoint;

        private Item currentItem;

        public float timer;
        public float maxTime;

        public bool isProcessing;
        public bool isReady;

        public bool isSpoiled;
        public float readyTimer;

        public GameObject progressBarInstance;
        public Slider progressBar;

        public Transform GetHoldPoint() => holdPoint;
        public void SetItem(Item item) => currentItem = item;
        public Item GetItem() => currentItem;
        public void ClearItem() => currentItem = null;
        public bool HasItem() => currentItem != null;
    }

    public List<DrinkSlot> slots = new List<DrinkSlot>();

    [Header("Configuração")]
    public float processTime = 7f;
    public float readyDuration = 5f;

    public GameObject progressBarPrefab;

    [Header("Itens permitidos")]
    public List<ItemType> allowedItems;

    public float interactDistance = 2.5f;

    void Update()
    {
        ProcessSlots();
    }

    // ===== PROCESSAMENTO =====
    void ProcessSlots()
    {
        foreach (var slot in slots)
        {
            if (!slot.HasItem()) continue;

            Item item = slot.GetItem();

            // ===== PROCESSANDO =====
            if (slot.isProcessing)
            {
                slot.timer += Time.deltaTime;

                if (slot.progressBar != null)
                {
                    slot.progressBar.value = slot.timer / slot.maxTime;
                }

                if (slot.timer >= slot.maxTime)
                {
                    if (slot.progressBarInstance != null)
                    {
                        Destroy(slot.progressBarInstance);
                    }

                    slot.isProcessing = false;
                    slot.isReady = true;

                    item.isProcessed = true;

                    // DEFINE COMO PERFEITO
                    item.quality = ItemQuality.Perfect;

                    slot.timer = slot.maxTime;
                    slot.readyTimer = 0f;

                    Debug.Log("Item PERFEITO");
                }
            }
            // ===== APÓS PRONTO =====
            else if (slot.isReady)
            {
                slot.readyTimer += Time.deltaTime;

                if (slot.readyTimer >= readyDuration)
                {
                    // TRANSIÇÃO DE ESTADOS
                    if (item.quality == ItemQuality.Perfect)
                    {
                        item.quality = ItemQuality.Overcooked;
                        Debug.Log("Item passou do ponto!");
                    }
                    else if (item.quality == ItemQuality.Overcooked)
                    {
                        item.quality = ItemQuality.Spoiled;
                        slot.isSpoiled = true;

                        // visual
                        Renderer rend = item.GetComponent<Renderer>();
                        if (rend != null)
                        {
                            rend.material.color = new Color(0.4f, 0.2f, 0.1f);
                        }

                        Debug.Log("Item ESTRAGOU!");
                    }

                    slot.readyTimer = 0f;
                }
            }
        }
    }

    // ===== INTERAÇÃO =====
    public void Interact(Player player)
    {
        if (player.HasItem())
            TryPlaceItem(player);
        else
            TryTakeItem(player);
    }

    // ===== COLOCAR ITEM =====
    void TryPlaceItem(Player player)
    {
        Item playerItem = player.GetItem();
        if (playerItem == null) return;

        if (!allowedItems.Contains(playerItem.itemType) || playerItem.isProcessed)
            return;

        DrinkSlot closestSlot = GetClosestAvailableSlot(player.transform.position);
        if (closestSlot == null) return;

        float distance = Vector3.Distance(player.transform.position, closestSlot.holdPoint.position);
        if (distance > interactDistance) return;

        playerItem.SetHolder(closestSlot);

        closestSlot.timer = playerItem.processProgress;
        closestSlot.maxTime = processTime;
        closestSlot.isProcessing = true;
        closestSlot.isReady = false;
        closestSlot.isSpoiled = false;
        closestSlot.readyTimer = 0f;

        closestSlot.progressBarInstance = Instantiate(
            progressBarPrefab,
            closestSlot.holdPoint.position + progressBarOffset,
            Quaternion.identity
        );

        closestSlot.progressBar = closestSlot.progressBarInstance.GetComponentInChildren<Slider>();
    }

    // ===== PEGAR ITEM =====
    void TryTakeItem(Player player)
    {
        DrinkSlot closestSlot = GetClosestAnySlot(player.transform.position);
        if (closestSlot == null) return;

        float distance = Vector3.Distance(player.transform.position, closestSlot.holdPoint.position);
        if (distance > interactDistance) return;

        Item item = closestSlot.GetItem();

        // salva progresso
        item.processProgress = closestSlot.timer;

        item.SetHolder(player);

        // ===== DEFINE QUALIDADE FINAL AO PEGAR =====
        if (!item.isProcessed)
        {
            float progressPercent = closestSlot.timer / closestSlot.maxTime;

            // MENOS DE 50% → CRU
            if (progressPercent < 0.5f)
            {
                item.quality = ItemQuality.Crude;
                Debug.Log("Pegou CRU");
            }
            else
            {
                item.quality = ItemQuality.Undercooked;
                Debug.Log("Pegou INCOMPLETO");
            }
        }
        else if (item.quality == ItemQuality.Overcooked)
        {
            Debug.Log("Pegou PASSOU DO PONTO");
        }
        else if (item.quality == ItemQuality.Spoiled)
        {
            Debug.Log("Pegou ESTRAGADO");
        }
        else
        {
            Debug.Log("Pegou PERFEITO");
        }

        // limpa UI
        if (closestSlot.progressBarInstance != null)
        {
            Destroy(closestSlot.progressBarInstance);
        }

        closestSlot.ClearItem();
        closestSlot.timer = 0f;
        closestSlot.isReady = false;
        closestSlot.isProcessing = false;
        closestSlot.isSpoiled = false;
    }

    // ===== SLOT VAZIO =====
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

    // ===== QUALQUER SLOT =====
    DrinkSlot GetClosestAnySlot(Vector3 playerPos)
    {
        DrinkSlot closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var slot in slots)
        {
            if (slot.HasItem())
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