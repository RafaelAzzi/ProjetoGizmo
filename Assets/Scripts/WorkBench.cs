using UnityEngine;
using UnityEngine.UI;

public class WorkBench : MonoBehaviour, IInteractable, IItemHolder
{
    [Header("Configuração de cliques por raridade")]
    public int clicksRaro = 10;
    public int clicksLendario = 20;

    public Transform holdPoint;
    public RecipeDatabase recipeDatabase;

    public float interactDistance = 2.5f;

    // UI da barra
    public Slider progressBar;

    private Item currentItem;
    private Item secondItem;

    private bool isProcessing = false;

    private Recipe currentRecipe;

    private float currentProgress = 0f;
    private float requiredClicks = 10f; // base (ajustar depois)

    // ===== INTERAÇÃO =====
    public void Interact(Player player)
    {
        float distance = Vector3.Distance(player.transform.position, holdPoint.position);

        if (distance > interactDistance)
            return;

        // SE ESTÁ PROCESSANDO → só aceita cliques
        if (isProcessing)
        {
            ProcessClick();
            return;
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

        // bloqueios
        if (heldItem is PlateItem)
            return;

        if (heldItem.itemType == ItemType.OleoComum || 
            heldItem.itemType == ItemType.OleoAntiferrugem)
            return;

        // BANCADA VAZIA → coloca primeiro item
        if (!HasItem())
        {
            heldItem.SetHolder(this);
            return;
        }

        // JÁ TEM 1 ITEM → tenta iniciar processamento
        TryStartProcessing(player, heldItem);
    }

    // ===== INICIAR PROCESSO =====
    void TryStartProcessing(Player player, Item heldItem)
    {
        Recipe recipe = recipeDatabase.GetRecipe(currentItem.itemType, heldItem.itemType);

        if (recipe != null)
        {
            secondItem = heldItem;
            currentRecipe = recipe;

            currentItem.HideIcon();
            secondItem.HideIcon();

            // destrói os dois itens
            Destroy(currentItem.gameObject);
            Destroy(secondItem.gameObject);

            // limpa referências
            ClearItem();
            secondItem = null; //  proteção contra bug futuro

            // inicia processamento
            isProcessing = true;
            currentProgress = 0;

            // pega o item do resultado direto do prefab
            Item resultItem = recipe.resultPrefab.GetComponent<Item>();

            // define cliques baseado na raridade do resultado
            switch (resultItem.rarity)
            {
                case Rarity.Raro:
                requiredClicks = clicksRaro;
                break;

                case Rarity.Lendario:
                requiredClicks = clicksLendario;
                break;

                default:
                requiredClicks = 5; // segurança (caso tenha algo comum)
                break;
            }

            // ativa barra
            progressBar.gameObject.SetActive(true);
            progressBar.value = 0f; //  garante que começa zerada
        }
    }

    // ===== PROCESSO DE CLIQUE =====
    void ProcessClick()
    {
        if (!isProcessing) return; //  proteção extra

        currentProgress++;

        progressBar.value = currentProgress / requiredClicks;

        if (currentProgress >= requiredClicks)
        {
            FinishProcessing();
        }
    }

    // ===== FINALIZA PROCESSO =====
    void FinishProcessing()
    {
        isProcessing = false;

        // reseta barra antes de esconder
        progressBar.value = 0f;

        // desativa barra
        progressBar.gameObject.SetActive(false);

        // cria resultado
        GameObject resultGO = Instantiate(
            currentRecipe.resultPrefab,
            holdPoint.position,
            Quaternion.identity
        );

        Item resultItem = resultGO.GetComponent<Item>();
        resultItem.SetHolder(this);

        resultItem.ShowIcon();

        // limpa dados
        currentRecipe = null;
        currentProgress = 0;
    }

    // ===== IItemHolder =====

    public Transform GetHoldPoint()
    {
        return holdPoint;
    }

    public void SetItem(Item item)
    {
        currentItem = item;
        if (item != null)
        {
            item.ShowIcon(); // mostra ícone
        }
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