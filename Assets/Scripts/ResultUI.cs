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
    public TextMeshProUGUI starsText;

    public void ShowResults()
    {
        var data = GameManager.Instance.CalculateMatchResult();

        rareText.text = "Itens raros: " + data.rareItems + " -> " + data.rarePoints;
        legendaryText.text = "Itens lendários: " + data.legendaryItems + " -> " + data.legendaryPoints;
        oilText.text = "Óleos: " + data.oils + " -> " + data.oilPoints;

        completedOrdersText.text = "Pedidos completos: " + data.ordersCompleted;
        failedOrdersText.text = "Pedidos falhados: " + data.ordersFailed + " -> " + data.failedPenalty;

        totalText.text = "TOTAL: " + data.totalScore;

        int stars = GameManager.Instance.score >= GameManager.Instance.threeStarScore ? 3 :
                    GameManager.Instance.score >= GameManager.Instance.twoStarScore ? 2 :
                    GameManager.Instance.score >= GameManager.Instance.oneStarScore ? 1 : 0;

        starsText.text = "Estrelas: " + stars;

        gameObject.SetActive(true);
    }
}
