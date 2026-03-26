using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gizmo/RecipeDatabase")]
public class RecipeDatabase : ScriptableObject
{
    public List<Recipe> recipes; // lista de todas as receitas

    public Recipe GetRecipe(ItemType a, ItemType b)
    {
        foreach (Recipe recipe in recipes)
        {
            // aceita ordem invertida
            if ((recipe.itemA == a && recipe.itemB == b) || (recipe.itemA == b && recipe.itemB == a))
            {
                return recipe; // encontrou
            }
        }

        return null; // não encontrou
    }
}