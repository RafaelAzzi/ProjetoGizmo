using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10f; 
    public Transform holdPoint; // Onde o obejto vai ficar quando estiver sendo segurado
    public float pickupRange = 2f; // Distancia para pegar o objeto
    public KeyCode pickupKey = KeyCode.E; 
    private Item heldItem; // guardamos o script Item ao invés de GameObject

    void Update()
    {
        // ===== MOVIMENTO ===== 
        float horizontal = Input.GetAxis("Horizontal"); // Pega os Input padrao da Unity, que vai de -1 a 1
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical); // Cria vetor de movimento, neste caso X e Z

        direction = Quaternion.Euler(0, 45, 0) * direction; // Faz o personagem andar de acordo com o �ngulo da c�mera
        direction = direction.normalized;

        transform.Translate(direction * speed * Time.deltaTime, Space.World); 
        // Movimento do personagem: Direcao calculada, velocidade, movimento independente do frame


        // ===== SISTEMA DE INTERACAO =====
        if (Input.GetKeyDown(pickupKey)) // Sistema de pegar obejto
        {
            // Se não estiver segurando nada, tenta pegar
            if (heldItem == null)
             {
                TryPickup();
             }
            else
             {
               // Se estiver segurando algo, tenta interagir com bancada
               TryInteractWithStation();
             }
        }
        
    }


    // ===== TENTAR PEGAR UM ITEM =====
    void TryPickup()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange); 
        // Detecta todos os colliders próximos dentro do raio

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Pickable")) // So pega objetos com a Tag Pickable
            {
                // Tenta pegar o script Item do objeto
                Item item = hit.GetComponent<Item>();

                if (item != null)
                {
                    heldItem = item; // Guarda o item

                    Rigidbody rb = item.GetComponent<Rigidbody>();

                    // Desativa física enquanto estiver segurando
                    rb.useGravity = false;
                    rb.isKinematic = true;

                    // Move para o ponto da mão
                    item.transform.position = holdPoint.position;

                    // Faz virar filho do holdPoint para acompanhar o player
                    item.transform.parent = holdPoint;

                    break;
                }
            }
        }
    }


    // ===== TENTAR INTERAGIR COM A BANCADA =====
    void TryInteractWithStation()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange);

        foreach (Collider hit in hits)
        {
            // Verifica se o objeto tem o script WorkBench
            WorkBench station = hit.GetComponent<WorkBench>();

            if (station != null)
            {
                // Entrega o item para a bancada
                station.Interact(heldItem);

                // Player não está mais segurando nada
                heldItem = null;

                return;
            }
        }

        // Se não encontrou bancada, solta no chão
        DropObject();
    }


    // ===== SOLTA O ITEM NO CHÃO =====
    void DropObject()
    {
        if (heldItem == null) return;

        Rigidbody rb = heldItem.GetComponent<Rigidbody>();

        // Remove o item do jogador
        heldItem.transform.parent = null;

        // Reativa física
        rb.useGravity = true;
        rb.isKinematic = false;

        heldItem = null;
    }
}
