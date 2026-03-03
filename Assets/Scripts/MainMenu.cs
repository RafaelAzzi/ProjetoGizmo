using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Necessário para Slider

public class MainMenu : MonoBehaviour
{
    public GameObject optionsPanel; // Painel de opções
    public Slider musicSlider; // Slider da música
    public Slider sfxSlider; // Slider dos efeitos sonoros

    void Start()
    {
        // valores iniciais
        musicSlider.value = AudioListener.volume;
        sfxSlider.value = 1f;
    }

    // Botão Jogar
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Botão Sair
    public void QuitGame()
    {
        Debug.Log("Saiu do jogo");
        Application.Quit();
    }

    // Botão Opções
    public void OpenOptions()
    {
        optionsPanel.SetActive(true); // Mostra painel
    }

    // Botão Fechar Opções
    public void CloseOptions()
    {
        optionsPanel.SetActive(false); // Esconde painel
    }

    // Ajusta volume da música
    public void SetMusicVolume(float volume)
    {
        AudioListener.volume = volume; // Controla volume global
    }

    // Ajusta volume de efeitos (por enquanto sem som)
    public void SetSFXVolume(float volume)
    {
        Debug.Log("Volume SFX: " + volume);
        // Depois conectar a um AudioManager
    }
}