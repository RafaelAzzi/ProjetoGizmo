using UnityEngine;

public class WorkBench : MonoBehaviour, IInteractable
{
    // onde os itens ficam na bancada
    public Transform holdPoint;

    // referência ao banco de receitas
    public RecipeDatabase recipeDatabase;


    // ===== INTERAÇÃO =====
    public void Interact(Player player)
    {
        Item heldItem = player.GetHeldItem();
        Item benchItem = GetItemOnBench();

        // ===== PLAYER SEM ITEM → pega da bancada =====
        if (heldItem == null)
        {
            if (benchItem != null)
            {
                TakeResult(player);
            }
            return;
        }

        // ===== NÃO TEM ITEM NA BANCADA → coloca =====
        if (benchItem == null)
        {
            PlaceItem(player, heldItem);
            return;
        }

        // ===== TEM ITEM NA BANCADA → tenta combinar =====
        TryCombine(player, heldItem, benchItem);
    }


    // ===== PEGA ITEM REAL DA BANCADA =====
    Item GetItemOnBench()
    {
        if (holdPoint.childCount == 0) return null;

        return holdPoint.GetChild(0).GetComponent<Item>();
    }


    // ===== COLOCA ITEM NA BANCADA =====
    void PlaceItem(Player player, Item item)
    {
        // remove da mão do player
        player.SetItem(null);

        // desativa física
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // coloca na bancada
        item.transform.SetParent(holdPoint);
        item.transform.localPosition = Vector3.zero;
    }


    // ===== TENTA COMBINAR =====
    void TryCombine(Player player, Item heldItem, Item benchItem)
    {
        // busca receita
        Recipe recipe = recipeDatabase.GetRecipe(benchItem.itemType, heldItem.itemType);

        // ===== SE EXISTE RECEITA =====
        if (recipe != null)
        {
            Destroy(benchItem.gameObject);
            Destroy(heldItem.gameObject);

            player.SetItem(null);

            GameObject result = Instantiate(
                recipe.resultPrefab,
                holdPoint.position,
                Quaternion.identity
            );

            result.transform.SetParent(holdPoint);

            return;
        }

        // ===== SE NÃO EXISTE RECEITA =====
        Debug.Log("Receita não encontrada!");
    }


    // ===== PEGAR ITEM DA BANCADA =====
    void TakeResult(Player player)
    {
        Item item = GetItemOnBench();
        if (item == null) return;

        item.transform.SetParent(null);

        player.PickupItem(item);
    }
}