using UnityEngine;

public class WorkBench : MonoBehaviour, IInteractable, IItemHolder
{
    public Transform holdPoint;
    public RecipeDatabase recipeDatabase;

    // distância máxima para interagir com o ponto de crafting
    public float interactDistance = 2.5f;

    private Item currentItem;

    // ===== INTERAÇÃO =====
    public void Interact(Player player)
    {

        // ===== NOVO: VERIFICA DISTÂNCIA ATÉ O HOLDPOINT =====
        float distance = Vector3.Distance(player.transform.position, holdPoint.position);

        if (distance > interactDistance)
        {
            return; // player está longe do ponto de crafting
        }

        Item heldItem = player.GetHeldItem();

        // PLAYER SEM ITEM → pega resultado
        if (heldItem == null)
        {
            if (HasItem())
            {
                GetItem().SetHolder(player);
            }
            return;
        }

        // BANCADA VAZIA → coloca item
        if (!HasItem())
        {
            heldItem.SetHolder(this);
            return;
        }

        // TENTA COMBINAR
        TryCombine(player, heldItem, currentItem);
    }

    // ===== COMBINAÇÃO =====
    void TryCombine(Player player, Item heldItem, Item benchItem)
    {
        Recipe recipe = recipeDatabase.GetRecipe(benchItem.itemType, heldItem.itemType);

        if (recipe != null)
        {
            // limpa referência antes de destruir
            ClearItem();

            Destroy(benchItem.gameObject);
            Destroy(heldItem.gameObject);

            // cria resultado
            GameObject resultGO = Instantiate(
                recipe.resultPrefab,
                holdPoint.position,
                Quaternion.identity
            );

            Item resultItem = resultGO.GetComponent<Item>();

            // coloca resultado na bancada usando o sistema correto
            resultItem.SetHolder(this);

            return;
        }

        Debug.Log("Receita não encontrada!");
    }

    // ===== IItemHolder =====

    public Transform GetHoldPoint()
    {
        return holdPoint;
    }

    public void SetItem(Item item)
    {
        currentItem = item;
    }

    public Item GetItem()
    {
        return currentItem;
    }

    public void ClearItem()
    {
        currentItem = null;
    }

    public bool HasItem()
    {
        return currentItem != null;
    }
}