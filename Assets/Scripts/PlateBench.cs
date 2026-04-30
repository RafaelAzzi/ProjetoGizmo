using System.Collections.Generic;
using UnityEngine;

public class PlateBench : MonoBehaviour, IInteractable
{
    [Header("Prefab do prato")]
    public GameObject platePrefab;

    // guarda os pratos de cada slot
    private Dictionary<Transform, PlateItem> plates = new Dictionary<Transform, PlateItem>();

    [Header("Slots (cada um com seu prato)")]
    public List<Transform> slotPoints;

    [Header("Distância de interação")]
    public float interactDistance = 3f;

    void Start()
    {
        // cria um prato em cada slot ao iniciar
        foreach (Transform slot in slotPoints)
        {
            CreatePlateInSlot(slot);
        }
    }

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

    // ===== CRIA PRATO NO SLOT =====
    void CreatePlateInSlot(Transform slot)
    {
        GameObject plateGO = Instantiate(platePrefab, slot.position, Quaternion.identity);

        PlateItem plate = plateGO.GetComponent<PlateItem>();

        // define o slot como pai
        plate.transform.SetParent(slot);
        plate.transform.localPosition = Vector3.zero;
        plate.transform.localRotation = Quaternion.identity;

        // salva referência
        plates[slot] = plate;

        plate.originalSlot = slot;
    }

    // ===== COLOCAR ITEM OU DEVOLVER PRATO =====
    void TryPlace(Player player)
    {
        Item heldItem = player.GetHeldItem();
        if (heldItem == null) return;

        // ===== SE FOR PRATO =====
        PlateItem plate = heldItem as PlateItem;

        if (plate != null)
        {
            Transform closestSlot = GetClosestSlot(player.transform.position);
            if (closestSlot == null) return;

            float distance = Vector3.Distance(player.transform.position, closestSlot.position);
            if (distance > interactDistance) return;

            // devolve prato ao slot
            plate.SetHolder(null);

            plate.transform.SetParent(closestSlot);
            plate.transform.localPosition = Vector3.zero;
            plate.transform.localRotation = Quaternion.identity;

            // atualiza referência
            plates[closestSlot] = plate;

            return;
        }

        // ===== ITEM NORMAL =====

        Transform closestSlotWithSpace = GetClosestSlotWithSpace(player.transform.position);
        if (closestSlotWithSpace == null) return;

        float dist = Vector3.Distance(player.transform.position, closestSlotWithSpace.position);
        if (dist > interactDistance) return;

        PlateItem plateInSlot = plates[closestSlotWithSpace];
        if (plateInSlot == null) return;

        Transform freePoint = GetFreePointOnPlate(plateInSlot);
        if (freePoint == null) return;

        // tenta adicionar no prato PRIMEIRO
        bool added = plateInSlot.AddItem(heldItem, freePoint);

        if (!added)
        {
            // se não conseguiu (ex: item estragado), não faz nada
            return;
        }

        // agora sim remove da mão do player
        heldItem.SetHolder(null);
    }

    // ===== PEGAR PRATO =====
    void TryTake(Player player)
    {
        Transform closestSlot = GetClosestSlot(player.transform.position);
        if (closestSlot == null) return;

        float distance = Vector3.Distance(player.transform.position, closestSlot.position);
        if (distance > interactDistance) return;

        PlateItem plate = plates[closestSlot];
        if (plate == null) return;

        plate.SetHolder(player);

        // limpa o slot (fica sem prato até devolver)
        plates[closestSlot] = null;
    }

    // ===== SLOT MAIS PRÓXIMO =====
    Transform GetClosestSlot(Vector3 playerPos)
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform slot in slotPoints)
        {
            float distance = Vector3.Distance(playerPos, slot.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = slot;
            }
        }

        return closest;
    }

    // ===== SLOT MAIS PRÓXIMO COM ESPAÇO =====
    Transform GetClosestSlotWithSpace(Vector3 playerPos)
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform slot in slotPoints)
        {
            PlateItem plate = plates[slot];

            if (plate == null) continue;

            // verifica se ainda cabe item
            if (!plate.CanAddItem()) continue;

            float distance = Vector3.Distance(playerPos, slot.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = slot;
            }
        }

        return closest;
    }

    // ===== PEGA UM PONTO LIVRE NO PRATO =====
    Transform GetFreePointOnPlate(PlateItem plate)
    {
        foreach (Transform point in plate.slotPoints)
        {
            if (point.childCount == 0)
                return point;
        }

        return null;
    }

    // ===== RESPAWN DO PRATO =====
    public void RespawnPlate(Transform slot)
    {
        if (slot == null) return;

        // evita duplicar prato
        if (plates.ContainsKey(slot))
        {
            plates[slot] = null;
        }

        CreatePlateInSlot(slot);
    }
}