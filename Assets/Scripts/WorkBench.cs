using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkBench : MonoBehaviour
{
    // Ponto onde os itens vão ficar visualmente na bancada
    public Transform itemPoint;

    // Prefab do item que será criado quando combinar dois itens
    public GameObject resultPrefab;

    // Guarda o primeiro item colocado
    private Item firstItem;

    // Guarda o segundo item colocado
    private Item secondItem;

    // Essa função será chamada pelo Player quando ele interagir com a bancada
    public void Interact(Item item)
    {
        // Se ainda não tem nenhum item na bancada
        if (firstItem == null)
        {
            firstItem = item; // Guarda como primeiro item
            PlaceItem(item);  // Coloca ele na posição da bancada
        }
        // Se já tem um item mas ainda não tem o segundo
        else if (secondItem == null)
        {
            secondItem = item; // Guarda como segundo item
            PlaceItem(item);   // Coloca ele na posição da bancada

            // Agora que tem dois itens, combina eles
            CombineItems();
        }
    }

    // Coloca o item na posição correta da bancada
    void PlaceItem(Item item)
    {
        // Pega o Rigidbody do item
        Rigidbody rb = item.GetComponent<Rigidbody>();

        // Desativa física para ele não cair
        rb.isKinematic = true;
        rb.useGravity = false;

        // Move o item para o ponto da bancada
        item.transform.position = itemPoint.position;

        // Faz o item virar filho do ponto da bancada
        item.transform.parent = itemPoint;
    }

    // Combina os dois itens e cria o novo
    void CombineItems()
    {
        // Destroi os dois itens antigos
        Destroy(firstItem.gameObject);
        Destroy(secondItem.gameObject);

        // Limpa as variáveis
        firstItem = null;
        secondItem = null;

        // Cria o novo item no lugar
        Instantiate(resultPrefab, itemPoint.position, Quaternion.identity);
    }
}
