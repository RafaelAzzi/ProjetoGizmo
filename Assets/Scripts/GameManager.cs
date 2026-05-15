using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // singleton

    public GameObject resultPanel;

    // ===== CONFIGURAÇÃO =====
    public float matchTime = 180f; // tempo total da fase

    // ===== ESTADO =====
    private float currentTime;
    private GameState currentState = GameState.WaitingToStart;
    private GameResult gameResult = GameResult.None;


    public int oneStarScore = 250;
    public int twoStarScore = 350;
    public int threeStarScore = 450;

    // ===== RESULTADO FINAL DETALHADO =====
    public class MatchResultData
    {
        public int comumItems;
        public int comumPoints;

        public int rareItems;
        public int rarePoints;

        public int legendaryItems;
        public int legendaryPoints;

        public int oils;
        public int oilPoints;

        public int ordersCompleted;
        public int ordersFailed;
        public int failedPenalty;

        public int totalScore;
    }

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

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        if (currentState != GameState.Playing) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            EndGame();
        }
    }

    // ===== INICIAR JOGO =====
    public void StartGame()
    {
        currentState = GameState.Playing;
        currentTime = matchTime;

        Debug.Log("Jogo começou!");
    }

    // ===== FINALIZAR JOGO =====
    void EndGame()
    {
        currentState = GameState.GameOver;

        int stars = CalculateStars();

        if (stars > 0)
            gameResult = GameResult.Victory;
        else
            gameResult = GameResult.Defeat;

        Debug.Log("Jogo terminou!");
        Debug.Log("Resultado: " + gameResult);
        Debug.Log("Estrelas: " + stars);

        MatchResultData result = CalculateMatchResult();

        Debug.Log("===== RESULTADO FINAL =====");
        Debug.Log("Comuns: " + result.comumItems + " -> " + result.comumPoints);
        Debug.Log("Raros: " + result.rareItems + " -> " + result.rarePoints);
        Debug.Log("Lendários: " + result.legendaryItems + " -> " + result.legendaryPoints);
        Debug.Log("Óleos: " + result.oils + " -> " + result.oilPoints);
        Debug.Log("Falhas: " + result.ordersFailed + " -> " + result.failedPenalty);
        Debug.Log("TOTAL: " + result.totalScore);

        resultPanel.GetComponent<ResultUI>().ShowResults();
    }

    // ===== CALCULAR ESTRELAS =====
    int CalculateStars()
    {
        int currentScore = ScoreManager.Instance.GetScore();

        if (currentScore >= threeStarScore) return 3;
        if (currentScore >= twoStarScore) return 2;
        if (currentScore >= oneStarScore) return 1;

        return 0;
    }

    // ===== GETTERS =====

    public bool IsGamePlaying()
    {
        return currentState == GameState.Playing;
    }

    public bool IsGameOver()
    {
        return currentState == GameState.GameOver;
    }

    public float GetTimeRemaining()
    {
        return currentTime;
    }

    public MatchResultData CalculateMatchResult()
    {
        MatchResultData data = new MatchResultData();

        // pega referência do novo manager de stats
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

        // ===== TOTAL =====
        data.totalScore = ScoreManager.Instance.GetScore();

        return data;
    }
}

// ===== ENUMS =====

public enum GameState
{
    WaitingToStart,
    Playing,
    GameOver
}

public enum GameResult
{
    None,
    Victory,
    Defeat
}