using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // ===== PROCESSA ENTREGA =====
    public bool TryDeliver(Order order, PlateItem plate, OrderManager orderManager)
    {
        // ===== VALIDAÇÕES =====
        if (order == null || order.requestedItems == null)
        {
            Debug.LogError("Pedido inválido!");
            return false;
        }

        // ===== COPIA LISTAS =====
        List<ItemType> orderItems = new List<ItemType>(order.requestedItems);
        List<ItemType> plateItems = new List<ItemType>(plate.GetItems());

        // ===== COMPARAÇÃO =====
        foreach (var plateItem in plateItems)
        {
            if (orderItems.Contains(plateItem))
            {
                orderItems.Remove(plateItem);
            }
            else
            {
                return false;
            }
        }

        if (orderItems.Count > 0)
        {
            return false;
        }

        // ===== SUCESSO =====
        Debug.Log("Pedido COMPLETO com prato!");

        GameStatsManager.Instance.ordersCompleted++;
        orderManager.activeOrders.Remove(order);

        List<Item> items = plate.GetItemObjects();

        foreach (Item item in items)
        {
            if (item.rarity == Rarity.Raro)
                GameStatsManager.Instance.rareItemsDelivered++;

            else if (item.rarity == Rarity.Lendario)
                GameStatsManager.Instance.legendaryItemsDelivered++;

            if (item.itemType == ItemType.OleoComum || item.itemType == ItemType.OleoAntiferrugem)
                GameStatsManager.Instance.oilsDelivered++;
        }

        int score = ScoreManager.Instance.CalculateOrderScore(
            items,
            order.timeRemaining,
            order.maxTime
        );

        ScoreManager.Instance.AddCustomScore(score);

        return true;
    }
}