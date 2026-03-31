using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class OrderManager : MonoBehaviour
{
    public TextMeshProUGUI ordersText;

    public float orderTime = 20f; // tempo padrão por pedido

    // lista de pedidos ativos
    public List<Order> activeOrders = new List<Order>();

    // itens que podem ser pedidos
    public ItemType[] possibleItems;

    // quantidade máxima de pedidos simultâneos
    public int maxOrders = 2;

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
        if (activeOrders.Count >= maxOrders) return;

        int index = Random.Range(0, possibleItems.Length);

        Order newOrder = new Order();
        newOrder.requestedItem = possibleItems[index];

        // tempo
        newOrder.maxTime = orderTime;
        newOrder.timeRemaining = orderTime;

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

    void Update()
    {
        UpdateOrders();
        UpdateUI();
    }

    // ===== SISTEMA DE TEMPO =====
    void UpdateOrders()
    {
        for (int i = activeOrders.Count - 1; i >= 0; i--)
        {
            Order order = activeOrders[i];

            order.timeRemaining -= Time.deltaTime;

            //  DEBUG VISUAL COM URGÊNCIA
            Debug.Log(
                GetOrderUrgency(order) +
                " Pedido: " + order.requestedItem +
                " | Tempo: " + Mathf.Ceil(order.timeRemaining)
            );

            if (order.timeRemaining <= 0)
            {
                Debug.Log("Pedido EXPIRADO: " + order.requestedItem);

                // remove pedido
                activeOrders.RemoveAt(i);

                // futura penalidade aqui

                // gera novo pedido
                GenerateNewOrder();
            }
        }
    }

    // ===== URGÊNCIA DO PEDIDO =====
    string GetOrderUrgency(Order order)
    {
        float percent = order.timeRemaining / order.maxTime;

        if (percent > 0.5f)
            return "green";

        if (percent > 0.25f)
            return "yellow";

        return "red";
    }

    void UpdateUI()
    {
        if (ordersText == null) return;

        string text = "Pedidos:\n";

        foreach (Order order in activeOrders)
        {
            string color = GetOrderUrgency(order); // A COR

            text += "<color=" + color + ">";
            text += order.requestedItem.ToString();
            text += " -> ";
            text += Mathf.Ceil(order.timeRemaining) + "s";
            text += "</color>\n"; // fechar 
        }

        ordersText.text = text;
    }
}