using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; //  Para Slider

public class MainMenu : MonoBehaviour
{
    public GameObject optionsPanel; // Painel de opções
    public Slider musicSlider; // Slider da música
    public Slider sfxSlider; // Slider dos efeitos sonoros
    public GameObject comoJogarPanel;

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

    // ===== NOVO: abrir "Como Jogar" =====
    public void OpenComoJogar()
    {
        comoJogarPanel.SetActive(true); // Mostra painel
    }

    // ===== NOVO: fechar "Como Jogar" =====
    public void CloseComoJogar()
    {
        comoJogarPanel.SetActive(false); // Esconde painel
    }
}