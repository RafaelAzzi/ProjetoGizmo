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

        OrderManager.OnOrderCompleted += HandleOrderCompleted;
        OrderManager.OnOrderExpired += HandleOrderExpired;
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

    void OnDestroy()
    {
        OrderManager.OnOrderCompleted -= HandleOrderCompleted;
        OrderManager.OnOrderExpired -= HandleOrderExpired;
    }

    // pedido entregue
    void HandleOrderCompleted(Order order)
    {
        // segurança
        if (!activeCards.ContainsKey(order))
            return;

        // pega referência do card
        OrderCardUI card =
            activeCards[order];

        // remove do dicionário
        activeCards.Remove(order);

        // toca feedback visual
        card.PlaySuccessFeedback();
    }

    // pedido expirado
    void HandleOrderExpired(Order order)
    {
        // segurança
        if (!activeCards.ContainsKey(order))
            return;

        // pega referência do card
        OrderCardUI card =
            activeCards[order];

        // remove do dicionário
        activeCards.Remove(order);

        // toca feedback visual
        card.PlayFailFeedback();
    }
}