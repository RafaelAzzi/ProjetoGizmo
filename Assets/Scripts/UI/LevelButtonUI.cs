using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButtonUI : MonoBehaviour
{
    [Header("Configuração")]
    public int levelIndex;

    [Header("Referências")]
    public Button levelButton;

    public GameObject lockIcon;

    public TMP_Text recordText;

    [Header("Estrelas")]
    public Image star1;
    public Image star2;
    public Image star3;

    [Header("Sprites")]
    public Sprite emptyStarSprite;
    public Sprite filledStarSprite;

    // ===== ATUALIZA UI =====
    public void RefreshUI()
    {
        // segurança
        if (ProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "ProgressManager não encontrado!"
            );

            return;
        }

        // verifica desbloqueio
        bool unlocked =
            ProgressManager.Instance
            .IsLevelUnlocked(levelIndex);

        // ===== LOCK =====

        // ativa/desativa cadeado
        if (lockIcon != null)
        {
            lockIcon.SetActive(!unlocked);
        }

        // ativa/desativa botão
        if (levelButton != null)
        {
            levelButton.interactable = unlocked;
        }

        // ===== RECORD =====

        int bestScore =
            ProgressManager.Instance
            .GetBestScore(levelIndex);

        if (recordText != null)
        {
            recordText.text =
                "Record: " + bestScore;
        }

        // ===== ESTRELAS =====

        int stars =
            ProgressManager.Instance
            .GetBestStars(levelIndex);

        UpdateStars(stars);
    }

    // ===== ATUALIZA ESTRELAS =====
    void UpdateStars(int stars)
    {
        // estrela 1
        if (star1 != null)
        {
            star1.sprite =
                stars >= 1
                ? filledStarSprite
                : emptyStarSprite;
        }

        // estrela 2
        if (star2 != null)
        {
            star2.sprite =
                stars >= 2
                ? filledStarSprite
                : emptyStarSprite;
        }

        // estrela 3
        if (star3 != null)
        {
            star3.sprite =
                stars >= 3
                ? filledStarSprite
                : emptyStarSprite;
        }
    }
}