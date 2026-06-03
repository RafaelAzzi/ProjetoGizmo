using System.Collections.Generic;
using UnityEngine;

// representa o robô cliente
public class RobotCustomer : MonoBehaviour, IInteractable
{
    public float moveSpeed = 3f;

    // ponto onde ele vai parar (slot)
    private Transform targetPoint;

    // ponto intermediário antes do slot
    private Transform entryPoint;

    // ponto para onde o robô olha ao chegar
    private Transform lookPoint;

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

    // já chegou no ponto intermediário?
    private bool reachedEntryPoint = false;

    // controle de pedido
    private bool hasOrder = false;
    private Order myOrder;

    // anchor do bubble
    public Transform bubbleAnchor;

    // prefab do bubble
    public RobotThoughtBubble bubblePrefab;

    // bubble atual
    private RobotThoughtBubble activeBubble;

    [Header("Robot Audio")]

    // intervalo do estado crítico
    public float minCriticalDelay = 4f;
    public float maxCriticalDelay = 5f;

    // volume do robô
    [Range(0f, 1f)]
    public float audioVolume = 0.15f;

    // spatial blend leve
    [Range(0f, 1f)]
    public float spatialBlend = 0.25f;

    // clip reservado para esse robô
    private AudioClip robotLoopClip;

    // source das chamadas críticas
    private AudioSource criticalAudioSource;

    // controla estado crítico
    private bool criticalAudioActive = false;

    // timer entre chamadas críticas
    private float criticalTimer;

    // delay atual do crítico
    private float currentCriticalDelay;

    // banco visual dos ícones
    public ItemVisualDatabase visualDatabase;

    // distância para considerar que chegou no destino
    public float stopDistance = 0.1f;

    // ===== CONFIGURAÇÃO INICIAL =====
    public void Setup(
    Transform entry,
    Transform target,
    Transform look,
    Transform exit,
    OrderManager manager)
    {
        entryPoint = entry;

        targetPoint = target;

        lookPoint = look;

        exitPoint = exit;

        orderManager = manager;
    }

    void Start()
    {
        SetupAudio();
    }

    void Update()
    {
        // controla pause
        HandlePauseAudio();

        // controla game over
        HandleGameOverAudio();

        // ===== IR ATÉ O SLOT =====
        if (!isWaiting && !isLeaving)
        {
            // ainda não chegou no ponto intermediário
            if (!reachedEntryPoint)
            {
                MoveToTarget(entryPoint.position);

                if (Vector3.Distance(
                    transform.position,
                    entryPoint.position) < stopDistance)
                {
                    reachedEntryPoint = true;
                }

                return;
            }

            // já passou pelo ponto intermediário
            MoveToTarget(targetPoint.position);

            if (Vector3.Distance(
                transform.position,
                targetPoint.position) < stopDistance)
            {
                isWaiting = true;

                // faz o robô olhar para a oficina
                if (lookPoint != null)
                {
                    Vector3 lookDirection =
                        lookPoint.position - transform.position;

                    lookDirection.y = 0f;

                    if (lookDirection != Vector3.zero)
                    {
                        transform.forward =
                            lookDirection.normalized;
                    }
                }

                PlayCriticalCall();

                if (!hasOrder)
                {
                    myOrder = orderManager.GenerateNewOrder();

                    if (myOrder != null)
                    {
                        hasOrder = true;
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

                // toca som de pedido expirado
                SFXManager.Instance.PlaySFX(
                    SFXType.DeliveryFail
                );

                // remove bubble
                DestroyBubble();

                isWaiting = false;
                isLeaving = true;
            }
        }

        // controla áudio crítico
        // HandleCriticalAudio();

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

    // configura áudio do robô
    void SetupAudio()
    {
        // ===== SOURCE DAS FALAS =====

        criticalAudioSource =
            gameObject.AddComponent<AudioSource>();

        criticalAudioSource.playOnAwake = false;
        criticalAudioSource.loop = false;

        criticalAudioSource.volume =
            audioVolume;

        criticalAudioSource.spatialBlend =
            spatialBlend;

        criticalAudioSource.minDistance = 3f;
        criticalAudioSource.maxDistance = 15f;

        // reserva clip exclusivo
        robotLoopClip =
            RobotAudioManager.Instance
                .ReserveClip();
    }

    // controla áudio de urgência
    void HandleCriticalAudio()
    {
        // precisa ter pedido
        if (!hasOrder)
            return;

        // ignora se saiu
        if (isLeaving)
            return;

        // ignora se pedido sumiu
        if (!orderManager.activeOrders.Contains(myOrder))
            return;

        // calcula porcentagem restante
        float percent =
            myOrder.timeRemaining /
            myOrder.maxTime;

        // abaixo de 50%
        bool isCritical =
            percent <= 0.5f;

        // não crítico
        if (!isCritical)
        {
            criticalAudioActive = false;
            return;
        }

        // entra no crítico
        if (!criticalAudioActive)
        {
            criticalAudioActive = true;

            // sorteia primeiro delay
            currentCriticalDelay =
                Random.Range(
                    minCriticalDelay,
                    maxCriticalDelay
                );

            criticalTimer = 0f;
        }

        // acumula timer
        criticalTimer += Time.deltaTime;

        // espera delay
        if (criticalTimer < currentCriticalDelay)
            return;

        // reseta timer
        criticalTimer = 0f;

        // sorteia próximo delay
        currentCriticalDelay =
            Random.Range(
                minCriticalDelay,
                maxCriticalDelay
            );

        // toca chamada
        PlayCriticalCall();
    }

    // toca chamada crítica
    void PlayCriticalCall()
    {
        // segurança
        if (robotLoopClip == null)
            return;

        // segurança
        if (criticalAudioSource == null)
            return;

        // evita sobrepor chamadas
        if (criticalAudioSource.isPlaying)
            return;

        // toca chamada
        criticalAudioSource.PlayOneShot(
            robotLoopClip
        );
    }

   // para áudio do robô
    void StopRobotAudio()
    {
        // para chamadas
        if (
            criticalAudioSource != null &&
            criticalAudioSource.isPlaying
        )
        {
            criticalAudioSource.Stop();
        }
    }

    // controla pause
    void HandlePauseAudio()
    {
        if (
            PauseManager.Instance != null &&
            PauseManager.Instance.IsPaused()
        )
        {
            StopRobotAudio();
        }
    }

    // controla fim de partida
    void HandleGameOverAudio()
    {
        if (!GameManager.Instance.IsGamePlaying())
        {
            StopRobotAudio();
        }
    }

    void OnDestroy()
    {
        // libera clip reservado
        if (RobotAudioManager.Instance != null)
        {
            RobotAudioManager.Instance
                .ReleaseClip(robotLoopClip);
        }
    }
}