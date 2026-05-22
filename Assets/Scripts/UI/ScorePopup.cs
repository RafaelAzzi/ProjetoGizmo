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

    // configura valor do popup
    public void Setup(int amount)
    {
        popupText.text = "+" + amount.ToString();
    }
}