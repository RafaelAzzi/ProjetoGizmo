using UnityEngine;

public class PhaseMusicManager : MonoBehaviour
{
    // singleton simples para acessar música atual
    public static PhaseMusicManager Instance;

    // referência do AudioSource
    private AudioSource audioSource;

    [Header("Dynamic Pitch")]

    // ativa/desativa aceleração da música
    public bool enableDynamicPitch = true;

    // velocidade normal
    public float normalPitch = 1f;

    // velocidade média
    public float fastPitch = 1.25f;

    // velocidade crítica
    public float veryFastPitch = 1.5f;

    [Header("Time Thresholds")]

    // quando faltar X segundos
    public float fastTimeThreshold = 30f;

    // quando faltar X segundos
    public float veryFastTimeThreshold = 15f;

    // evita parar a música várias vezes
    private bool musicStopped = false;

    void Awake()
    {
        // salva referência singleton
        Instance = this;
    }

    void Start()
    {
        // pega AudioSource
        audioSource = GetComponent<AudioSource>();

        // aplica volume salvo
        float savedVolume =
            PlayerPrefs.GetFloat("MusicVolume", 1f);

        audioSource.volume = savedVolume;

        // velocidade inicial
        audioSource.pitch = normalPitch;
    }

    void Update()
    {
        // se pitch dinâmico estiver desligado
        // não faz lógica de aceleração
        if (!enableDynamicPitch)
            return;

        // segurança
        if (GameManager.Instance == null)
            return;

        // verifica se jogo terminou
        if (GameManager.Instance.IsGameOver())
        {
            // evita chamar Stop infinitamente
            if (!musicStopped)
            {
                audioSource.Stop();
                musicStopped = true;
            }

            return;
        }

        // pega tempo restante
        float timeRemaining =
            GameManager.Instance.GetTimeRemaining();

        // velocidade crítica
        if (timeRemaining <= veryFastTimeThreshold)
        {
            audioSource.pitch = veryFastPitch;
        }

        // velocidade média
        else if (timeRemaining <= fastTimeThreshold)
        {
            audioSource.pitch = fastPitch;
        }

        // velocidade normal
        else
        {
            audioSource.pitch = normalPitch;
        }
    }

    // chamado pelo slider
    public void SetMusicVolume(float volume)
    {
        // altera volume atual
        audioSource.volume = volume;

        // salva volume
        PlayerPrefs.SetFloat("MusicVolume", volume);

        // garante salvamento imediato
        PlayerPrefs.Save();
    }
}