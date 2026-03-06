using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    private Item item;

    void Awake()
    {
        // Pega o script Item que está no mesmo objeto
        item = GetComponent<Item>();
    }

    // ===== INTERAÇÃO DO PLAYER =====
    public void Interact(Player player)
    {
        // Se o player já estiver segurando algo, não pega
        if (player.GetHeldItem() != null) return;

        Rigidbody rb = item.GetComponent<Rigidbody>();

        // Desativa física
        rb.useGravity = false;
        rb.isKinematic = true;

        // Move o item para a mão do player
        item.transform.position = player.holdPoint.position;

        // Faz o item virar filho da mão
        item.transform.parent = player.holdPoint;

        // Player passa a segurar esse item
        player.SetHeldItem(item);
    }
}