using UnityEngine;

public class LevelSelectionUI : MonoBehaviour
{
    // ===== BOTÕES DAS FASES =====
    public LevelButtonUI[] levelButtons;

    void Start()
    {
        RefreshAllButtons();
    }

    // ===== ATUALIZA TODOS OS BOTÕES =====
    public void RefreshAllButtons()
    {
        foreach (LevelButtonUI button in levelButtons)
        {
            if (button != null)
            {
                button.RefreshUI();
            }
        }
    }
}