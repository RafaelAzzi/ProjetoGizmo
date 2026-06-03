using UnityEngine;

[System.Serializable]
public class TutorialStep
{
    [TextArea(3, 6)]
    public string message;

    // alvo no mundo 3D
    public Transform worldTarget;

    // alvo na UI
    public RectTransform uiTarget;

    // tamanho do destaque
    public Vector2 highlightSize = new Vector2(200f, 200f);

    [Header("Arrow")]

    // posição da seta
    public ArrowPosition arrowPosition = ArrowPosition.Top;

    // ajuste fino da posição
    public Vector2 arrowOffset = Vector2.zero;

    // mostrar seta neste passo?
    public bool showArrow = true;
}