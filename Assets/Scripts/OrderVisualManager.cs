using System.Collections.Generic;
using UnityEngine;

// controla cores visuais dos pedidos
public class OrderVisualManager : MonoBehaviour
{
    // singleton
    public static OrderVisualManager Instance;

    [Header("Cores disponíveis")]

    // lista de cores usadas pelos pedidos
    public List<Color> orderColors = new List<Color>();

    // IDs atualmente em uso
    private HashSet<int> usedVisualIDs =
        new HashSet<int>();

    void Awake()
    {
        // garante singleton único
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // cria cores automaticamente
        SetupColors();
    }

   // cria palette inicial
    void SetupColors()
    {
        // azul forte escuro
        orderColors.Add(
            new Color(0.1f, 0.35f, 0.85f));

        // vermelho forte escuro
        orderColors.Add(
            new Color(0.85f, 0.15f, 0.15f));

        // verde petróleo
        orderColors.Add(
            new Color(0.1f, 0.55f, 0.25f));

        // laranja forte
        orderColors.Add(
            new Color(0.9f, 0.45f, 0.05f));

        // roxo forte
        orderColors.Add(
            new Color(0.5f, 0.2f, 0.85f));

        // ciano escuro
        orderColors.Add(
            new Color(0.05f, 0.65f, 0.75f));
    }

    // pega um ID livre
    public int ReserveVisualID()
    {
        // percorre todas as cores
        for (int i = 0; i < orderColors.Count; i++)
        {
            // encontrou ID livre
            if (!usedVisualIDs.Contains(i))
            {
                usedVisualIDs.Add(i);

                return i;
            }
        }

        // fallback
        Debug.LogWarning(
            "Sem Visual IDs disponíveis!");

        return -1;
    }

    // libera ID
    public void ReleaseVisualID(int id)
    {
        // segurança
        if (id < 0)
            return;

        // remove da lista usada
        usedVisualIDs.Remove(id);
    }

    // retorna cor pelo ID
    public Color GetColor(int id)
    {
        // segurança
        if (id < 0 || id >= orderColors.Count)
        {
            return Color.white;
        }

        return orderColors[id];
    }
}