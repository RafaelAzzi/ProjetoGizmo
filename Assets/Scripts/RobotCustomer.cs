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

    // anchor do bubble
    public Transform bubbleAnchor;

    // prefab do bubble
    public RobotThoughtBubble bubblePrefab;

    // bubble atual
    private RobotThoughtBubble activeBubble;

    // banco visual dos ícones
    public ItemVisualDatabase visualDatabase;

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

                        // cria bubble visual
                        SpawnBubble();
                    }
                }
            }
        }

        // ==== VERIFICA SE O PEDIDO EXPIROU =====
        if (isWaiting && hasOrder && !isLeaving)
        {
            if (!orderManager.activeOrders.Contains(myOrder))
            {
                Debug.Log("Pedido do robô expirou, indo embora...");

                // remove bubble
                DestroyBubble();

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
        if (!GameManager.Instance.IsGamePlaying()) return;
        
        // ===== VALIDA DISTÂNCIA REAL =====
        if (interactPoint == null)
        {
            Debug.LogError("InteractPoint NÃO definido no robô!");
            return;
        }

        float distance = Vector3.Distance(player.transform.position, interactPoint.position);

        if (distance > interactDistance)
        {
            return;
        }

        // só permite interação quando estiver parado esperando
        if (!isWaiting) return;

        // ===== ENTREGA =====
        Item heldItem = player.GetHeldItem();
        if (heldItem == null) return;

        // verifica se é prato
        PlateItem plate = heldItem as PlateItem;

        // se NÃO for prato, não entrega
        if (plate == null)
        {
            return;
        }

        bool success = DeliveryManager.Instance.TryDeliver(
                        myOrder,
                        plate,
                        orderManager
                    );

        if (success)
        {
            if (plate.originalSlot != null)
            {
                PlateBench bench = plate.originalSlot.GetComponentInParent<PlateBench>();

                if (bench != null)
                {
                    bench.RespawnPlate(plate.originalSlot);
                }
            }
            
            Destroy(plate.gameObject);

            // remove bubble
            DestroyBubble();

            isWaiting = false;
            isLeaving = true;
        }
    } 

    // cria bubble visual
    void SpawnBubble()
    {
        // segurança
        if (bubblePrefab == null)
            return;

        // segurança
        if (bubbleAnchor == null)
            return;

        // segurança
        if (myOrder == null)
            return;

        // segurança
        if (myOrder.requestedItems.Count <= 0)
            return;

        // evita duplicar
        if (activeBubble != null)
            return;

        // cria bubble
        activeBubble =
            Instantiate(
                bubblePrefab,
                bubbleAnchor);

        // pega item principal
        ItemType mainItem =
            myOrder.requestedItems[0];

        // pega ícone
        Sprite icon =
            visualDatabase.GetIcon(mainItem);

        // configura visual
        activeBubble.Setup(
            icon,
            myOrder.visualColor);
    } 

    // destrói bubble atual
    void DestroyBubble()
    {
        // segurança
        if (activeBubble == null)
            return;

        Destroy(activeBubble.gameObject);

        activeBubble = null;
    }  
}