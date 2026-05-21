using UnityEngine;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int score = 0;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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

    }

    public int CalculateOrderScore(List<Item> items, float timeRemaining, float maxTime)
    {
        int total = 0;

        // ===== PONTOS POR ITEM =====
        foreach (Item item in items)
        {
            // ----- QUALIDADE -----
            switch (item.quality)
            {
                case ItemQuality.Perfect:
                    total += 80;
                    break;

                case ItemQuality.Overcooked:
                    total += 50;
                    break;

                case ItemQuality.Undercooked:
                        total += 40;
                        break;

                case ItemQuality.Crude:
                        total += 15;
                        break;
            }

            // ----- RARIDADE -----
            // item comum
            if (item.rarity == Rarity.Comum)
            {
                total += 20;
            }

            // item raro
            else if (item.rarity == Rarity.Raro)
            {
                total += 40;
            }

            // item lendário
            else if (item.rarity == Rarity.Lendario)
            {
                total += 60;
            }
        }

        // ===== BÔNUS DE TEMPO =====
        float percent = timeRemaining / maxTime;

        if (percent >= 0.7f)
        {
            total += 40;
        }
        else if (percent >= 0.5f)
        {
            total += 30;
        }
        else if (percent >= 0.3f)
        {
            total += 25;
        }
        else if (percent >= 0.15f)
        {
            total += 20;
        }
        else if (percent >= 0.05f)
        {
            total += 10;
        }

        return total;
    }
}
