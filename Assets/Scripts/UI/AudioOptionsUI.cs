using UnityEngine;
using UnityEngine.UI;

public class AudioOptionsUI : MonoBehaviour
{
    [Header("Music")]

    // slider da música
    public Slider musicSlider;

    // slider dos efeitos sonoros
    public Slider sfxSlider;

    void Start()
    {
        // ===== MUSIC =====

        float savedMusicVolume =
            PlayerPrefs.GetFloat("MusicVolume", 1f);

        musicSlider.value = savedMusicVolume;

        musicSlider.onValueChanged.AddListener(
            SetMusicVolume
        );

        // ===== SFX =====

        float savedSFXVolume =
            PlayerPrefs.GetFloat("SFXVolume", 1f);

        sfxSlider.value = savedSFXVolume;

        sfxSlider.onValueChanged.AddListener(
            SetSFXVolume
        );
    }

    // altera volume da música
    public void SetMusicVolume(float volume)
    {
        // aplica no menu
        if (MenuMusicManager.Instance != null)
        {
            MenuMusicManager.Instance
                .SetMusicVolume(volume);
        }

        // aplica na gameplay
        if (PhaseMusicManager.Instance != null)
        {
            PhaseMusicManager.Instance
                .SetMusicVolume(volume);
        }
    }

    // altera volume dos efeitos sonoros
    public void SetSFXVolume(float volume)
    {
        // verifica se existe SFXManager
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.SetSFXVolume(volume);
        }
    }
}