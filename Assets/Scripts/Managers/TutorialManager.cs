using System.Collections;
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

    public RectTransform highlightBorder;

    // seta do tutorial
    public RectTransform tutorialArrow;

    // câmera principal da fase
    private Camera mainCamera;

    // passo atual
    private int currentStepIndex = 0;

    // tutorial ativo?
    private bool tutorialActive = false;

    // controla qual som será tocado
    private int currentSoundIndex = 0;

    // velocidade da animação
    [SerializeField]
    private float arrowBounceSpeed = 8f;

    // distância do movimento
    [SerializeField]
    private float arrowBounceHeight = 30f;

    // posição base da seta na UI
    private Vector2 arrowBaseAnchoredPosition;

    // direção da animação da seta
    private Vector2 arrowBounceDirection;

    [Header("Typing Effect")]

    // Tempo entre cada caractere
    [SerializeField] private float typingSpeed = 0.03f;

    // Indica se o texto está sendo digitado
    private bool isTyping = false;

    // Coroutine da digitação atual
    private Coroutine typingCoroutine;

    // Guarda a mensagem completa atual
    private string currentFullMessage;

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Se ainda está digitando,
            // completa o texto imediatamente
            if (isTyping)
            {
                CompleteCurrentText();
                return;
            }

            NextStep();
        }

        AnimateArrow();
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

        // Guarda a mensagem completa atual
        currentFullMessage = step.message;

        // Segurança: para coroutine anterior
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Inicia digitação
        typingCoroutine = StartCoroutine(TypeText(step.message));

        // Atualiza highlight
        UpdateHighlight(step);

        // Atualiza seta
        UpdateArrow(step);
    }

    IEnumerator TypeText(string message)
    {
        isTyping = true;

        dialogueText.text = "";

        foreach (char letter in message)
        {
            dialogueText.text += letter;

            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
    }

    void CompleteCurrentText()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueText.text = currentFullMessage;

        isTyping = false;
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
            highlightBorder.gameObject.SetActive(false);
            return;
        }

        highlightBorder.gameObject.SetActive(true);

        // alvo UI
        if (step.uiTarget != null)
        {
            highlightBorder.position =
                step.uiTarget.position;
        }
        // alvo mundo 3D
        else if (step.worldTarget != null)
        {
            Vector3 screenPosition =
                mainCamera.WorldToScreenPoint(
                    step.worldTarget.position
                );

            highlightBorder.position = screenPosition;
        }

        highlightBorder.sizeDelta =
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

    void UpdateArrow(TutorialStep step)
    {
        // este passo não usa seta
        if (!step.showArrow)
        {
            tutorialArrow.gameObject.SetActive(false);
            return;
        }

        tutorialArrow.gameObject.SetActive(true);

        Vector2 borderPosition =
            highlightBorder.position;

        Vector2 borderSize =
            highlightBorder.sizeDelta;

        Vector2 arrowPosition = borderPosition;

        float margin = 80f;

        switch (step.arrowPosition)
        {
            case ArrowPosition.Top:

                arrowPosition +=
                    new Vector2(
                        0,
                        borderSize.y * 0.5f + margin
                    );

                tutorialArrow.rotation =
                    Quaternion.Euler(0, 0, 0);

                arrowBounceDirection =
                    Vector2.up;

                break;

            case ArrowPosition.Left:

                arrowPosition +=
                    new Vector2(
                        -(borderSize.x * 0.5f + margin),
                        0
                    );

                tutorialArrow.rotation =
                    Quaternion.Euler(0, 0, 90);

                arrowBounceDirection =
                    Vector2.right;

                break;

            case ArrowPosition.Right:

                arrowPosition +=
                    new Vector2(
                        borderSize.x * 0.5f + margin,
                        0
                    );

                tutorialArrow.rotation =
                    Quaternion.Euler(0, 0, -90);

                arrowBounceDirection =
                    Vector2.left;

                break;
        }

        arrowPosition += step.arrowOffset;

        tutorialArrow.position = arrowPosition;

        // guarda posição base da animação
        arrowBaseAnchoredPosition =
            tutorialArrow.anchoredPosition;
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

    // animação da seta
    void AnimateArrow()
    {
        if (!tutorialArrow.gameObject.activeSelf)
            return;

        float offset =
            Mathf.Sin(Time.unscaledTime * arrowBounceSpeed)
            * arrowBounceHeight;

        tutorialArrow.anchoredPosition =
            arrowBaseAnchoredPosition +
            arrowBounceDirection * offset;
    }
}