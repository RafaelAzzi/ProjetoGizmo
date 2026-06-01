using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Save")]
    public int tutorialLevelID = 1;

    [Header("Tutorial")]
    public List<TutorialStep> steps = new List<TutorialStep>();

    [Header("UI")]
    public GameObject tutorialCanvas;

    public TextMeshProUGUI dialogueText;

    public RectTransform highlightPanel;

    // câmera principal da fase
    private Camera mainCamera;

    // passo atual
    private int currentStepIndex = 0;

    // tutorial ativo?
    private bool tutorialActive = false;

    // controla qual som será tocado
    private int currentSoundIndex = 0;

    private bool skipTutorial = false;

    // indica se tutorial foi aberto pelo pause
    private bool openedFromPause = false;

    [Header("Tutorial Audio")]

    // AudioSource usado para tocar os sons do robô
    public AudioSource tutorialAudioSource;

    // lista dos sons do robô
    public List<AudioClip> dialogueSounds = new List<AudioClip>();

    void Awake()
    {
        // segurança
        if (ProgressManager.Instance == null)
        {
            StartTutorial();
            return;
        }

        if (ProgressManager.Instance.IsTutorialCompleted(tutorialLevelID))
        {
            skipTutorial = true;
            return;
        }
        StartTutorial();
    }

    void Start()
    {
        mainCamera = Camera.main;

        // tutorial já concluído
        if (skipTutorial)
        {
            GameManager.Instance.StartGame();

            if (PhaseMusicManager.Instance != null)
            {
                PhaseMusicManager.Instance.PlayPhaseMusic();
            }
        }
    }

    void Update()
    {
        // só permite avançar se tutorial estiver ativo
        if (!tutorialActive)
            return;

        // SPACE avança
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextStep();
        }
    }

    // ===== INICIAR TUTORIAL =====
    public void StartTutorial()
    {
        if (steps.Count == 0)
            return;

        tutorialActive = true;

        Time.timeScale = 0f;

        tutorialCanvas.SetActive(true);

        currentStepIndex = 0;

        ShowStep(currentStepIndex);

        // toca som da primeira fala
        PlayDialogueSound();
    }

    // ===== MOSTRAR PASSO =====
    void ShowStep(int index)
    {
        TutorialStep step = steps[index];

        // atualiza texto
        dialogueText.text = step.message;

        // atualiza highlight
        UpdateHighlight(step);
    }

    // ===== AVANÇAR =====
    void NextStep()
    {
        currentStepIndex++;

        // terminou tutorial?
        if (currentStepIndex >= steps.Count)
        {
            EndTutorial();
            return;
        }

        // toca som apenas quando existe próxima fala
        PlayDialogueSound();

        ShowStep(currentStepIndex);
    }

    // ===== ATUALIZA DESTAQUE =====
    void UpdateHighlight(TutorialStep step)
    {
        // sem alvo
        if (step.worldTarget == null &&
            step.uiTarget == null)
        {
            highlightPanel.gameObject.SetActive(false);
            return;
        }

        highlightPanel.gameObject.SetActive(true);

        // alvo UI
        if (step.uiTarget != null)
        {
            highlightPanel.position =
                step.uiTarget.position;
        }

        // alvo mundo 3D
        else if (step.worldTarget != null)
        {
            Vector3 screenPosition =
                mainCamera.WorldToScreenPoint(
                    step.worldTarget.position
                );

            highlightPanel.position = screenPosition;
        }

        highlightPanel.sizeDelta =
            step.highlightSize;
    }

    // ===== SOM DA FALA =====
    void PlayDialogueSound()
    {
        // segurança
        if (tutorialAudioSource == null)
            return;

        // segurança
        if (dialogueSounds.Count == 0)
            return;

        // pega som atual
        AudioClip clip =
            dialogueSounds[currentSoundIndex];

        // toca som
        tutorialAudioSource.PlayOneShot(clip);

        // próximo índice
        currentSoundIndex++;

        // volta para o início da lista
        if (currentSoundIndex >= dialogueSounds.Count)
        {
            currentSoundIndex = 0;
        }
    }

    // ===== TUTORIAL PELO PAUSE =====
    public void StartTutorialFromPause()
    {
        openedFromPause = true;

        StartTutorial();
    }

    // ===== FINALIZA ====
    void EndTutorial()
    {
        // tutorial aberto pelo pause
        if (openedFromPause)
        {
            openedFromPause = false;

            tutorialActive = false;

            tutorialCanvas.SetActive(false);

            // volta para menu do pause
            if (PauseManager.Instance != null)
            {
                PauseManager.Instance.BackToPauseMenu();
            }

            return;
        }
        
        // salva conclusão do tutorial
        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.SetTutorialCompleted(
                tutorialLevelID
            );
        }

        tutorialActive = false;

        tutorialCanvas.SetActive(false);

        // inicia música da fase
        if (PhaseMusicManager.Instance != null)
        {
            PhaseMusicManager.Instance.PlayPhaseMusic();
        }

        Time.timeScale = 1f;

        // inicia a partida
        GameManager.Instance.StartGame();
    }
}