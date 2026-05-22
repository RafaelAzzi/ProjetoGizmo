using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; //  Para Slider

public class MainMenu : MonoBehaviour
{
    public GameObject optionsPanel; // Painel de opções
    public GameObject comoJogarPanel;

    void Start()
    {
       
    }

    // Botão Jogar
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
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