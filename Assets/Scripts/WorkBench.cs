using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkBench : MonoBehaviour
{
    // Ponto onde os itens vão aparecer na bancada
    public Transform itemPoint; // posição onde itens ficam 

    // Guarda o primeiro item colocado
    private Item firstItem;

    // Guarda o segundo item colocado
    private Item secondItem; 


    // ===== SISTEMA DE RECEITAS =====

    [System.Serializable] // permite aparecer no inspector
    public class Recipe
    {
        public ItemType itemA; // primeiro item da receita
        public ItemType itemB; // segundo item da receita
        public GameObject resultPrefab; // prefab do resultado
    }

    // Lista de todas as receitas possíveis
    public List<Recipe> recipes = new List<Recipe>();


    // ===== INTERAÇÃO COM PLAYER =====
    public void Interact(Item item)
    {
        // Se ainda não tem primeiro item
        if (firstItem == null)
        {
            firstItem = item; // guarda o item
            PlaceItem(item); // posiciona na bancada
        }

        // Se já tem primeiro mas não tem segundo
        else if (secondItem == null)
        {
            secondItem = item; // guarda o segundo
            PlaceItem(item); // posiciona na bancada

            CombineItems(); // tenta combinar
        }
    }


    // ===== COLOCA ITEM NA BANCADA =====
    void PlaceItem(Item item)
    {
        Rigidbody rb = item.GetComponent<Rigidbody>(); // pega o rigidbody

        rb.isKinematic = true; // desativa física
        rb.useGravity = false; // desativa gravidade

        item.transform.position = itemPoint.position; // move para bancada

        item.transform.parent = itemPoint; // vira filho da bancada
    }


    // ===== COMBINAR ITENS =====
    void CombineItems()
    {
        // Verifica se os itens são da mesma fase
        if (firstItem.phase != secondItem.phase)
        {
            Debug.Log("Itens de fases diferentes não podem ser combinados!");
            ResetItems();
            return;
        }

        // Fase 3 não pode combinar
        if (firstItem.phase == ItemPhase.Fase3_Lendario)
        {
            Debug.Log("Itens lendários não podem ser combinados!");
            ResetItems();
            return;
        }

        // Procura receita válida
        foreach (Recipe recipe in recipes)
        {
            // Verifica receita em qualquer ordem
            if (
                (recipe.itemA == firstItem.itemType && recipe.itemB == secondItem.itemType) ||
                (recipe.itemA == secondItem.itemType && recipe.itemB == firstItem.itemType)
            )
            {
                // Destroi os itens antigos
                Destroy(firstItem.gameObject);
                Destroy(secondItem.gameObject);

                // Cria resultado correto
                Instantiate(recipe.resultPrefab, itemPoint.position, Quaternion.identity);

                ResetItems(); // limpa variáveis
                return;
            }
        }

        // Se não encontrou receita
        Debug.Log("Receita não encontrada!");
        ResetItems();
    }


    // ===== RESETAR ITENS =====
    void ResetItems()
    {
        firstItem = null; // limpa primeiro item
        secondItem = null; // limpa segundo item
    }
}