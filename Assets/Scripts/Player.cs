using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IItemHolder
{
    // Layer dos objetos interagíveis 
    public LayerMask interactLayer;

    // ===== CONFIGURAÇÕES DO JOGADOR =====
    public float speed = 9f;
    public Transform holdPoint;
    public float interactRange = 2f;
    public KeyCode interactKey = KeyCode.E;

    // Item que o jogador está segurando
    private Item heldItem;

    void Update()
    {
        // bloqueia tudo se o jogo não estiver rodando
        if (!GameManager.Instance.IsGamePlaying()) return;
        
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

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        direction = direction.normalized;

        // move o personagem
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        //  ROTACIONA NA DIREÇÃO DO MOVIMENTO =====
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }

    // ===== SISTEMA DE INTERAÇÃO =====
    void TryInteract()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, interactLayer);

        IInteractable closestInteractable = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            // IGNORA ITEM QUE O PLAYER ESTÁ SEGURANDO
            if (heldItem != null && hit.transform == heldItem.transform)
                continue;

            //  Interface → usar GetComponent 
            IInteractable interactable = hit.GetComponent<IInteractable>();

            if (interactable != null)
            {
                // posição padrão
                Vector3 targetPosition = hit.transform.position;

                //  aqui usar TryGetComponent  (seguro)
                if (hit.TryGetComponent(out IItemHolder holder))
                {
                    Transform hold = holder.GetHoldPoint();

                    if (hold != null)
                    {
                        targetPosition = hold.position;
                    }
                }

                float distance = Vector3.Distance(transform.position, targetPosition);

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

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // move para a mão do player
        item.transform.position = holdPoint.position;

        // vira filho do holdPoint
        item.transform.SetParent(holdPoint);

        // define que o player é o holder
        item.SetHolder(this);
    }

    // ===== IMPLEMENTAÇÃO DO IItemHolder =====

    public Transform GetHoldPoint()
    {
        return holdPoint;
    }

    public void SetItem(Item item)
    {
        heldItem = item;
    }

    public Item GetItem()
    {
        return heldItem;
    }

    public bool HasItem()
    {
        return heldItem != null;
    }

    public void ClearItem()
    {
        heldItem = null;
    }

    public Item GetHeldItem()
    {
        return heldItem;
    }

    public void SetHeldItem(Item item)
    {
        heldItem = item;
    }
}