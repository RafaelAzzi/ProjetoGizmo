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

    [Header("Cores do Popup")]
    public Color positiveColor = Color.green;
    public Color negativeColor = Color.red;

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
            if (currentDisplayedScore >= 0)
            {
                // GANHOU pontos
                if (gainedAmount > 0)
                {
                    ShowPopup(
                        gainedAmount,
                        positiveColor
                    );

                    // anima score positivo
                    StartCoroutine(
                        AnimateScore(positiveColor)
                    );
                }

                // PERDEU pontos
                else if (gainedAmount < 0)
                {
                    ShowPopup(
                        gainedAmount,
                        negativeColor
                    );

                    // anima score negativo
                    StartCoroutine(
                        AnimateScore(negativeColor)
                    );
                }
            }

            // atualiza texto
            scoreText.text = realScore.ToString();

            // salva score atual
            currentDisplayedScore = realScore;
        }
    }

    // ===== POPUP =====
    void ShowPopup(int amount, Color popupColor)
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
            popupScript.Setup(amount, popupColor);
        }
    }

    // ===== ANIMAÇÃO DO SCORE =====
    IEnumerator AnimateScore(Color flashColor)
    {
        // aumenta escala
        scoreText.transform.localScale = originalScale * 1.2f;

        // muda cor
        scoreText.color = flashColor;

        // espera
        yield return new WaitForSecondsRealtime(0.3f);

        // volta ao normal
        scoreText.transform.localScale = originalScale;
        scoreText.color = originalColor;
    }
}