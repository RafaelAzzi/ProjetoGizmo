using UnityEngine;

// representa um pedido 
[System.Serializable]
public class Order
{
    public ItemType requestedItem; // item que o jogador precisa entregar
    public float timeRemaining; // tempo atual
    public float maxTime;       // tempo inicial
}
