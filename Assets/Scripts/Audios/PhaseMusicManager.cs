using UnityEngine;
using UnityEngine.Audio;

public class PhaseMusicManager : MonoBehaviour
{
    // singleton simples para acessar música atual
    public static PhaseMusicManager Instance;

    // referência do AudioSource
    private AudioSource audioSource;

    [Header("Audio Mixer")]

    // referência do mixer principal
    public AudioMixer audioMixer;

    [Header("Result Music")]

    // música de vitória
    public AudioClip victoryMusic;

    // música de derrota
    public AudioClip defeatMusic;

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

    // indica se música de resultado está tocando
    private bool playingResultMusic = false;

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

        SetMusicVolume(savedVolume);

        // velocidade inicial
        audioSource.pitch = normalPitch;
    }

    void Update()
    {
        // se pitch dinâmico estiver desligado
        // não faz lógica de aceleração
        if (!enableDynamicPitch)
            return;

            // se estiver tocando música final
            // ignora lógica da música da fase
            if (playingResultMusic)
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

    public void SetMusicVolume(float volume)
    {
        // segurança caso mixer não esteja configurado
        if (audioMixer == null)
        {
            Debug.LogError("AudioMixer não foi atribuído no PhaseMusicManager!");
            return;
        }
        // evita log de zero infinito
        volume = Mathf.Clamp(volume, 0.0001f, 1f);

        // converte volume linear para decibéis
        float dB = Mathf.Log10(volume) * 20f;

        // aplica no mixer
        audioMixer.SetFloat("MusicVolume", dB);

        // salva volume
        PlayerPrefs.SetFloat("MusicVolume", volume);

        // garante salvamento imediato
        PlayerPrefs.Save();
    }

    // ===== TOCAR MÚSICA DE VITÓRIA =====
    public void PlayVictoryMusic()
    {
        // ativa modo de música final
        playingResultMusic = true;

        // segurança
        if (victoryMusic == null)
        {
            Debug.LogWarning("Victory Music não atribuída!");
            return;
        }

        // troca música
        audioSource.Stop();
        audioSource.pitch = 1f;
        audioSource.clip = victoryMusic;

        // música de vitória toca apenas uma vez
        audioSource.loop = false;

        audioSource.Play();
    }

    // ===== TOCAR MÚSICA DE DERROTA =====
    public void PlayDefeatMusic()
    {
        // ativa modo de música final
        playingResultMusic = true;

        // segurança
        if (defeatMusic == null)
        {
            Debug.LogWarning("Defeat Music não atribuída!");
            return;
        }

        // troca música
        audioSource.Stop();
        audioSource.pitch = 1f;
        audioSource.clip = defeatMusic;
        audioSource.loop = false;
        audioSource.Play();
    }
}