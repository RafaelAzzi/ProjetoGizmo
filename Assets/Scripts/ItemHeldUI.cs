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
            // Remove o "(Clone)" do nome do objeto
            string itemName = heldItem.name.Replace("(Clone)", "");

            // Atualiza o texto
            text.text = "Item segurado: " + itemName;
        }
        else
        {
            // se não tiver item
            text.text = "Item segurado: Nenhum";
        }
    }
}