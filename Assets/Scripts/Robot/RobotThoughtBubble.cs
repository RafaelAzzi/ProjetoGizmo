using UnityEngine;
using UnityEngine.UI;

public class RobotThoughtBubble : MonoBehaviour
{
    [Header("UI")]

    // fundo colorido da borda
    public Image bubbleBorder;

    // ícone do item
    public Image itemIcon;

    // aplica visual do pedido
    public void Setup(Sprite iconSprite, Color borderColor)
    {
        // aplica ícone
        itemIcon.sprite = iconSprite;

        // aplica cor da borda
        bubbleBorder.color = borderColor;
    }
}