using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // singleton

    // ===== CONFIGURAÇÃO =====
    public float matchTime = 120f; // tempo total da fase

    // ===== ESTADO =====
    private float currentTime;
    private GameState currentState = GameState.WaitingToStart;
    private GameResult gameResult = GameResult.None;

    // ===== ESTRELAS (TEMPORÁRIO - depois ligamos com score) =====
    public int score = 0;

    public int oneStarScore = 100;
    public int twoStarScore = 200;
    public int threeStarScore = 300;

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
    }

    // ===== CALCULAR ESTRELAS =====
    int CalculateStars()
    {
        if (score >= threeStarScore) return 3;
        if (score >= twoStarScore) return 2;
        if (score >= oneStarScore) return 1;

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