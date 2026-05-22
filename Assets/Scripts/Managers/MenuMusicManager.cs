using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MenuMusicManager : MonoBehaviour
{
    // singleton
    public static MenuMusicManager Instance;

    // AudioSource da música
    private AudioSource audioSource;

    [Header("Audio Mixer")]

    // mixer principal
    public AudioMixer audioMixer;

    void Awake()
    {
        // já existe outro manager?
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // salva singleton
        Instance = this;

        // NÃO destruir entre cenas
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // pega AudioSource
        audioSource = GetComponent<AudioSource>();

        // aplica volume salvo
        float savedVolume =
            PlayerPrefs.GetFloat("MusicVolume", 1f);

        SetMusicVolume(savedVolume);
    }

    void Update()
    {
        // segurança
        if (audioSource == null)
            return;

        // pausa música no pause
        if (
            PauseManager.Instance != null &&
            PauseManager.Instance.IsPaused()
        )
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }

            return;
        }

        // volta música ao despausar
        if (
            !audioSource.isPlaying &&
            Time.timeScale > 0f
        )
        {
            audioSource.UnPause();
        }
    }

    // altera volume da música
    public void SetMusicVolume(float volume)
    {
        // segurança
        if (audioMixer == null)
        {
            Debug.LogError(
                "AudioMixer não atribuído!"
            );

            return;
        }

        // evita log infinito
        volume =
            Mathf.Clamp(volume, 0.0001f, 1f);

        // converte para dB
        float dB =
            Mathf.Log10(volume) * 20f;

        // aplica no mixer
        audioMixer.SetFloat(
            "MusicVolume",
            dB
        );

        // salva
        PlayerPrefs.SetFloat(
            "MusicVolume",
            volume
        );

        PlayerPrefs.Save();
    }

    // destrói música ao entrar gameplay
    public void DestroyMusicManager()
    {
        Destroy(gameObject);
    }
}