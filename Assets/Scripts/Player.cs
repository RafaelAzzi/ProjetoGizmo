using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // ===== CONFIGURAÇÕES DO JOGADOR =====
    public float speed = 10f;
    public Transform holdPoint; // ponto onde o item ficará quando o jogador estiver segurando
    public float interactRange = 2f; // distância máxima para interagir com objetos
    public KeyCode interactKey = KeyCode.E;

    // Guarda o item que o jogador está segurando no momento
    private Item heldItem;

    void Update()
    {
        // Movimento do jogador
        Move();

        // Se apertar tecla de interação
        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    // ===== MOVIMENTO DO JOGADOR =====
    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Cria vetor de direção
        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        // Ajusta direção para câmera isométrica
        direction = Quaternion.Euler(0, 45, 0) * direction;

        // Normaliza para evitar velocidade maior na diagonal
        direction = direction.normalized;

        // Move o player
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    // ===== SISTEMA DE INTERAÇÃO =====
    void TryInteract()
    {
        // Detecta objetos próximos
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange);

        // ===============================
        // SE O PLAYER ESTÁ SEGURANDO ITEM
        // ===============================
        if (heldItem != null)
        {
            foreach (Collider hit in hits)
            {
                // Ignora o próprio player
                if (hit.gameObject == gameObject) continue;

                IInteractable interactable = hit.GetComponent<IInteractable>();

                // Se encontrou algo interagível
                if (interactable != null)
                {
                    interactable.Interact(this);
                    return;
                }
            }

            // Se não encontrou nada para interagir
            // solta o item no chão
            DropItem();
            return;
        }

        // ==================================
        // SE O PLAYER NÃO ESTÁ SEGURANDO ITEM
        // ==================================
        foreach (Collider hit in hits)
        {
            // Ignora o próprio player
            if (hit.gameObject == gameObject) continue;

            ItemPickup pickup = hit.GetComponent<ItemPickup>();

            if (pickup != null)
            {
                pickup.Interact(this);
                return;
            }
        }
    }

    // ===== PEGAR ITEM =====
    public void PickupItem(Item item)
    {
        heldItem = item;

        Rigidbody rb = item.GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = true;

        item.transform.position = holdPoint.position;
        item.transform.parent = holdPoint;
    }

    // ===== SOLTAR ITEM NO CHÃO =====
    public void DropItem()
    {
        if (heldItem == null) return;

        Rigidbody rb = heldItem.GetComponent<Rigidbody>();

        heldItem.transform.parent = null;

        // Solta o item um pouco à frente do jogador
        heldItem.transform.position = transform.position + transform.forward * 1f;

        rb.useGravity = true;
        rb.isKinematic = false;

        heldItem = null;
    }

    // ===== RETORNA ITEM NA MÃO =====
    public Item GetHeldItem()
    {
        return heldItem;
    }

    // ===== DEFINE ITEM NA MÃO =====
    public void SetHeldItem(Item item)
    {
        heldItem = item;
    }

    // ===== LIMPA ITEM DA MÃO =====
    public void ClearHeldItem()
    {
        heldItem = null;
    }
}