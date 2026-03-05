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
}