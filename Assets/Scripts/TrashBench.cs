using UnityEngine;

// Bancada de lixo: serve apenas para destruir o item do player
public class TrashStation : MonoBehaviour, IInteractable
{
    // ponto de interação 
    public Transform interactPoint;

    // distância máxima para interagir
    public float interactDistance = 2.5f;

    // ===== INTERAÇÃO =====
    public void Interact(Player player)
    {
        // ===== VALIDA DISTÂNCIA  =====
        float distance = Vector3.Distance(player.transform.position, interactPoint.position);

        // se estiver longe demais, não permite interação
        if (distance > interactDistance)
        {
            return;
        }

        // verifica se o player tem item
        if (!player.HasItem())
        {
            return;
        }

        // pega o item do player
        Item item = player.GetItem();

        // segurança contra null
        if (item == null)
        {
            return;
        }

        // remove corretamente do sistema de holder
        item.SetHolder(null);

        // destrói o item
        Destroy(item.gameObject);
    }
}