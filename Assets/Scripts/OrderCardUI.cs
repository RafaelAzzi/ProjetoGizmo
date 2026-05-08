using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderCardUI : MonoBehaviour
{
    [Header("UI Principal")]

    // ícone principal do item
    public Image mainItemIcon;

    // nome do item
    public TextMeshProUGUI itemNameText;

    // texto do timer
    public TextMeshProUGUI orderTimerText;

    [Header("Recipe Panel")]

    // painel da receita
    public GameObject recipePanel;

    // ícones da receita
    public Image ingredientAIcon;
    public Image ingredientBIcon;
    public Image resultIcon;

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

        // pega primeiro item do pedido
        ItemType itemType = currentOrder.requestedItems[0];

        // atualiza ícone principal
        mainItemIcon.sprite =
            visualDatabase.GetIcon(itemType);

        // atualiza nome
        itemNameText.text =
            visualDatabase.GetDisplayName(itemType);

        // tenta mostrar receita
        TryShowRecipe(itemType);
    }

    // atualiza timer
    void UpdateTimer()
    {
        // atualiza texto
        orderTimerText.text =
            Mathf.Ceil(currentOrder.timeRemaining) + "s";

        // porcentagem restante
        float percent =
            currentOrder.timeRemaining /
            currentOrder.maxTime;

        // verde
        if (percent > 0.5f)
        {
            orderTimerText.color = Color.green;
        }
        // amarelo
        else if (percent > 0.25f)
        {
            orderTimerText.color = Color.yellow;
        }
        // vermelho piscando
        else
        {
            blinkTimer += Time.deltaTime;

            bool blink =
                Mathf.FloorToInt(blinkTimer * 8) % 2 == 0;

            orderTimerText.color =
                blink ? Color.red : Color.yellow;
        }
    }

    // tenta mostrar receita
    void TryShowRecipe(ItemType resultType)
    {
        Recipe foundRecipe = null;

        // procura receita correspondente
        foreach (Recipe recipe in recipeDatabase.recipes)
        {
            // pega item resultante
            Item resultItem =
                recipe.resultPrefab.GetComponent<Item>();

            // segurança
            if (resultItem == null)
                continue;

            // encontrou receita
            if (resultItem.itemType == resultType)
            {
                foundRecipe = recipe;
                break;
            }
        }

        // item comum → sem receita
        if (foundRecipe == null)
        {
            recipePanel.SetActive(false);
            return;
        }

        // ativa painel
        recipePanel.SetActive(true);

        // ingrediente A
        ingredientAIcon.sprite =
            visualDatabase.GetIcon(foundRecipe.itemA);

        // ingrediente B
        ingredientBIcon.sprite =
            visualDatabase.GetIcon(foundRecipe.itemB);

        // resultado
        resultIcon.sprite =
            visualDatabase.GetIcon(resultType);
    }
}