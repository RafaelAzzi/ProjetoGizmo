using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    // ===== CARREGAR PRÓXIMA FASE =====
    public void LoadNextLevel()
    {
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
        // recarrega a cena atual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ===== IR PARA MENU =====
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    // ===== CARREGAR FASE ESPECÍFICA =====
    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    void Update()
    {
        // pressiona N para próxima fase
        if (Input.GetKeyDown(KeyCode.N))
        {
            LoadNextLevel();
        }
    }
}