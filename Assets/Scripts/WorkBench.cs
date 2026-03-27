using UnityEngine;

public class WorkBench : MonoBehaviour, IInteractable
{
    //  onde os itens ficam na bancada
    public Transform holdPoint;

    // referência ao banco de receitas
    public RecipeDatabase recipeDatabase;

    // primeiro item colocado
    private Item firstItem;


    // ===== INTERAÇÃO =====
    public void Interact(Player player)
    {
        Item heldItem = player.GetHeldItem();

        // ===== PLAYER SEM ITEM → tenta pegar resultado =====
        if (heldItem == null)
        {
            TakeResult(player);
            return;
        }

        // ===== PRIMEIRO ITEM =====
        if (firstItem == null)
        {
            PlaceFirstItem(player, heldItem);
            return;
        }

        // ===== SEGUNDO ITEM =====
        TryCombine(player, heldItem);
    }


    // ===== COLOCA PRIMEIRO ITEM =====
    void PlaceFirstItem(Player player, Item item)
    {
        firstItem = item;

        // remove item da mão do player
        player.SetItem(null);

        // desativa física
        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // posiciona na bancada
        item.transform.SetParent(holdPoint);
        item.transform.localPosition = Vector3.zero;
    }


    // ===== TENTA COMBINAR =====
    void TryCombine(Player player, Item secondItem)
    {
        // busca receita no database
        Recipe recipe = recipeDatabase.GetRecipe(firstItem.itemType, secondItem.itemType);

        // ===== SE EXISTE RECEITA =====
        if (recipe != null)
        {
            // destrói os dois itens usados
            Destroy(firstItem.gameObject);
            Destroy(secondItem.gameObject);

            // remove item da mão do player
            player.SetItem(null);

            // cria o resultado
            GameObject result = Instantiate(
                recipe.resultPrefab,
                holdPoint.position,
                Quaternion.identity
            );

            // coloca na bancada
            result.transform.SetParent(holdPoint);

            // limpa o primeiro item
            firstItem = null;

            return;
        }

        // ===== SE NÃO EXISTE RECEITA =====
        Debug.Log("Receita não encontrada!");
    }


    // ===== PEGAR RESULTADO =====
    void TakeResult(Player player)
    {
        // verifica se tem algo na bancada
        if (holdPoint.childCount == 0) return;

        Item item = holdPoint.GetChild(0).GetComponent<Item>();

        if (item == null) return;

        // remove da bancada
        item.transform.SetParent(null);

        // entrega pro player
        player.PickupItem(item);
    }
}