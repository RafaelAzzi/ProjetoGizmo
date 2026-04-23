using System.Collections.Generic;
using UnityEngine;

public class PlateStation : MonoBehaviour, IInteractable
{
    [Header("Prefab do prato")]
    public GameObject platePrefab;

    [Header("Slots do prato")]
    public List<Transform> slotPoints;

    [Header("Distância de interação")]
    public float interactDistance = 2.5f;

    private PlateItem currentPlate;

    // ===== INTERAÇÃO =====
    public void Interact(Player player)
    {
        if (player.HasItem())
        {
            TryPlace(player);
        }
        else
        {
            TryTake(player);
        }
    }

    // ===== COLOCAR ITEM OU PRATO =====
    void TryPlace(Player player)
    {
        Item heldItem = player.GetHeldItem();
        if (heldItem == null) return;

        // ===== SE FOR PRATO =====
        PlateItem plate = heldItem as PlateItem;

        if (plate != null)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance > interactDistance) return;

            // coloca prato na bancada
            plate.SetHolder(null);
            plate.transform.position = transform.position;

            currentPlate = plate;
            return;
        }

        // ===== BLOQUEIA COLOCAR PRATO EM OUTRAS BANCADAS =====
        if (heldItem is PlateItem) return;

        // ===== ITEM NORMAL =====

        // cria prato se não existir
        if (currentPlate == null)
        {
            GameObject plateGO = Instantiate(platePrefab, transform.position, Quaternion.identity);
            currentPlate = plateGO.GetComponent<PlateItem>();
        }

        // encontra slot disponível
        Transform slot = GetAvailableSlot();

        if (slot == null) return;

        float dist = Vector3.Distance(player.transform.position, slot.position);
        if (dist > interactDistance) return;

        // adiciona item ao prato
        currentPlate.AddItem(heldItem, slot);
    }

    // ===== PEGAR PRATO =====
    void TryTake(Player player)
    {
        if (currentPlate == null) return;

        // só pega se tiver pelo menos 1 item
        if (currentPlate.itemsInside.Count == 0) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance > interactDistance) return;

        currentPlate.SetHolder(player);

        currentPlate = null;
    }

    // ===== SLOT DISPONÍVEL =====
    Transform GetAvailableSlot()
    {
        foreach (var slot in slotPoints)
        {
            if (slot.childCount == 0)
            {
                return slot;
            }
        }

        return null;
    }
}
