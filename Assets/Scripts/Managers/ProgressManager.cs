using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;

    void Awake()
    {
        // ===== SINGLETON =====
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // mantém entre cenas
        DontDestroyOnLoad(gameObject);

        // inicializa progresso
        InitializeProgress();
    }

    // ===== INICIALIZA PROGRESSO =====
    void InitializeProgress()
    {
        Debug.Log("InitializeProgress chamado");

        Debug.Log(
            "Tem ProgressInitialized? " +
            PlayerPrefs.HasKey("ProgressInitialized")
        );
        // verifica se já inicializou anteriormente
        if (!PlayerPrefs.HasKey("ProgressInitialized"))
        {
            // desbloqueia primeira fase
            UnlockLevel(1);

            // marca inicialização
            PlayerPrefs.SetInt("ProgressInitialized", 1);

            // salva
            PlayerPrefs.Save();

            Debug.Log("Progresso inicial criado!");
        }
    }

    // ===== DESBLOQUEAR FASE =====
    public void UnlockLevel(int levelIndex)
    {
        PlayerPrefs.SetInt(
            GetUnlockedKey(levelIndex),
            1
        );

        PlayerPrefs.Save();
    }

    // ===== VERIFICA DESBLOQUEIO =====
    public bool IsLevelUnlocked(int levelIndex)
    {
        return PlayerPrefs.GetInt(
            GetUnlockedKey(levelIndex),
            0
        ) == 1;
    }

    // ===== SALVAR SCORE =====
    public void SaveBestScore(int levelIndex, int score)
    {
        int currentBest = GetBestScore(levelIndex);

        // salva apenas se for maior
        if (score > currentBest)
        {
            PlayerPrefs.SetInt(
                GetBestScoreKey(levelIndex),
                score
            );

            PlayerPrefs.Save();
        }
    }

    // ===== PEGAR BEST SCORE =====
    public int GetBestScore(int levelIndex)
    {
        return PlayerPrefs.GetInt(
            GetBestScoreKey(levelIndex),
            0
        );
    }

    // ===== SALVAR ESTRELAS =====
    public void SaveStars(int levelIndex, int stars)
    {
        int currentBest = GetBestStars(levelIndex);

        // salva apenas se for maior
        if (stars > currentBest)
        {
            PlayerPrefs.SetInt(
                GetBestStarsKey(levelIndex),
                stars
            );

            PlayerPrefs.Save();
        }
    }

    // ===== PEGAR ESTRELAS =====
    public int GetBestStars(int levelIndex)
    {
        return PlayerPrefs.GetInt(
            GetBestStarsKey(levelIndex),
            0
        );
    }

    // ===== KEYS =====

    string GetUnlockedKey(int levelIndex)
    {
        return "Level_" + levelIndex + "_Unlocked";
    }

    string GetBestScoreKey(int levelIndex)
    {
        return "Level_" + levelIndex + "_BestScore";
    }

    string GetBestStarsKey(int levelIndex)
    {
        return "Level_" + levelIndex + "_BestStars";
    }
}