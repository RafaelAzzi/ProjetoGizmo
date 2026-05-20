using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    // singleton global
    public static SFXManager Instance;

    [Header("Referências")]

    // database de efeitos sonoros
    public SFXDatabase sfxDatabase;

    // mixer principal
    public AudioMixer audioMixer;

    // AudioSource usado para tocar sons
    private AudioSource audioSource;

    void Awake()
    {
        // singleton simples
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        // pega AudioSource
        audioSource = GetComponent<AudioSource>();

        // aplica volume salvo
        float savedVolume =
            PlayerPrefs.GetFloat("SFXVolume", 1f);

        SetSFXVolume(savedVolume);
    }

    // toca efeito sonoro 2D
    public void PlaySFX(SFXType type)
    {
        // segurança
        if (sfxDatabase == null)
        {
            Debug.LogError("SFXDatabase não atribuída!");
            return;
        }

        // pega clip
        AudioClip clip =
            sfxDatabase.GetClip(type);

        // segurança
        if (clip == null)
            return;

        // toca som
        audioSource.PlayOneShot(clip);
    }

    // altera volume dos efeitos
    public void SetSFXVolume(float volume)
    {
        // segurança
        if (audioMixer == null)
        {
            Debug.LogError("AudioMixer não atribuído!");
            return;
        }

        // evita log infinito
        volume = Mathf.Clamp(volume, 0.0001f, 1f);

        // converte para dB
        float dB =
            Mathf.Log10(volume) * 20f;

        // aplica no mixer
        audioMixer.SetFloat("SFXVolume", dB);

        // salva
        PlayerPrefs.SetFloat(
            "SFXVolume",
            volume
        );

        PlayerPrefs.Save();
    }
}