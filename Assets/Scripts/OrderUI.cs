using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrderUI : MonoBehaviour
{
    public TextMeshProUGUI ordersText;

    private OrderManager orderManager;

    float blinkTimer = 0f;

    void Start()
    {
        // pega referência via Singleton
        orderManager = OrderManager.Instance;
    }

    void Update()
    {
        if (!GameManager.Instance.IsGamePlaying()) return;

        blinkTimer += Time.deltaTime;

        UpdateUI();
    }

    void UpdateUI()
    {
        if (ordersText == null || orderManager == null) return;

        string text = "Pedidos:\n";

        foreach (Order order in orderManager.activeOrders)
        {
            string color = GetOrderUrgency(order);

            // efeito de piscar
            if (color == "red")
            {
                bool blink = Mathf.FloorToInt(blinkTimer * 30) % 2 == 0;
                color = blink ? "red" : "yellow";
            }

            text += "<color=" + color + ">";

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

    string GetOrderUrgency(Order order)
    {
        float percent = order.timeRemaining / order.maxTime;

        if (percent > 0.5f)
            return "green";

        if (percent > 0.25f)
            return "yellow";

        return "red";
    }
}