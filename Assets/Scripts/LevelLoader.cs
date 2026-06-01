using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    // ===== CARREGAR PRÓXIMA FASE =====
    public void LoadNextLevel()
    {
        // garante que a próxima cena não fique congelada
        Time.timeScale = 1f;

        // pega o índice da cena atual
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // calcula próxima cena
        int nextSceneIndex = currentSceneIndex + 1;

        // verifica se existe próxima fase
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // carrega próxima fase
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // se não tiver próxima, volta pro menu
            Debug.Log("Última fase atingida, voltando ao menu");

            SceneManager.LoadScene(0); // 0 = MainMenu
        }
    }

    // ===== REINICIAR FASE =====
    public void RestartLevel()
    {
        // garante tempo normal
        Time.timeScale = 1f;

        // recarrega a cena atual
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    // ===== IR PARA MENU =====
    public void LoadMainMenu()
    {
        // garante tempo normal
        Time.timeScale = 1f;

        SceneManager.LoadScene(0);
    }

   public void LoadLevel(int levelIndex)
    {
        // garante tempo normal
        Time.timeScale = 1f;
        
        // destrói música do menu
        // antes de entrar na gameplay
        if (MenuMusicManager.Instance != null)
        {
            MenuMusicManager.Instance
                .DestroyMusicManager();
        }

        // carrega fase
        SceneManager.LoadScene(levelIndex);
    }
}