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

    [Header("Visual do Pedido")]

    // imagem principal do card
    public Image cardBackground;

    [Header("Recipe Panel")]

    // rect do fundo do card
    public RectTransform cardRect;

    // altura do card sem receitas
    public float baseHeight = 140f;

    // quanto cada linha adiciona
    public float extraHeightPerRecipe = 40f;

    // posição inicial das receitas
    public float recipeStartY = -110f;

    // espaço entre receitas
    public float recipeSpacing = 35f;


    // container das receitas
    public GameObject recipeContainer;

    // prefab da linha de receita
    public RecipeRowUI recipeRowPrefab;

    // quantidade total de receitas atuais
    private int totalRecipeRows;

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

        // aplica cor visual do pedido
        ApplyVisualColor();

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
            // volta altura normal
            cardRect.sizeDelta =
                new Vector2(
                    cardRect.sizeDelta.x,
                    baseHeight);

            recipeContainer.SetActive(false);
            return;
        }

        // ativa container
        recipeContainer.SetActive(true);

        // quantidade de receitas
        int recipeCount =
            CountRecipesRecursive(resultType);

            // salva total atual
            totalRecipeRows = recipeCount;

        // calcula nova altura
        float newHeight =
            baseHeight +
            (recipeCount * extraHeightPerRecipe);

        // aplica altura
        cardRect.sizeDelta =
            new Vector2(
                cardRect.sizeDelta.x,
                newHeight);

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

                // pega rect da linha
                RectTransform rowRect =
                    row.GetComponent<RectTransform>();

                // índice atual
                int rowIndex =
                    recipeContainer.transform.childCount - 1;

                // altura total do bloco
                float totalHeight =
                    (totalRecipeRows - 1) * recipeSpacing;

                // ponto inicial centralizado
                float startOffset =
                    totalHeight / 2f;

                // posição final
                float posY =
                    recipeStartY +
                    startOffset -
                    (rowIndex * recipeSpacing);

                // aplica posição
                rowRect.anchoredPosition =
                    new Vector2(0, posY);

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

    // conta quantidade de receitas
    int CountRecipesRecursive(ItemType resultType)
    {
        // procura receita
        Recipe recipe =
            FindRecipeByResult(resultType);

        // item comum
        if (recipe == null)
            return 0;

        // conta esta receita
        int count = 1;

        // soma ingredientes
        count += CountRecipesRecursive(recipe.itemA);
        count += CountRecipesRecursive(recipe.itemB);

        return count;
    }

    // aplica cor visual do pedido
    void ApplyVisualColor()
    {
        // segurança
        if (cardBackground == null)
            return;

        // pega cor do pedido
        Color visualColor =
            OrderVisualManager.Instance
            .GetColor(currentOrder.visualID);

        // aplica na borda
        cardBackground.color = visualColor;
    }
    
}