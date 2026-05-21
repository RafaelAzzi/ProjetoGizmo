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

    // texto de vitória/derrota
    public TextMeshProUGUI resultText;

    // botão de próxima fase
    public GameObject nextButton;

    // imagens das estrelas
    public Image star1;
    public Image star2;
    public Image star3;

    // sprites
    public Sprite fullStar;
    public Sprite emptyStar;

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
            // mostra texto de vitória
            resultText.text = "VITÓRIA";

            // habilita botão de continuar
            nextButton.SetActive(true);

            // toca música de vitória
            PhaseMusicManager.Instance.PlayVictoryMusic();
        }
        else
        {
            // mostra texto de derrota
            resultText.text = "DERROTA";

            // esconde botão de continuar
            nextButton.SetActive(false);

            // toca música de derrota
            PhaseMusicManager.Instance.PlayDefeatMusic();
        }

        gameObject.SetActive(true);
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
}