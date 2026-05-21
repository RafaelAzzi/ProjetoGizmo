using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    // referência ao texto na tela
    public TextMeshProUGUI timerText;

    void Update()
    {
        // só atualiza se o jogo estiver rolando
        if (!GameManager.Instance.IsGamePlaying()) return;

        // pega o tempo restante da fase
        float timeRemaining = GameManager.Instance.GetTimeRemaining();

        // evita número negativo
        if (timeRemaining < 0)
            timeRemaining = 0;

        // converte para minutos
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);

        // pega os segundos restantes
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        // formata para 2:08 por exemplo
        timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
    }
}