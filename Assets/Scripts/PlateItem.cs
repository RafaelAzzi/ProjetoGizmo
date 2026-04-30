using System.Collections.Generic;
using UnityEngine;

// Representa o prato como ITEM carregável
public class PlateItem : Item
{
    public Transform originalSlot;
    
    // Lista de itens dentro do prato
    public List<ItemType> itemsInside = new List<ItemType>();

    // guarda os objetos reais (para verificar qualidade)
    public List<Item> itemsObjects = new List<Item>();

    [Header("Pontos onde os itens ficam no prato")]
    public List<Transform> slotPoints = new List<Transform>();

    // Escala original dos itens
    private Dictionary<Item, Vector3> originalScales = new Dictionary<Item, Vector3>();

    // ===== ADICIONAR ITEM AO PRATO =====
    public bool AddItem(Item item, Transform slotPoint)
    {
        // NÃO permite adicionar item estragado
        if (item.quality == ItemQuality.Spoiled)
        {
            Debug.Log("Item estragado não pode ir para o prato!");
            return false;
        }

        if (item == null) 
            return false;

        // adiciona tipo na lista
        itemsInside.Add(item.itemType);

        // guarda o item real também
        itemsObjects.Add(item);

        // salva escala original
        originalScales[item] = item.transform.localScale;

        // move item para o slot do prato
        item.SetHolder(null);
        item.transform.SetParent(slotPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        // reduz tamanho (40%)
        item.transform.localScale *= 0.6f;

        // desativa física
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // desativa collider do item dentro do prato
        Collider col = item.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        return true;
    }

    // ===== PEGAR ITENS (para validação) =====
    public List<ItemType> GetItems()
    {
        return itemsInside;
    }

    // verifica se ainda cabe item no prato
    public bool CanAddItem()
    {
        return itemsInside.Count < slotPoints.Count;
    }

    // retorna os itens reais (para checar qualidade)
    public List<Item> GetItemObjects()
    {
        return itemsObjects;
    }
}
