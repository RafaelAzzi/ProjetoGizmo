using UnityEngine;

// Classe base para TODAS as estações do jogo
// Spawn, Storage, Craft, Trash vão herdar daqui
public class Station : MonoBehaviour, IInteractable
{
    void Start()
    {
        Debug.Log("Station ativa em: " + gameObject.name);
    }
    // Ponto onde o item ficará na bancada
    public Transform itemPoint;

    // Item atualmente na estação
    protected Item currentItem;

    // ===== INTERAÇÃO COM PLAYER =====
    public virtual void Interact(Player player)
    {
        Debug.Log("Station Interact chamada em: " + gameObject.name + " ID:" + GetInstanceID());
        // Pega o item que o jogador está segurando
        Item heldItem = player.GetHeldItem();

        // Se o jogador estiver segurando um item
        if (heldItem != null)
        {
            Debug.Log("Player está segurando: " + heldItem.name);
            TryPlaceItem(player, heldItem); // tenta colocar item na estação
        }
        else
        {
            Debug.Log("Player não está segurando item, tentando pegar");
            TryTakeItem(player); // tenta pegar item da estação
        }
    }

    // ===== TENTAR COLOCAR ITEM =====
    protected virtual void TryPlaceItem(Player player, Item item)
    {
        // Se já tem item na estação não permite colocar
        if (currentItem != null)
        {
            Debug.Log("Estação já tem item!");
            return;
        } 

        Debug.Log("Colocando item na estação");

        // Guarda item na estação
        currentItem = item;

        // Remove item da mão do jogador
        player.ClearHeldItem();

        // Desativa física do item
        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // Move item para posição da estação
        item.transform.position = itemPoint.position;

        // Faz item virar filho da estação
        item.transform.SetParent(itemPoint);
    }

    // ===== TENTAR PEGAR ITEM =====
    protected virtual void TryTakeItem(Player player)
    {
        Debug.Log("currentItem antes de limpar: " + currentItem);
        Debug.Log("Tentando pegar item da estação");

        if (currentItem == null)
        {
            Debug.Log("Estação não tem item");
            return;
        }

        Debug.Log("Item pego da estação");

        Item item = currentItem;

        // LIMPA A ESTAÇÃO PRIMEIRO
        currentItem = null;
        Debug.Log("currentItem depois de limpar: " + currentItem);

        // REMOVE DO PONTO DA BANCADA
        item.transform.SetParent(null);

        // PLAYER PEGA
        player.PickupItem(item);
    }
}
