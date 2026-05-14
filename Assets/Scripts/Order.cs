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

    // ID visual usado pelo pedido
    public int visualID = -1;

    // cor visual do pedido
    [HideInInspector]
    public Color visualColor = Color.white;
}