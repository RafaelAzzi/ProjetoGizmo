using UnityEngine;

public class DeliveryPoint : MonoBehaviour, IInteractable
{
    public OrderManager orderManager;

    // ponto exato onde a entrega deve acontecer 
    public Transform deliveryPoint;

    // distância máxima para permitir entrega
    public float interactDistance = 2.5f;

    public void Interact(Player player)
    {
        // ===== NOVO: VERIFICA DISTÂNCIA ATÉ O PONTO DE ENTREGA =====
        float distance = Vector3.Distance(player.transform.position, deliveryPoint.position);

        if (distance > interactDistance)
        {
            return; // está longe do ponto de entrega
        }

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