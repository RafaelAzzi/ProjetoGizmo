using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;

    [Header("Quantidade de itens por pedido")]
    public int minItemsPerOrder = 1;
    public int maxItemsPerOrder = 1;

    [Header("Tempo por raridade")]
    public float commonOrderTime = 30f;
    public float rareOrderTime = 45f;
    public float legendaryOrderTime = 60f;

    [Header("Lista de todos os prefabs de itens")]
    public List<Item> allItems;

    // lista de pedidos ativos
    public List<Order> activeOrders = new List<Order>();

    // itens que podem ser pedidos
    public ItemType[] possibleItems;

    // quantidade máxima de pedidos simultâneos
    public int maxOrders = 3;

    void Awake()
    {
        // garante que só existe um OrderManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

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

        // calcula tempo total baseado nos itens do pedido
        float totalOrderTime = CalculateOrderTime(newOrder.requestedItems);

        // define tempo do pedido
        newOrder.maxTime = totalOrderTime;
        newOrder.timeRemaining = totalOrderTime;

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

    // calcula tempo do pedido baseado na raridade dos itens
    float CalculateOrderTime(List<ItemType> items)
    {
        float totalTime = 0f;

        // percorre todos os itens do pedido
        foreach (ItemType itemType in items)
        {
            // procura o prefab correspondente
            foreach (Item itemPrefab in allItems)
            {
                // encontrou o item correto
                if (itemPrefab.itemType == itemType)
                {
                    // adiciona tempo baseado na raridade
                    switch (itemPrefab.rarity)
                    {
                        case Rarity.Comum:
                            totalTime += commonOrderTime;
                            break;

                        case Rarity.Raro:
                            totalTime += rareOrderTime;
                            break;

                        case Rarity.Lendario:
                            totalTime += legendaryOrderTime;
                            break;
                    }

                    // já encontrou, então para o loop
                    break;
                }
            }
        }

        return totalTime;
    }

    

    void Update()
    {
        // não atualiza pedidos se a partida acabou
        if (!GameManager.Instance.IsGamePlaying())
            return;
            
        UpdateOrders();
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
                // mostra todos os itens do pedido
                string items = "";

                foreach (var item in order.requestedItems)
                {
                    items += item.ToString() + " ";
                }

                Debug.Log("Pedido EXPIRADO: " + items);

                // ===== PENALIDADE =====
                ScoreManager.Instance.AddCustomScore(-10);

                // ===== CONTABILIZA FALHA =====
                GameStatsManager.Instance.ordersFailed++;

                // remove pedido
                activeOrders.RemoveAt(i);

                //  NÃO gera novo automaticamente
                // robôs vão gerar novos pedidos depois

                // futura penalidade aqui
            }
        }
    } 
}