using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    // singleton
    public static PauseManager Instance;

    [Header("UI")]
    public GameObject pausePanel;

    // janela principal do pause
    public GameObject pauseWindow;

    // painel de como jogar
    public GameObject howToPlayPanel;
    
    // painel de opções
    public GameObject optionsPanel;

    // botão de pause da HUD
    public GameObject pauseButton;

    // controla estado do pause
    private bool isPaused = false;

    void Awake()
    {
        // garante singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        // garante que começa fechado
        pausePanel.SetActive(false);

        // começa mostrando apenas janela do pause
        pauseWindow.SetActive(true);
        howToPlayPanel.SetActive(false);

        optionsPanel.SetActive(false);

        // garante tempo normal
        Time.timeScale = 1f;
    }

    void Update()
    {
        // não permite pause após fim de jogo
        if (GameManager.Instance.IsGameOver())
            return;

        // tecla ESC pausa/despausa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // se pausado -> volta
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // ===== PAUSAR =====
    public void PauseGame()
    {
        isPaused = true;

        // ativa UI
        pausePanel.SetActive(true);

        // esconde botão de pause
        pauseButton.SetActive(false);

        // sempre volta para menu principal do pause
        pauseWindow.SetActive(true);
        howToPlayPanel.SetActive(false);

        optionsPanel.SetActive(false);

        // congela jogo
        Time.timeScale = 0f;
    }

    // ===== VOLTAR AO JOGO =====
    public void ResumeGame()
    {
        isPaused = false;

        // esconde UI
        pausePanel.SetActive(false);

        // mostra botão novamente
        pauseButton.SetActive(true);

        // volta tempo normal
        Time.timeScale = 1f;
    }

    // ===== ABRIR COMO JOGAR =====
    public void OpenHowToPlay()
    {
        pauseWindow.SetActive(false);
        howToPlayPanel.SetActive(true);
    }

    // ===== ABRIR OPÇÕES =====
    public void OpenOptions()
    {
        pauseWindow.SetActive(false);
        howToPlayPanel.SetActive(false);

        optionsPanel.SetActive(true);
    }

    // ===== VOLTAR PARA PAUSE =====
    public void BackToPauseMenu()
    {
        howToPlayPanel.SetActive(false);
        optionsPanel.SetActive(false);

        pauseWindow.SetActive(true);
    }

    // ===== IR PARA MENU =====
    public void GoToMenu(string sceneName)
    {
        // MUITO IMPORTANTE
        // evita próxima cena congelada
        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneName);
    }

    // getter opcional
    public bool IsPaused()
    {
        return isPaused;
    }
}