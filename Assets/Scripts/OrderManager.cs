using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrderManager : MonoBehaviour
{
    [Header("Quantidade de itens por pedido")]
    public int minItemsPerOrder = 1;
    public int maxItemsPerOrder = 1;

    public TextMeshProUGUI ordersText;

    float blinkTimer = 0f;

    public float orderTime = 20f; // tempo padrão por pedido

    // lista de pedidos ativos
    public List<Order> activeOrders = new List<Order>();

    // itens que podem ser pedidos
    public ItemType[] possibleItems;

    // quantidade máxima de pedidos simultâneos
    public int maxOrders = 2;

    void Start()
    {
        //  NÃO GERAR MAIS PEDIDOS AUTOMATICAMENTE
        // agora os robôs controlam isso
    }

    // ===== NOVO: AGORA É PUBLIC E RETORNA O PEDIDO =====
    public Order GenerateNewOrder()
    {
        // se já atingiu limite, não cria
        if (activeOrders.Count >= maxOrders) return null;

        Order newOrder = new Order();

        // define quantos itens o pedido terá
        int itemCount = Random.Range(minItemsPerOrder, maxItemsPerOrder + 1);

        // limpa lista
        newOrder.requestedItems.Clear();

        // gera os itens
        for (int i = 0; i < itemCount; i++)
        {
            int randomIndex = Random.Range(0, possibleItems.Length);
            ItemType randomItem = possibleItems[randomIndex];

            newOrder.requestedItems.Add(randomItem);
        }

        // compatibilidade
        if (newOrder.requestedItems.Count > 0)
        {
            newOrder.requestedItem = newOrder.requestedItems[0];
        }

        // define tempo
        newOrder.maxTime = orderTime;
        newOrder.timeRemaining = orderTime;

        // adiciona na lista
        activeOrders.Add(newOrder);

        // DEBUG
        string items = "";

        foreach (var item in newOrder.requestedItems)
        {
            items += item.ToString() + " ";
        }

        Debug.Log("Novo pedido: " + items);

        return newOrder;
    }

    // ===== COMPLETAR PEDIDO =====
    public bool TryCompleteOrder(Item item)
    {
        for (int i = 0; i < activeOrders.Count; i++)
        {
            Order order = activeOrders[i];

            // ===== NOVO SISTEMA (lista) =====
            if (order.requestedItems.Contains(item.itemType))
            {
                Debug.Log("Item entregue: " + item.itemType);

                // remove item da lista
                order.requestedItems.Remove(item.itemType);

                // destrói item entregue
                Destroy(item.gameObject);

                // se acabou a lista → pedido completo
                if (order.requestedItems.Count == 0)
                {
                    Debug.Log("Pedido COMPLETO!");

                    activeOrders.RemoveAt(i);
                }

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

        blinkTimer += Time.deltaTime;
    }

    // ===== SISTEMA DE TEMPO =====
    void UpdateOrders()
    {
        for (int i = activeOrders.Count - 1; i >= 0; i--)
        {
            Order order = activeOrders[i];

            order.timeRemaining -= Time.deltaTime;

            if (order.timeRemaining <= 0)
            {
                Debug.Log("Pedido EXPIRADO: " + order.requestedItem);

                // remove pedido
                activeOrders.RemoveAt(i);

                //  NÃO gera novo automaticamente
                // robôs vão gerar novos pedidos depois

                // futura penalidade aqui
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
            string color = GetOrderUrgency(order);

            // piscar se estiver crítico
            if (color == "red")
            {
                bool blink = Mathf.FloorToInt(blinkTimer * 30) % 2 == 0;
                color = blink ? "red" : "yellow";
            }

            text += "<color=" + color + ">";
            // mostra todos os itens do pedido
            for (int j = 0; j < order.requestedItems.Count; j++)
            {
                text += order.requestedItems[j].ToString();

                if (j < order.requestedItems.Count - 1)
                text += ", ";
            }
            text += " -> ";
            text += Mathf.Ceil(order.timeRemaining) + "s";
            text += "</color>\n";
        }

        ordersText.text = text;
    }
}