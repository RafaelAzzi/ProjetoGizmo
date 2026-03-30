using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    // lista de pedidos ativos
    public List<Order> activeOrders = new List<Order>();

    // itens que podem ser pedidos
    public ItemType[] possibleItems;

    // quantidade máxima de pedidos simultâneos
    public int maxOrders = 3;

    void Start()
    {
        // gera pedidos iniciais
        for (int i = 0; i < maxOrders; i++)
        {
            GenerateNewOrder();
        }
    }

    // cria um novo pedido
    void GenerateNewOrder()
    {
        int index = Random.Range(0, possibleItems.Length);

        Order newOrder = new Order();
        newOrder.requestedItem = possibleItems[index];

        activeOrders.Add(newOrder);

        Debug.Log("Novo pedido: " + newOrder.requestedItem);
    }

    // tenta completar um pedido com o item entregue
    public bool TryCompleteOrder(Item item)
    {
        for (int i = 0; i < activeOrders.Count; i++)
        {
            if (item.itemType == activeOrders[i].requestedItem)
            {
                Debug.Log("Pedido COMPLETO: " + item.itemType);

                // remove pedido
                activeOrders.RemoveAt(i);

                // destrói item entregue
                Destroy(item.gameObject);

                // gera novo pedido
                GenerateNewOrder();

                return true;
            }
        }

        Debug.Log("Item não corresponde a nenhum pedido");
        return false;
    }
}
