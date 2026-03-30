using UnityEngine;

public class DeliveryPoint : MonoBehaviour, IInteractable
{
    public OrderManager orderManager;

    public void Interact(Player player)
    {
        Item heldItem = player.GetHeldItem();

        // se player não tem item, sai
        if (heldItem == null) return;

        // tenta completar pedido
        bool success = orderManager.TryCompleteOrder(heldItem);

        if (success)
        {
            // remove item da mão do player
            player.SetItem(null);
        }
    }
}
