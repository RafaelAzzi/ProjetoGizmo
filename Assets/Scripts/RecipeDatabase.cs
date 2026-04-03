using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gizmo/RecipeDatabase")]
public class RecipeDatabase : ScriptableObject
{
    public List<Recipe> recipes; // lista de todas as receitas

    public Recipe GetRecipe(ItemType a, ItemType b)
    {
        // debug opcional (ajuda MUITO quando tiver muitas receitas)
        Debug.Log($"Tentando combinar: {a} + {b}");

        foreach (Recipe recipe in recipes)
        {
            // verifica combinação (ordem não importa)
            if ((recipe.itemA == a && recipe.itemB == b) ||
                (recipe.itemA == b && recipe.itemB == a))
            {
                Debug.Log("Receita encontrada!");
                return recipe;
            }
        }

        Debug.Log("Nenhuma receita encontrada.");
        return null;
    }
}