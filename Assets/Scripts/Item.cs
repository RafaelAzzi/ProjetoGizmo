using UnityEngine;

public enum ItemPhase 
{
    // --- FASE DOS ITENS --- 
    Fase1_Comum,
    Fase2_Raro,
    Fase3_Lendario
}

public enum Rarity
{
    // --- DEFINE A RARIDADE ---
    Comum,
    Raro,
    Lendario
}

public enum ItemType
{
    // --- FASE 1 (COMUNS) ---
    Sucata,
    Fiacao,
    Bateria,
    OleoComum,
    OleoAntiferrugem,

    // --- FASE 2 (RAROS) ---
    ChipComum,
    CircuitoRemendado,
    CelulaEnergia,
    Potencializador,
    SistemaEletrico,
    PlacaCircuitos,

    // --- FASE 3 (LENDÁRIOS) ---
    UnidadeUltraLogica,
    NucleoNanomassa,
    ChipAprimorado,
    ModuloAutomacao,
    Supercontrolador,
    TurboReator,
    CircuitoModeloX,
    RedeOptica,
    ProcessadorOmega,
    CelulaTesla,
    MatrizIonica,
    CoreIntraneural,
    SistemaStorm,
    PlacaOverdrive,
    MalhaQuantum

}

public class Item : MonoBehaviour
{
    [Header("Identidade do item")]
    public ItemType itemType;

    [Header("Classificação")]
    public ItemPhase phase;
    public Rarity rarity;



// ===== SISTEMA DE HOLDER =====

    private IItemHolder currentHolder;

    public void SetHolder(IItemHolder newHolder)
    {
        // evita refazer tudo se já estiver no mesmo holder
        if (currentHolder == newHolder)
        {
            return;
        }

        // remove do holder anterior
        if (currentHolder != null)
        {
            currentHolder.ClearItem();
        }

        // define novo holder
        currentHolder = newHolder;

        // se não tiver novo holder, para aqui
        if (newHolder == null) return;

        // registra item no novo holder
        newHolder.SetItem(this);

        // pega o ponto onde o item deve ficar
        Transform holdPoint = newHolder.GetHoldPoint();

        // segurança: evita erro se não tiver holdPoint
        if (holdPoint == null)
        {
            Debug.LogError("HoldPoint está NULL no holder: " + newHolder);
            return;
        }

        // move o item para o holder
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // desativa física para não cair ou colidir
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
}