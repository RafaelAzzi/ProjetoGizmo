using System.Collections.Generic;
using UnityEngine;

// representa um pedido 
[System.Serializable]
public class Order
{
    // ===== SISTEMA NOVO =====
    public List<ItemType> requestedItems = new List<ItemType>();

    public float timeRemaining;
    public float maxTime;
}