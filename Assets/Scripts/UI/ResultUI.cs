using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // necessário para trocar de cena

public class ResultUI : MonoBehaviour
{
    public TextMeshProUGUI comumText;
    public TextMeshProUGUI rareText;
    public TextMeshProUGUI legendaryText;
    public TextMeshProUGUI oilText;
    public TextMeshProUGUI completedOrdersText;
    public TextMeshProUGUI failedOrdersText;
    public TextMeshProUGUI totalText;
    public TextMeshProUGUI itemsHeaderText;
    public TextMeshProUGUI ordersHeaderText;

    [Header("Result Visuals")]

    // banner de vitória
    public GameObject victoryBanner;

    // banner de derrota
    public GameObject defeatBanner;

    [Header("Character Visual")]

    // imagem do personagem/robô
    public Image resultCharacterImage;

    // sprite do robô feliz
    public Sprite victoryCharacterSprite;

    // sprite do robô triste
    public Sprite defeatCharacterSprite;

    // botão de próxima fase
    public GameObject nextButton;

    // imagens das estrelas
    public Image star1;
    public Image star2;
    public Image star3;

    // sprites
    public Sprite fullStar;
    public Sprite emptyStar;

    [Header("Fade")]
    public CanvasGroup canvasGroup;

    public float fadeDuration = 0.4f;

    // ===== referência ao LevelLoader =====
    public LevelLoader levelLoader;

    public void ShowResults()
    {
        // pega resultado já processado
        var data = MatchResultManager.Instance.GetMatchData();

        comumText.text = "Itens comuns: " + data.comumItems + " -> " + data.comumPoints;
        rareText.text = "Itens raros: " + data.rareItems + " -> " + data.rarePoints;
        legendaryText.text = "Itens lendários: " + data.legendaryItems + " -> " + data.legendaryPoints;
        oilText.text = "Óleos: " + data.oils + " -> " + data.oilPoints;

        completedOrdersText.text = "Pedidos completos: " + data.ordersCompleted;
        failedOrdersText.text = "Pedidos falhados: " + data.ordersFailed + " -> " + data.failedPenalty;

        itemsHeaderText.text = "ITENS";
        ordersHeaderText.text = "PEDIDOS";

        totalText.text = "TOTAL\n" + data.totalScore;

        // pega estrelas calculadas pelo MatchResultManager
        int stars = MatchResultManager.Instance.GetStars();

        SetStars(stars);

       if (MatchResultManager.Instance.IsVictory())
        {
            // ativa banner de vitória
            victoryBanner.SetActive(true);

            // desativa banner de derrota
            defeatBanner.SetActive(false);

            // troca sprite do robô/personagem
            resultCharacterImage.sprite = victoryCharacterSprite;

            // habilita botão de continuar
            nextButton.SetActive(true);

            // toca música de vitória
            PhaseMusicManager.Instance.PlayVictoryMusic();
        }
        else
        {
            // desativa banner de vitória
            victoryBanner.SetActive(false);

            // ativa banner de derrota
            defeatBanner.SetActive(true);

            // troca sprite do robô/personagem
            resultCharacterImage.sprite = defeatCharacterSprite;

            // esconde botão de continuar
            nextButton.SetActive(false);

            // toca música de derrota
            PhaseMusicManager.Instance.PlayDefeatMusic();
        }

        gameObject.SetActive(true);

        StartCoroutine(FadeIn());
    }

    void SetStars(int stars)
    {
        // define todas como vazias primeiro
        star1.sprite = emptyStar;
        star2.sprite = emptyStar;
        star3.sprite = emptyStar;

        // preenche conforme quantidade
        if (stars >= 1)
            star1.sprite = fullStar;

        if (stars >= 2)
            star2.sprite = fullStar;

        if (stars >= 3)
            star3.sprite = fullStar;
    }

    // ===== BOTÃO: PRÓXIMA FASE =====
    public void OnClickNext()
    {
        // carrega próxima fase
        levelLoader.LoadNextLevel();
    }

    // ===== BOTÃO: REINICIAR =====
    public void OnClickRestart()
    {
        // reinicia fase atual
        levelLoader.RestartLevel();
    }

    // ===== BOTÃO: SELEÇÃO DE FASE =====
    public void OnClickLevelSelect()
    {
        // carrega a cena de seleção de fases
        SceneManager.LoadScene("LevelSelect");
    }

    // ===== BOTÃO: MENU PRINCIPAL =====
    public void OnClickMenu()
    {
        // volta para o menu
        levelLoader.LoadMainMenu();
    }

    IEnumerator FadeIn()
    {
        float timer = 0f;

        // começa invisível
        canvasGroup.alpha = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / fadeDuration;

            // fade suave
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}