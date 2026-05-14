using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderCardUI : MonoBehaviour
{
    [Header("UI Principal")]

    // ícone principal do pedido
    public Image mainItemIcon;
    // preenchimento da barra de tempo
    public Image timerFill;

    [Header("Recipe Panel")]

    public RectTransform cardRect;

    public float baseHeight = 140;

    public float extraHeightPerRecipe = 40f;

    public float recipeStartY = 110;


    // container das receitas
    public GameObject recipeContainer;

    // prefab da linha de receita
    public RecipeRowUI recipeRowPrefab;

    // pedido atual
    private Order currentOrder;

    // banco visual
    private ItemVisualDatabase visualDatabase;

    // database de receitas
    private RecipeDatabase recipeDatabase;

    // timer de piscar
    private float blinkTimer;

    // configura o card
    public void Setup(
        Order order,
        ItemVisualDatabase visualDB,
        RecipeDatabase recipeDB)
    {
        // salva referências
        currentOrder = order;
        visualDatabase = visualDB;
        recipeDatabase = recipeDB;

        // atualiza visual
        RefreshVisual();
    }

    void Update()
    {
        // segurança
        if (currentOrder == null)
            return;

        // atualiza timer
        UpdateTimer();
    }

   // atualiza visual completo
    void RefreshVisual()
    {
        // segurança
        if (currentOrder.requestedItems.Count <= 0)
            return;

        // pega item principal
        ItemType mainItem =
            currentOrder.requestedItems[0];

        // atualiza ícone principal
        mainItemIcon.sprite =
            visualDatabase.GetIcon(mainItem);

        // monta receitas
        BuildRecipeChain(mainItem);
    }

    // atualiza timer visual
    void UpdateTimer()
    {
        // porcentagem restante
        float percent =
            currentOrder.timeRemaining /
            currentOrder.maxTime;

        // atualiza barra
        timerFill.fillAmount = percent;

        // verde
        if (percent > 0.5f)
        {
            timerFill.color = Color.green;
        }
        // amarelo
        else if (percent > 0.25f)
        {
            timerFill.color = Color.yellow;
        }
        // vermelho piscando
        else
        {
            blinkTimer += Time.deltaTime;

            bool blink =
                Mathf.FloorToInt(blinkTimer * 8) % 2 == 0;

            timerFill.color =
                blink ? Color.red : Color.yellow;
        }
    }

    // monta cadeia de receitas
    void BuildRecipeChain(ItemType resultType)
    {
        // limpa receitas antigas
        foreach (Transform child in recipeContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // busca receita principal
        Recipe recipe =
            FindRecipeByResult(resultType);

        // item comum → sem receita
        if (recipe == null)
        {
            recipeContainer.SetActive(false);
            return;
        }

        // ativa container
        recipeContainer.SetActive(true);

        // cria cadeia recursiva
        CreateRecipeRecursive(resultType);
    }

    // cria receitas recursivamente
    void CreateRecipeRecursive(ItemType resultType)
    {
        // procura receita
        Recipe recipe =
            FindRecipeByResult(resultType);

        // segurança
        if (recipe == null)
            return;

        // verifica ingrediente A
        CreateRecipeRecursive(recipe.itemA);

        // verifica ingrediente B
        CreateRecipeRecursive(recipe.itemB);

        // cria linha visual
        RecipeRowUI row =
            Instantiate(
                recipeRowPrefab,
                recipeContainer.transform);

        // configura ícones
        row.Setup(
            visualDatabase.GetIcon(recipe.itemA),
            visualDatabase.GetIcon(recipe.itemB));
    }

    // procura receita pelo resultado
    Recipe FindRecipeByResult(ItemType resultType)
    {
        foreach (Recipe recipe in recipeDatabase.recipes)
        {
            // pega item do prefab
            Item resultItem =
                recipe.resultPrefab.GetComponent<Item>();

            // segurança
            if (resultItem == null)
                continue;

            // encontrou
            if (resultItem.itemType == resultType)
            {
                return recipe;
            }
        }

        return null;
    }
    
}