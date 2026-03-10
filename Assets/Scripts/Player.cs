using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IItemHolder
{
    // ===== CONFIGURAÇÕES DO JOGADOR =====
    public float speed = 6f;
    public Transform holdPoint;
    public float interactRange = 2f;
    public KeyCode interactKey = KeyCode.E;

    // Item que o jogador está segurando
    private Item heldItem;

    void Update()
    {
        Move();

        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    // ===== MOVIMENTO =====
    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        direction = Quaternion.Euler(0, 45, 0) * direction;

        direction = direction.normalized;

        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    // ===== SISTEMA DE INTERAÇÃO =====
    void TryInteract()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange);

        IInteractable closestInteractable = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            // IGNORA ITEM QUE O PLAYER ESTÁ SEGURANDO
            if (heldItem != null && hit.transform == heldItem.transform)
                continue;

            IInteractable interactable = hit.GetComponent<IInteractable>();

            if (interactable != null)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }
    }

    if (closestInteractable != null)
    {
        closestInteractable.Interact(this);
    }
}

    // ===== PEGAR ITEM =====
    public void PickupItem(Item item)
    {
        heldItem = item;

        Rigidbody rb = item.GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = true;

        // move para a mão do player
        item.transform.position = holdPoint.position;

        // vira filho do holdPoint
        item.transform.SetParent(holdPoint);

        // define que o player é o holder
        item.SetHolder(this);
    }

    // ===== IMPLEMENTAÇÃO DO IItemHolder =====

    // retorna o ponto onde o item ficará
    public Transform GetHoldPoint()
    {
        return holdPoint;
    }

    // define item na mão
    public void SetItem(Item item)
    {
        heldItem = item;
    }

    // retorna item atual
    public Item GetItem()
    {
        return heldItem;
    }

    // verifica se possui item
    public bool HasItem()
    {
        return heldItem != null;
    }

    // limpa item da mão
    public void ClearItem()
    {
        heldItem = null;
    }

    // compatibilidade código antigo
    public Item GetHeldItem()
    {
        return heldItem;
    }

    public void SetHeldItem(Item item)
    {
        heldItem = item;
    }
}