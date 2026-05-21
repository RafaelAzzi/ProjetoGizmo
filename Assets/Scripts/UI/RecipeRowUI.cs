using UnityEngine;
using UnityEngine.UI;

public class RecipeRowUI : MonoBehaviour
{
    [Header("Ícones")]

    // ícone do ingrediente A
    public Image ingredientAIcon;

    // ícone do ingrediente B
    public Image ingredientBIcon;

    // configura os ícones da linha
    public void Setup(Sprite iconA, Sprite iconB)
    {
        // define ingrediente A
        ingredientAIcon.sprite = iconA;

        // define ingrediente B
        ingredientBIcon.sprite = iconB;
    }
}