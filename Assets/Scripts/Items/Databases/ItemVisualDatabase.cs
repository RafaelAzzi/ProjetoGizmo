using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemVisualData
{
    // tipo do item
    public ItemType itemType;

    // prefab do item
    public Item itemPrefab;
}

[CreateAssetMenu(menuName = "Gizmo/Item Visual Database")]
public class ItemVisualDatabase : ScriptableObject
{
    // lista de itens visuais
    public List<ItemVisualData> items;

    // pega prefab do item
    public Item GetItemPrefab(ItemType type)
    {
        foreach (ItemVisualData data in items)
        {
            if (data.itemType == type)
            {
                return data.itemPrefab;
            }
        }

        Debug.LogWarning("Item visual não encontrado: " + type);
        return null;
    }

    // pega sprite do item
    public Sprite GetIcon(ItemType type)
    {
        Item item = GetItemPrefab(type);

        if (item == null)
            return null;

        return item.iconSprite;
        }

    // pega raridade do item
    public Rarity GetRarity(ItemType type)
    {
        Item item = GetItemPrefab(type);

        if (item == null)
            return Rarity.Comum;

        return item.rarity;
    }

    // pega nome do item
    public string GetDisplayName(ItemType type)
    {
        return type.ToString();
    }
}
