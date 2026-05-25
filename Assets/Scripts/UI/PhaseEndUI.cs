using System.Collections;
using UnityEngine;
using TMPro;

public class PhaseEndUI : MonoBehaviour
{
    [Header("Referências")]
    public RectTransform timeUpText;
    public CanvasGroup canvasGroup;

    [Header("Configuração")]
    public float popDuration = 0.3f;
    public float stayDuration = 2.3f;
    public float fadeOutDuration = 0.4f;

    [Header("Escala")]
    public Vector3 startScale = new Vector3(0.6f, 0.6f, 0.6f);
    public Vector3 overshootScale = new Vector3(1.05f, 1.05f, 1.05f);
    public Vector3 finalScale = Vector3.one;

    [Header("Áudio")]
    public AudioSource audioSource;
    public AudioClip timeUpSFX;

    // referência da ResultUI
    public ResultUI resultUI;

    void Start()
    {
        // garante escala inicial
        timeUpText.localScale = startScale;
    }

    // ===== MOSTRAR TELA =====
    public void Show()
    {
        canvasGroup.alpha = 1f;

        // ativa canvas
        gameObject.SetActive(true);

        // toca som
        if (audioSource != null && timeUpSFX != null)
        {
            audioSource.PlayOneShot(timeUpSFX);
        }

        // inicia animação
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // tempo da animação
        float timer = 0f;

        // ===== ANIMAÇÃO DE ENTRADA =====
        while (timer < popDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / popDuration;

            // cresce até overshoot
            timeUpText.localScale = Vector3.Lerp(startScale, overshootScale, t);

            yield return null;
        }

        // pequena estabilização
        timeUpText.localScale = finalScale;

        // ===== ESPERA NA TELA =====
        yield return new WaitForSecondsRealtime(stayDuration);

        // ===== FADE OUT =====
        float fadeTimer = 0f;

        while (fadeTimer < fadeOutDuration)
        {
            fadeTimer += Time.unscaledDeltaTime;

            float t = fadeTimer / fadeOutDuration;

            // fade da tela inteira
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

canvasGroup.alpha = 0f;

        // ===== ABRE RESULTADO =====
        if (resultUI != null)
        {
            resultUI.ShowResults();
        }

        // desativa tela
        canvasGroup.alpha = 0f;
    }
}