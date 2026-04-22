using System.Collections.Generic;
using UnityEngine;

// representa um pedido 
[System.Serializable]
public class Order
{
    // ===== SISTEMA NOVO =====
    public List<ItemType> requestedItems = new List<ItemType>();

    // ===== SISTEMA ANTIGO (compatibilidade) =====
    public ItemType requestedItem;

    public float timeRemaining;
    public float maxTime;
}