using UnityEngine;

public class MatchResultManager : MonoBehaviour
{
    public static MatchResultManager Instance;

    // ===== RESULTADO FINAL =====
    private int starsEarned;
    private GameResult gameResult = GameResult.None;

    // ===== DADOS DETALHADOS =====
    private GameManager.MatchResultData matchData;

    void Awake()
    {
        // garante singleton
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // ===== PROCESSAR RESULTADO FINAL =====
    public void ProcessMatchResult()
    {
        // calcula estrelas
        starsEarned = CalculateStars();

        // define vitória ou derrota
        if (starsEarned > 0)
            gameResult = GameResult.Victory;
        else
            gameResult = GameResult.Defeat;

        // calcula dados detalhados
        matchData = CalculateMatchResult();

        Debug.Log("===== MATCH RESULT MANAGER =====");
        Debug.Log("Resultado: " + gameResult);
        Debug.Log("Stars: " + starsEarned);

        // ===== SALVAR PROGRESSO =====

        // verifica se ProgressManager existe
        if (ProgressManager.Instance != null)
        {
            // pega ID lógico da fase
            int currentLevelIndex =
                GameManager.Instance.levelID;

            // pega score atual
            int currentScore =
                ScoreManager.Instance.GetScore();

            // salva best score
            ProgressManager.Instance.SaveBestScore(
                currentLevelIndex,
                currentScore
            );

            // salva estrelas
            ProgressManager.Instance.SaveStars(
                currentLevelIndex,
                starsEarned
            );

            // desbloqueia próxima fase se venceu
            if (starsEarned > 0)
            {
                ProgressManager.Instance.UnlockLevel(
                    currentLevelIndex + 1
                );

                Debug.Log(
                    "Próxima fase desbloqueada!"
                );
            }
        }
        else
        {
            Debug.LogWarning(
                "ProgressManager não encontrado. " +
                "Abra o jogo pelo menu principal."
            );
        }
    }

    // ===== CALCULAR ESTRELAS =====
    int CalculateStars()
    {
        int currentScore = ScoreManager.Instance.GetScore();

        if (currentScore >= GameManager.Instance.threeStarScore)
            return 3;

        if (currentScore >= GameManager.Instance.twoStarScore)
            return 2;

        if (currentScore >= GameManager.Instance.oneStarScore)
            return 1;

        return 0;
    }

    // ===== CALCULAR DADOS DETALHADOS =====
    GameManager.MatchResultData CalculateMatchResult()
    {
        GameManager.MatchResultData data =
            new GameManager.MatchResultData();

        // pega stats da partida
        var stats = GameStatsManager.Instance;

        // ===== ITENS COMUNS =====
        data.comumItems = stats.comumItemsDelivered;
        data.comumPoints = stats.comumItemsDelivered * 20;

        // ===== ITENS RAROS =====
        data.rareItems = stats.rareItemsDelivered;
        data.rarePoints = stats.rareItemsDelivered * 50;

        // ===== ITENS LENDÁRIOS =====
        data.legendaryItems = stats.legendaryItemsDelivered;
        data.legendaryPoints = stats.legendaryItemsDelivered * 70;

        // ===== ÓLEOS =====
        data.oils = stats.oilsDelivered;
        data.oilPoints = stats.oilsDelivered * 80;

        // ===== PEDIDOS =====
        data.ordersCompleted = stats.ordersCompleted;
        data.ordersFailed = stats.ordersFailed;
        data.failedPenalty = stats.ordersFailed * -10;

        // ===== SCORE FINAL =====
        data.totalScore = ScoreManager.Instance.GetScore();

        return data;
    }

    // ===== GETTERS =====

    public int GetStars()
    {
        return starsEarned;
    }

    public GameResult GetResult()
    {
        return gameResult;
    }

    public bool IsVictory()
    {
        return gameResult == GameResult.Victory;
    }

    public bool IsDefeat()
    {
        return gameResult == GameResult.Defeat;
    }

    public GameManager.MatchResultData GetMatchData()
    {
        return matchData;
    }
}