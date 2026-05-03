using UnityEngine;
using TMPro;

public class ResultUI : MonoBehaviour
{
    public TextMeshProUGUI rareText;
    public TextMeshProUGUI legendaryText;
    public TextMeshProUGUI oilText;
    public TextMeshProUGUI completedOrdersText;
    public TextMeshProUGUI failedOrdersText;
    public TextMeshProUGUI totalText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI itemsHeaderText;
    public TextMeshProUGUI ordersHeaderText;

    // imagens das estrelas
    public UnityEngine.UI.Image star1;
    public UnityEngine.UI.Image star2;
    public UnityEngine.UI.Image star3;

    // sprites
    public Sprite fullStar;
    public Sprite emptyStar;

    public void ShowResults()
    {
        var data = GameManager.Instance.CalculateMatchResult();

        rareText.text = "Itens raros: " + data.rareItems + " -> " + data.rarePoints;
        legendaryText.text = "Itens lendários: " + data.legendaryItems + " -> " + data.legendaryPoints;
        oilText.text = "Óleos: " + data.oils + " -> " + data.oilPoints;

        completedOrdersText.text = "Pedidos completos: " + data.ordersCompleted;
        failedOrdersText.text = "Pedidos falhados: " + data.ordersFailed + " -> " + data.failedPenalty;

        titleText.text = "RESULTADO";
        itemsHeaderText.text = "ITENS";
        ordersHeaderText.text = "PEDIDOS";

        totalText.text = "TOTAL\n" + data.totalScore;

        int currentScore = ScoreManager.Instance.GetScore();

        int stars = currentScore >= GameManager.Instance.threeStarScore ? 3 :
                    currentScore >= GameManager.Instance.twoStarScore ? 2 :
                    currentScore >= GameManager.Instance.oneStarScore ? 1 : 0;

        SetStars(stars);

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
}
