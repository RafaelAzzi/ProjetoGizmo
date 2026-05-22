using System.Collections.Generic;
using UnityEngine;

public class OrderUI : MonoBehaviour
{
    [Header("Referências")]

    // container onde os cards ficarão
    public Transform ordersContainer;

    // prefab visual do card
    public OrderCardUI orderCardPrefab;

    // banco visual
    public ItemVisualDatabase visualDatabase;

    // database de receitas
    public RecipeDatabase recipeDatabase;

    // manager de pedidos
    private OrderManager orderManager;

    // relação pedido -> card
    private Dictionary<Order, OrderCardUI> activeCards =
        new Dictionary<Order, OrderCardUI>();

    void Start()
    {
        // pega singleton
        orderManager = OrderManager.Instance;
    }

    void Update()
    {
        // segurança
        if (orderManager == null)
            return;

        // atualiza cards
        RefreshOrders();
    }

    // atualiza lista visual de pedidos
    void RefreshOrders()
    {
        // cria cards que ainda não existem
        foreach (Order order in orderManager.activeOrders)
        {
            // já existe
            if (activeCards.ContainsKey(order))
                continue;

            CreateCard(order);
        }

        // lista temporária
        List<Order> ordersToRemove =
            new List<Order>();

        // lista temporária de cards
        List<OrderCardUI> cardsToDestroy =
            new List<OrderCardUI>();

        // verifica cards inválidos
        foreach (var pair in activeCards)
        {
            // pedido ainda existe
            if (orderManager.activeOrders.Contains(pair.Key))
                continue;

            // adiciona card para destruir depois
            cardsToDestroy.Add(pair.Value);

            // marca pedido para remover
            ordersToRemove.Add(pair.Key);
        }

        // remove do dicionário primeiro
        foreach (Order order in ordersToRemove)
        {
            activeCards.Remove(order);
        }

        // destrói cards depois
        foreach (OrderCardUI card in cardsToDestroy)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }
    }

    // cria card visual
    void CreateCard(Order order)
    {
        // instancia prefab
        OrderCardUI newCard =
            Instantiate(
                orderCardPrefab,
                ordersContainer);

        // configura card
        newCard.Setup(
            order,
            visualDatabase,
            recipeDatabase);

        // salva no dicionário
        activeCards.Add(order, newCard);
    }
}