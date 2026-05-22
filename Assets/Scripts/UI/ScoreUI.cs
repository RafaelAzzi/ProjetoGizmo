using TMPro;
using UnityEngine;
using System.Collections;

public class ScoreUI : MonoBehaviour
{
    [Header("Texto da pontuação")]
    public TextMeshProUGUI scoreText;

    [Header("Popup")]
    public GameObject scorePopupPrefab;

    [Header("Spawn do popup")]
    public Transform popupSpawnPoint;

    // score exibido atualmente
    private int currentDisplayedScore = -1;

    // guarda escala original
    private Vector3 originalScale;

    // guarda cor original
    private Color originalColor;

    void Start()
    {
        // salva valores originais
        originalScale = scoreText.transform.localScale;
        originalColor = scoreText.color;
    }

    void Update()
    {
        // segurança
        if (ScoreManager.Instance == null)
            return;

        int realScore = ScoreManager.Instance.GetScore();

        // detecta mudança de score
        if (realScore != currentDisplayedScore)
        {
            // calcula quanto ganhou
            int gainedAmount = realScore - currentDisplayedScore;

            // evita popup ao iniciar jogo
            if (currentDisplayedScore >= 0 && gainedAmount > 0)
            {
                ShowPopup(gainedAmount);

                // anima score
                StartCoroutine(AnimateScore());
            }

            // atualiza texto
            scoreText.text = realScore.ToString();

            // salva score atual
            currentDisplayedScore = realScore;
        }
    }

    // ===== POPUP =====
    void ShowPopup(int amount)
    {
        // cria popup
        GameObject popup = Instantiate(
            scorePopupPrefab,
            popupSpawnPoint.position,
            Quaternion.identity,
            popupSpawnPoint
        );

        // configura texto
        ScorePopup popupScript = popup.GetComponent<ScorePopup>();

        if (popupScript != null)
        {
            popupScript.Setup(amount);
        }
    }

    // ===== ANIMAÇÃO DO SCORE =====
    IEnumerator AnimateScore()
    {
        // aumenta escala
        scoreText.transform.localScale = originalScale * 1.2f;

        // muda cor
        scoreText.color = Color.green;

        // espera
        yield return new WaitForSecondsRealtime(0.3f);

        // volta ao normal
        scoreText.transform.localScale = originalScale;
        scoreText.color = originalColor;
    }
}