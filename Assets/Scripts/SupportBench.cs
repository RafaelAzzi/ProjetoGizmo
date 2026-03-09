using UnityEngine;

public class SupportBench : MonoBehaviour, IInteractable
{
    public Transform slot1;
    public Transform slot2;

    private Item itemSlot1;
    private Item itemSlot2;

    public void Interact(Player player)
    {
        Debug.Log("Bench Interact");

        Item heldItem = player.GetHeldItem();

        // Jogador segurando item
        if (heldItem != null)
        {
            if (TryPlaceItem(heldItem))
            {
                player.ClearHeldItem();
            }
        }
        // Jogador sem item
        else
        {
            Item item = TakeItem();

            if (item != null)
            {
                player.PickupItem(item);
            }
        }
    }

    bool TryPlaceItem(Item item)
    {
        if (itemSlot1 == null)
        {
            itemSlot1 = item;

            item.transform.SetParent(slot1);
            item.transform.localPosition = Vector3.zero;
            item.transform.rotation = Quaternion.identity;

            return true;
        }

        if (itemSlot2 == null)
        {
            itemSlot2 = item;

            item.transform.SetParent(slot2);
            item.transform.localPosition = Vector3.zero;
            item.transform.rotation = Quaternion.identity;

            return true;
        }

        return false;
    }

    Item TakeItem()
    {
        if (itemSlot1 != null)
        {
            Item item = itemSlot1;
            itemSlot1 = null;

            item.transform.SetParent(null);
            return item;
        }

        if (itemSlot2 != null)
        {
            Item item = itemSlot2;
            itemSlot2 = null;

            item.transform.SetParent(null);
            return item;
        }

        return null;
    }
}