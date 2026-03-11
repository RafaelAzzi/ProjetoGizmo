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

        // move visualmente para o ponto correto
        Transform holdPoint = newHolder.GetHoldPoint();

        transform.SetParent(holdPoint, true);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // desativa física
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
}