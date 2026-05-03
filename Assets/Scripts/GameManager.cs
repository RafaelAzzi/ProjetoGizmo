using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // singleton

    public GameObject resultPanel;

    // ===== CONFIGURAÇÃO =====
    public float matchTime = 120f; // tempo total da fase

    // ===== ESTADO =====
    private float currentTime;
    private GameState currentState = GameState.WaitingToStart;
    private GameResult gameResult = GameResult.None;


    public int oneStarScore = 100;
    public int twoStarScore = 200;
    public int threeStarScore = 300;

    // ===== ESTATÍSTICAS DA FASE =====

    // pedidos
    public int ordersCompleted = 0;
    public int ordersFailed = 0;

    // itens
    public int rareItemsDelivered = 0;
    public int legendaryItemsDelivered = 0;
    public int oilsDelivered = 0;

    // ===== RESULTADO FINAL DETALHADO =====
    public class MatchResultData
    {
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
        // por enquanto começa automático
        StartGame();
        // reset estatísticas
        ordersCompleted = 0;
        ordersFailed = 0;

        rareItemsDelivered = 0;
        legendaryItemsDelivered = 0;
        oilsDelivered = 0;
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

        // ===== ITENS RAROS =====
        data.rareItems = rareItemsDelivered;
        data.rarePoints = rareItemsDelivered * 50;

        // ===== ITENS LENDÁRIOS =====
        data.legendaryItems = legendaryItemsDelivered;
        data.legendaryPoints = legendaryItemsDelivered * 70;

        // ===== ÓLEOS =====
        data.oils = oilsDelivered;
        data.oilPoints = oilsDelivered * 80; // simplificado (depois podemos melhorar)

        // ===== PEDIDOS =====
        data.ordersCompleted = ordersCompleted;
        data.ordersFailed = ordersFailed;
        data.failedPenalty = ordersFailed * -10;

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