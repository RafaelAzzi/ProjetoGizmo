using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int score = 0;

    // valores base
    public int perfectScore = 100;
    public int partialScore = 50;
    public int spoiledScore = 10;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // ===== ADICIONAR PONTOS =====
    public void AddScore(bool isPerfect, bool isSpoiled)
    {
        int points = 0;

        if (isSpoiled)
        {
            points = spoiledScore;
        }
        else if (isPerfect)
        {
            points = perfectScore;
        }
        else
        {
            points = partialScore;
        }

        score += points;

        Debug.Log("Pontuação atual: " + score);

        // atualiza GameManager (para estrelas)
        GameManager.Instance.score = score;
    }

    public int GetScore()
    {
        return score;
    }

    // adiciona valor direto (usado pelo prato)
    public void AddCustomScore(int points)
    {
        score += points;

        Debug.Log("Pontuação atual: " + score);

        GameManager.Instance.score = score;
    }
}
