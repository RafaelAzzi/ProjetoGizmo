using UnityEngine;
using TMPro; // necessário pro TextMeshPro

public class OrderUI : MonoBehaviour
{
    public OrderManager orderManager; // referência ao sistema de pedidos
    public TextMeshProUGUI ordersText; // texto da tela

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        // texto que será exibido
        string text = "Pedidos:\n";

        // percorre todos os pedidos ativos
        foreach (Order order in orderManager.activeOrders)
        {
            text += "- " + order.requestedItem.ToString() + "\n";
        }

        // aplica no UI
        ordersText.text = text;
    }
}
