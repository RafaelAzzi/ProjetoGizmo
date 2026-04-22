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
            }
        }

        // ===== NOVO: VERIFICA SE O PEDIDO EXPIRou =====
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

        // ===== PASSO 1: PEGAR PEDIDO =====
        if (!hasOrder)
        {
            myOrder = orderManager.GenerateNewOrder();

            if (myOrder != null)
            {
                hasOrder = true;
                string items = "";

                foreach (var item in myOrder.requestedItems)
                {
                    items += item.ToString() + " ";
                }

                Debug.Log("Robô fez pedido: " + items);
            }

            return; // importante: não continua para entrega
        }

        // ===== PASSO 2: ENTREGAR ITEM =====
        Item heldItem = player.GetHeldItem();

        // player não tem item
        if (heldItem == null) return;

        bool success = orderManager.TryCompleteOrder(heldItem);

        if (success)
        {
            // remove item corretamente do sistema
            heldItem.SetHolder(null);

            // robô começa a sair
            isWaiting = false;
            isLeaving = true;
        }
    }
}