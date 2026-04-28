using System.Collections.Generic;
using UnityEngine;

// representa o robô cliente
public class RobotCustomer : MonoBehaviour, IInteractable
{
    public float moveSpeed = 3f;

    // ponto onde ele vai parar (slot)
    private Transform targetPoint;

    // ponto de saída
    private Transform exitPoint;

    // referência ao OrderManager
    private OrderManager orderManager;

    // ===== NOVO: PONTO REAL DE INTERAÇÃO =====
    public Transform interactPoint;

    // distância máxima para interagir
    public float interactDistance = 4f;

    // estado do robô
    private bool isWaiting = false;
    private bool isLeaving = false;

    // controle de pedido
    private bool hasOrder = false;
    private Order myOrder;

    // distância para considerar que chegou no destino
    public float stopDistance = 0.1f;

    // ===== CONFIGURAÇÃO INICIAL =====
    public void Setup(Transform target, Transform exit, OrderManager manager)
    {
        targetPoint = target;
        exitPoint = exit;
        orderManager = manager;
    }

    void Update()
    {
        // ===== IR ATÉ O SLOT =====
        if (!isWaiting && !isLeaving)
        {
            MoveToTarget(targetPoint.position);

            if (Vector3.Distance(transform.position, targetPoint.position) < stopDistance)
            {
                isWaiting = true;

                // ===== NOVO: GERA PEDIDO AUTOMATICAMENTE =====
                if (!hasOrder)
                {
                    myOrder = orderManager.GenerateNewOrder();

                    if (myOrder != null)
                    {
                        hasOrder = true;
                        Debug.Log("Robô gerou pedido automático: " + myOrder.requestedItems.Count + " itens");
                    }
                }
            }
        }

        // ===== NOVO: VERIFICA SE O PEDIDO EXPIROU =====
        if (isWaiting && hasOrder && !isLeaving)
        {
            if (!orderManager.activeOrders.Contains(myOrder))
            {
                Debug.Log("Pedido do robô expirou, indo embora...");

                isWaiting = false;
                isLeaving = true;
            }
        }

        // ===== SAIR DO MAPA =====
        if (isLeaving)
        {
            MoveToTarget(exitPoint.position);

            if (Vector3.Distance(transform.position, exitPoint.position) < stopDistance)
            {
                Destroy(gameObject);
            }
        }
    }

    // ===== MOVIMENTO =====
    void MoveToTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;

        // rotaciona na direção do movimento
        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }

    // ===== INTERAÇÃO COM PLAYER =====
    public void Interact(Player player)
    {
        Debug.Log("Interagiu com robô");
        Debug.Log("isWaiting: " + isWaiting);
        Debug.Log("hasOrder: " + hasOrder);
        Debug.Log("isLeaving: " + isLeaving);
        
        // ===== VALIDA DISTÂNCIA REAL =====
        if (interactPoint == null)
        {
            Debug.LogError("InteractPoint NÃO definido no robô!");
            return;
        }

        float distance = Vector3.Distance(player.transform.position, interactPoint.position);

        // se estiver longe, não interage
        if (distance > interactDistance)
        {
            return;
        }

        // só permite interação quando estiver parado esperando
        if (!isWaiting) return;


        // ===== PASSO 2: ENTREGAR ITEM =====
        // ===== ENTREGA =====
        Item heldItem = player.GetHeldItem();
        if (heldItem == null) return;

        // DEBUG
        Debug.Log("Item na mão: " + heldItem.name);
        Debug.Log("Tipo do item: " + heldItem.GetType());

        // verifica se é prato
        PlateItem plate = heldItem as PlateItem;

        if (plate != null)
        {
            return; // não é prato → não entrega
        }

        bool success = TryDeliverPlate(plate);

        if (success)
        {
            Destroy(plate.gameObject);

            isWaiting = false;
            isLeaving = true;
        }

        
    }

    bool TryDeliverPlate(PlateItem plate)
    {
        // ===== VALIDAÇÕES =====
        if (plate == null)
        {
            Debug.LogError("Plate está NULL!");
            return false;
        }

        if (myOrder == null || myOrder.requestedItems == null)
        {
            Debug.LogError("Pedido inválido!");
            return false;
        }

        if (plate.GetItems() == null)
        {
            Debug.LogError("Plate não tem itens!");
            return false;
        }

        // ===== COPIA LISTAS =====
        List<ItemType> orderItems = new List<ItemType>(myOrder.requestedItems);
        List<ItemType> plateItems = new List<ItemType>(plate.GetItems());

        // ===== DEBUG (UMA VEZ SÓ) =====
        Debug.Log("----- VALIDANDO PRATO -----");

        Debug.Log("Pedido:");
        foreach (var orderItem in orderItems)
        {
            Debug.Log(orderItem);
        }

        Debug.Log("Prato:");
        foreach (var plateItem in plateItems)
        {
            Debug.Log(plateItem);
        }

        // ===== COMPARAÇÃO =====
        foreach (var plateItem in plateItems)
        {
            if (orderItems.Contains(plateItem))
            {
                orderItems.Remove(plateItem);
            }
            else
            {
                Debug.Log("Item do prato NÃO corresponde ao pedido: " + plateItem);
                return false;
            }
        }

        // ===== VERIFICA SE FALTOU ITEM =====
        if (orderItems.Count > 0)
        {
            Debug.Log("Prato incompleto! Faltando itens:");
            foreach (var remainingItem in orderItems)
            {
                Debug.Log(remainingItem);
            }
            return false;
        }

        // ===== SUCESSO =====
        Debug.Log("Pedido COMPLETO com prato!");

        orderManager.activeOrders.Remove(myOrder);

        return true;
    }
}