using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // singleton

    public GameObject resultPanel;

    // ===== ID LÓGICO DA FASE =====
    public int levelID = 1;

    // ===== CONFIGURAÇÃO =====
    public float matchTime = 180f; // tempo total da fase

    // ===== ESTADO =====
    private float currentTime;

    // tempo inicial configurado da fase
    private bool initializedTime = false;

    private GameState currentState = GameState.WaitingToStart;

    public int oneStarScore = 250;
    public int twoStarScore = 350;
    public int threeStarScore = 450;

    [Header("Countdown Final")]
    public AudioSource countdownAudioSource;
    public AudioClip countdownTickSFX;

    // guarda último segundo tocado
    private int lastCountdownSecond = -1;

    public PhaseEndUI phaseEndUI;

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
        currentTime = matchTime;

        initializedTime = true;

        Debug.Log("GameManager Start executou");

        // se não existir TutorialManager na cena,
        // inicia a partida normalmente
        if (FindObjectOfType<TutorialManager>() == null)
        {
            StartGame();

            if (PhaseMusicManager.Instance != null)
            {
                PhaseMusicManager.Instance.PlayPhaseMusic();
            }
        }
    }

    void Update()
    {
        if (currentState != GameState.Playing) return;

        currentTime -= Time.deltaTime;

        HandleFinalCountdown();

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            EndGame();
        }
    }

    // ===== INICIAR JOGO =====
    public void StartGame()
    {
        Debug.Log("StartGame executou");
        currentState = GameState.Playing;
        currentTime = matchTime;

        Debug.Log("Jogo começou!");
        
    }

    // ===== FINALIZAR JOGO =====
    void EndGame()
    {
        currentState = GameState.GameOver;

        // congela gameplay
        Time.timeScale = 0f;

        // processa resultado final da partida
        MatchResultManager.Instance.ProcessMatchResult();

        Debug.Log("Jogo terminou!");

        phaseEndUI.Show();
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

    // ===== TICK FINAL =====
    void HandleFinalCountdown()
    {
        // pega segundo inteiro atual
        int currentSecond = Mathf.CeilToInt(currentTime);

        // verifica últimos 10 segundos
        if (currentSecond <= 10 && currentSecond > 0)
        {
            // evita repetir no mesmo segundo
            if (currentSecond != lastCountdownSecond)
            {
                lastCountdownSecond = currentSecond;

                // toca som
                if (countdownAudioSource != null && countdownTickSFX != null)
                {
                    // pequena variação de pitch
                   //countdownAudioSource.pitch = Random.Range(0.95f, 1.05f);

                    countdownAudioSource.PlayOneShot(countdownTickSFX);
                }
            }
        }
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