using System.Collections.Generic;
using UnityEngine;

public class WorkBench : MonoBehaviour, IInteractable
{
    // ponto onde o item aparece
    public Transform holdPoint;

    // primeiro item colocado
    private Item firstItem;

    // ===== SISTEMA DE RECEITAS =====
    [System.Serializable]
    public class Recipe
    {
        public ItemType itemA;
        public ItemType itemB;
        public GameObject resultPrefab;
    }

    public List<Recipe> recipes = new List<Recipe>();


    // ===== INTERAÇÃO =====
    public void Interact(Player player)
    {
        Item heldItem = player.GetHeldItem();

        // jogador não tem item
        if (heldItem == null)
        {
            TakeResult(player);
            return;
        }

        // jogador colocou primeiro item
        if (firstItem == null)
        {
            PlaceFirstItem(player, heldItem);
            return;
        }

        // jogador colocou segundo item
        TryCombine(player, heldItem);
    }


    // ===== COLOCA PRIMEIRO ITEM =====
    void PlaceFirstItem(Player player, Item item)
    {
        firstItem = item;

        player.SetItem(null);

        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        item.transform.SetParent(holdPoint);
        item.transform.localPosition = Vector3.zero;
    }


    // ===== TENTA COMBINAR =====
    void TryCombine(Player player, Item secondItem)
    {
        foreach (Recipe recipe in recipes)
        {
            if (
                (recipe.itemA == firstItem.itemType && recipe.itemB == secondItem.itemType) ||
                (recipe.itemA == secondItem.itemType && recipe.itemB == firstItem.itemType)
            )
            {
                // remove itens antigos
                Destroy(firstItem.gameObject);
                Destroy(secondItem.gameObject);

                player.SetItem(null);

                // cria resultado
                GameObject result = Instantiate(
                    recipe.resultPrefab,
                    holdPoint.position,
                    Quaternion.identity
                );

                result.transform.SetParent(holdPoint);

                // limpa primeiro item
                firstItem = null;

                return;
            }
        }

        Debug.Log("Receita não encontrada!");
    }


    // ===== PEGAR RESULTADO =====
    void TakeResult(Player player)
    {
        if (holdPoint.childCount == 0) return;

        Item item = holdPoint.GetChild(0).GetComponent<Item>();

        if (item == null) return;

        item.transform.SetParent(null);

        player.PickupItem(item);
    }
}