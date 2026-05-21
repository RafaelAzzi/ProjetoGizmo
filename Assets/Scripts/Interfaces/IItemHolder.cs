using UnityEngine;

public interface IItemHolder
{
    Transform GetHoldPoint();

    void SetItem(Item item);

    Item GetItem();

    void ClearItem();

    bool HasItem();
}