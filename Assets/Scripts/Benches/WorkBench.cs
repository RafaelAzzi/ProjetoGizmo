using UnityEngine;
using UnityEngine.UI;

public class WorkBench : MonoBehaviour, IInteractable, IItemHolder
{
    [Header("Configuração de cliques por raridade")]
    public int clicksRaro = 10;
    public int clicksLendario = 20;

    [Header("Configuração da bancada")]
    public bool workBenchEnabled = true;

    [Header("Ícone de bancada vazia")]
    public GameObject emptyIconRoot;

    // permite criar itens lendários nessa fase
    public bool allowLegendaryCraft = true;

    public Transform holdPoint;

    // ponto onde a barra aparecerá
    public Transform uiAnchor;
    public RecipeDatabase recipeDatabase;

    // prefab da barra de progresso
    public GameObject progressBarPrefab;

    // instância atual da barra
    private GameObject progressBarInstance;

    // slider da barra instanciada
    private Slider progressBar;

    public float interactDistance = 2.5f;

    private Item currentItem;
    private Item secondItem;

    private bool isProcessing = false;

    private Recipe currentRecipe;

    private float currentProgress = 0f;
    private float requiredClicks = 10f; // base (ajustar depois)

    void Start()
    {
        UpdateEmptyIcon();
    }

    // ===== INTERAÇÃO =====
    public void Interact(Player player)
    {
        // verifica se a bancada está habilitada nessa fase
        if (!workBenchEnabled)
        {
            return;
        }

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
            // pega o item do resultado direto do prefab
            Item resultItem = recipe.resultPrefab.GetComponent<Item>();

            // bloqueia craft lendário se estiver desativado
            if (!allowLegendaryCraft && resultItem.rarity == Rarity.Lendario)
            {
                Debug.Log("Craft lendário bloqueado nessa fase");
                return;
            }

            secondItem = heldItem;
            currentRecipe = recipe;

            currentItem.HideIcon();
            secondItem.HideIcon();

            // destrói os dois itens
            Destroy(currentItem.gameObject);
            Destroy(secondItem.gameObject);

            // inicia processamento ANTES
            // de limpar item para evitar
            // mostrar o ícone "+"
            isProcessing = true;

            // limpa referências
            ClearItem();
            secondItem = null;

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

            // cria barra de progresso
            progressBarInstance = Instantiate(
                progressBarPrefab,
                uiAnchor.position,
                uiAnchor.rotation
            );

            // pega slider dentro da prefab
            progressBar =
                progressBarInstance.GetComponentInChildren<Slider>();

            // começa zerada
            progressBar.value = 0f;
        }
    }

    // ===== PROCESSO DE CLIQUE =====
    void ProcessClick()
    {
        if (!isProcessing) return; //  proteção extra

        currentProgress++;

        // toca som de clique do crafting
        SFXManager.Instance.PlaySFX(
            SFXType.WorkbenchClick
        );

        if (progressBar != null)
        {
            progressBar.value =
                currentProgress / requiredClicks;
        }

        if (currentProgress >= requiredClicks)
        {
            FinishProcessing();
        }
    }

    // ===== FINALIZA PROCESSO =====
    void FinishProcessing()
    {
        // toca som de craft concluído
        SFXManager.Instance.PlaySFX(
            SFXType.WorkbenchComplete
        );
        
        isProcessing = false;

        // destrói barra de progresso
        if (progressBarInstance != null)
        {
            Destroy(progressBarInstance);
        }

        // limpa referências da barra
        progressBarInstance = null;
        progressBar = null;

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

        // atualiza visual da bancada
        UpdateEmptyIcon();
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

        // atualiza ícone da bancada
        UpdateEmptyIcon();
    }

    public Item GetItem()
    {
        return currentItem;
    }

    public void ClearItem()
    {
        currentItem = null;

        // atualiza ícone da bancada
        UpdateEmptyIcon();
    }

    public bool HasItem()
    {
        return currentItem != null;
    }

    // retorna se a bancada pode ser utilizada
    public bool IsWorkbenchEnabled()
    {
        return workBenchEnabled;
    }

    // atualiza visual do ícone de bancada vazia
    void UpdateEmptyIcon()
    {
        // segurança
        if (emptyIconRoot == null)
            return;

        // mostra apenas se:
        // - não tiver item
        // - não estiver processando
        bool shouldShow =
            workBenchEnabled &&
            !HasItem() &&
            !isProcessing;

        emptyIconRoot.SetActive(shouldShow);
    }
}