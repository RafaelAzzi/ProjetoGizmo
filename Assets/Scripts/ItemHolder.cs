using UnityEngine;

// IMPLEMENTA a interface IItemHolder
// Ele é o componente que pode segurar itens
public class ItemHolder : MonoBehaviour, IItemHolder
{
    // Ponto onde o item ficará visualmente
    public Transform holdPoint;

    // Item atualmente segurado
    private Item currentItem;

    // Retorna o ponto onde o item deve ficar
    public Transform GetHoldPoint()
    {
        return holdPoint;
    }

    // Define qual item está neste holder
    public void SetItem(Item item)
    {
        currentItem = item;
    }

    // Retorna o item atual
    public Item GetItem()
    {
        return currentItem;
    }

    // Limpa o item
    public void ClearItem()
    {
        currentItem = null;
    }

    // Verifica se tem item
    public bool HasItem()
    {
        return currentItem != null;
    }
}
