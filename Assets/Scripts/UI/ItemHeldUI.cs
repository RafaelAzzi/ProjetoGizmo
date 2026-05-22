using TMPro;
using UnityEngine;

public class ItemHeldUI : MonoBehaviour
{
    public Player player; // referência ao player
    public TextMeshProUGUI text; // referência ao texto da UI

    void Update()
    {
        // pega o item que o player está segurando
        Item heldItem = player.GetHeldItem();

        if (heldItem != null)
        {
            // Atualiza o texto usando o nome de exibição
            text.text = "Item segurado: " + heldItem.displayName;
        }
        else
        {
            // se não tiver item
            text.text = "Item segurado: Nenhum";
        }
    }
}