using System.Collections;
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

    // overlay de feedback visual
    public Image feedbackOverlay;

    [Header("Recipe Panel")]

    // rect do fundo do card
    public RectTransform cardRect;

    // objeto visual que receberá animações
    public RectTransform visualRoot;

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

    // ===== SHAKE =====

    // intensidade do shake
    public float shakeAmount = 1f;

    // velocidade do shake
    public float shakeSpeed = 15f;

    // porcentagem para iniciar shake
    public float shakeThreshold = 0.25f;

    // posição original do card
    private Vector2 originalCardPosition;

    // controla estado do shake
    private bool isShaking;

    [Header("Feedback Visual")]

    // duração do feedback
    public float feedbackDuration = 0.35f;

    // alpha máxima do overlay
    public float feedbackAlpha = 0.45f;

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

        // salva posição original do card
        originalCardPosition =
            cardRect.anchoredPosition;
    }

    void Update()
    {
        // segurança
        if (currentOrder == null)
            return;

        // atualiza timer
        UpdateTimer();

        // atualiza shake visual
        UpdateShake();
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
            isShaking = false;

            timerFill.color = Color.green;
        }
        // amarelo
        else if (percent > 0.25f)
        {
            isShaking = false;

            timerFill.color = Color.yellow;
        }
        // vermelho piscando
        else
        {
            isShaking = true;

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

   // ===== SHAKE VISUAL =====
    void UpdateShake()
    {
        // segurança
        if (visualRoot == null)
            return;

        // se NÃO estiver tremendo
        if (!isShaking)
        {
            // volta posição visual normal
            visualRoot.anchoredPosition = Vector2.zero;

            return;
        }

        // gera offset horizontal
        float offsetX =
            Mathf.Sin(Time.time * shakeSpeed)
            * shakeAmount;

        // aplica shake SOMENTE no visual
        visualRoot.anchoredPosition =
            new Vector2(offsetX, 0);
    }

    // toca feedback de sucesso
    public void PlaySuccessFeedback()
    {
        StartCoroutine(
            PlayFeedbackCoroutine(
                new Color(0f, 1f, 0f),
                true));
    }

    // toca feedback de falha
    public void PlayFailFeedback()
    {
        StartCoroutine(
            PlayFeedbackCoroutine(
                new Color(1f, 0f, 0f),
                false));
    }

    // coroutine principal do feedback
    IEnumerator PlayFeedbackCoroutine(
        Color feedbackColor,
        bool success)
    {
        // segurança
        if (feedbackOverlay == null)
        {
            Destroy(gameObject);
            yield break;
        }

        // desativa shake durante feedback
        isShaking = false;

        // garante posição normal
        visualRoot.anchoredPosition = Vector2.zero;

        // timer
        float timer = 0f;

        while (timer < feedbackDuration)
        {
            timer += Time.deltaTime;

            // porcentagem
            float percent =
                timer / feedbackDuration;

            // fade do alpha
            float alpha =
                Mathf.Lerp(
                    feedbackAlpha,
                    0f,
                    percent);

            // aplica cor
            feedbackOverlay.color =
                new Color(
                    feedbackColor.r,
                    feedbackColor.g,
                    feedbackColor.b,
                    alpha);

            yield return null;
        }

        // destrói card
        Destroy(gameObject);
    }
}