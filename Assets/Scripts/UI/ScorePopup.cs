using TMPro;
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    [Header("Referências")]
    public TextMeshProUGUI popupText;

    [Header("Movimento")]
    public float moveSpeed = 80f;

    [Header("Tempo")]
    public float lifetime = 0.8f;

    private float timer;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        // pega CanvasGroup
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        // aumenta timer
        timer += Time.deltaTime;

        // move para cima
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // fade out
        float alpha = 1f - (timer / lifetime);

        canvasGroup.alpha = alpha;

        // destrói ao terminar
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    // configura popup
    public void Setup(int amount, Color textColor)
    {
        // define texto
        if (amount >= 0)
        {
            popupText.text = "+" + amount.ToString();
        }
        else
        {
            popupText.text = amount.ToString();
        }

        // define cor
        popupText.color = textColor;
    }
}