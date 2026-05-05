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

    // direção do raycast
    public float interactWidth = 0.5f; // largura do "cone" de interação

    // Item que o jogador está segurando
    private Item heldItem;

    // HoldPoint atualmente destacado
    private HoldPoint currentHighlight;

    void Update()
    {
        // bloqueia tudo se o jogo não estiver rodando
        if (!GameManager.Instance.IsGamePlaying()) return;
        
        Move();

        HandleHighlight();

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
        // cria um "raio largo" na frente do player
        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position,
            interactWidth, // largura
            transform.forward, // direção
            interactRange, // distância
            interactLayer
        );

        IInteractable closestHoldPoint = null;
        float closestHoldPointDistance = Mathf.Infinity;

        IInteractable closestFallback = null;
        float closestFallbackDistance = Mathf.Infinity;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject == gameObject) continue;

            // IGNORA ITEM QUE O PLAYER ESTÁ SEGURANDO
            if (heldItem != null && hit.transform == heldItem.transform)
                continue;

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable == null) continue;

            Vector3 targetPosition = hit.transform.position;

            if (hit.collider.TryGetComponent(out IItemHolder holder))
            {
                Transform hold = holder.GetHoldPoint();

                if (hold != null)
                    targetPosition = hold.position;
            }

            float distance = Vector3.Distance(transform.position, targetPosition);

            // PRIORIDADE: HOLDPOINT
            if (hit.collider.GetComponent<HoldPoint>() != null)
            {
                if (distance < closestHoldPointDistance)
                {
                    closestHoldPointDistance = distance;
                    closestHoldPoint = interactable;
                }
            }
            else
            {
                if (distance < closestFallbackDistance)
                {
                    closestFallbackDistance = distance;
                    closestFallback = interactable;
                }
            }
        }

        // decisão final
        if (closestHoldPoint != null)
        {
            closestHoldPoint.Interact(this);
        }
        else if (closestFallback != null)
        {
            closestFallback.Interact(this);
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

    void HandleHighlight()
    {
        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position,
            interactWidth,
            transform.forward,
            interactRange,
            interactLayer
        );

        HoldPoint closestHoldPoint = null;
        float closestDistance = Mathf.Infinity;

        foreach (RaycastHit hit in hits)
        {
            HoldPoint hp = hit.collider.GetComponent<HoldPoint>();

            if (hp == null) continue;

            float distance = Vector3.Distance(transform.position, hp.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestHoldPoint = hp;
            }
        }

        // atualização do highlight (igual antes)
        if (currentHighlight != closestHoldPoint)
        {
            if (currentHighlight != null)
                currentHighlight.HideHighlight();

            if (closestHoldPoint != null)
                closestHoldPoint.ShowHighlight();

            currentHighlight = closestHoldPoint;
        }
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