using UnityEngine;
using UnityEngine.UI;

public class AudioOptionsUI : MonoBehaviour
{
    [Header("Music")]

    // slider da música
    public Slider musicSlider;

    void Start()
    {
        // pega volume salvo
        float savedVolume =
            PlayerPrefs.GetFloat("MusicVolume", 1f);

        // atualiza valor visual do slider
        musicSlider.value = savedVolume;

        // adiciona listener no slider
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    // altera volume da música
    public void SetMusicVolume(float volume)
    {
        // verifica se existe MusicManager na cena
        if (PhaseMusicManager.Instance != null)
        {
            // altera volume atual
            PhaseMusicManager.Instance.SetMusicVolume(volume);
        }
    }
}