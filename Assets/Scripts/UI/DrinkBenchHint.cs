using System.Collections;
using UnityEngine;

public class DrinkBenchHint : MonoBehaviour
{
    // Singleton simples para acesso global
    public static DrinkBenchHint Instance;

    [Header("Visual")]
    public SpriteRenderer arrowRenderer;

    [Header("Tempo")]
    public float showDuration = 3f;

    [Header("Bounce")]
    public float bounceHeight = 0.15f;
    public float bounceSpeed = 4f;

    private Coroutine hintCoroutine;

    private Vector3 startPosition;

    void Awake()
    {
        // Garante apenas uma instância
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        startPosition = transform.position;

        if (arrowRenderer != null)
        {
            arrowRenderer.enabled = false;
        }
    }

    void Update()
    {
        // animação simples de quicar
        float offset =
            Mathf.Sin(Time.time * bounceSpeed)
            * bounceHeight;

        transform.position =
            startPosition +
            Vector3.up * offset;
    }

    // Mostra a seta
    public void ShowHint()
    {
        Debug.Log("ShowHint chamado");

        if (!CanShowHint())
            return;

        if (hintCoroutine != null)
        {
            StopCoroutine(hintCoroutine);
        }

        hintCoroutine =
            StartCoroutine(ShowHintRoutine());
    }

    // Mostra a seta
    bool CanShowHint()
    {
        if (GameManager.Instance == null)
        {
            Debug.Log("GameManager NULL");
            return false;
        }

        Debug.Log("LevelID: " + GameManager.Instance.levelID);
        Debug.Log("Tempo: " + GameManager.Instance.GetTimeRemaining());

        // somente fase 1
        if (GameManager.Instance.levelID != 1)
        {
            Debug.Log("Não está na fase 1");
            return false;
        }

        // somente primeiros 2 minutos
        if (GameManager.Instance.GetTimeRemaining() <= 60f)
        {
            Debug.Log("Passou dos 2 minutos");
            return false;
        }

        return true;
    }

    IEnumerator ShowHintRoutine()
    {
        arrowRenderer.enabled = true;

        yield return new WaitForSeconds(showDuration);

        arrowRenderer.enabled = false;
    }
}