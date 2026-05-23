using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

// Bancada de bebidas com tempo
public class DrinkBench : MonoBehaviour, IInteractable
{
    [Header("UI")]
    public Vector3 progressBarOffset = new Vector3(0, 1f, 0);

    [Header("Tick Alert")]

    // intervalo inicial
    public float slowTickInterval = 1.2f;

    // intervalo final
    public float fastTickInterval = 0.25f;

    // tempo antes dos ticks começarem
    public float tickStartDelay = 2f;

    [System.Serializable]
    public class DrinkSlot : IItemHolder
    {
        public Transform holdPoint;
        // ponto onde a barra de progresso aparecerá
        public Transform uiAnchor;

        private Item currentItem;

        public float timer;
        public float maxTime;

        public bool isProcessing;
        public bool isReady;

        public float readyTimer;

        public GameObject progressBarInstance;
        public Slider progressBar;

        // AudioSource dedicado do slot
        public AudioSource audioSource;

        // timer do tick
        public float tickTimer;

        // intervalo atual do tick
        public float currentTickInterval;

        // delay antes do primeiro tick
        public float tickDelayTimer;

        public Transform GetHoldPoint() => holdPoint;
        public void SetItem(Item item) => currentItem = item;
        public Item GetItem() => currentItem;
        public void ClearItem() => currentItem = null;
        public bool HasItem() => currentItem != null;
    }

    public List<DrinkSlot> slots = new List<DrinkSlot>();

    [Header("Configuração")]
    public float processTime = 7f;
    public float readyDuration = 5f;

    public GameObject progressBarPrefab;

    [Header("Itens permitidos")]
    public List<ItemType> allowedItems;

    public float interactDistance = 2.5f;

    void Start()
    {
        SetupAudioSources();
    }

    void Update()
    {
        // pausa processamento se jogo estiver pausado
        if (
            PauseManager.Instance != null &&
            PauseManager.Instance.IsPaused()
        )
        {
            StopAllSlotAudio();
            return;
        }

        // não processa após fim da partida
        if (!GameManager.Instance.IsGamePlaying())
        {
            StopAllSlotAudio();
            return;
        }
        ProcessSlots();

        // garante loops ativos dos slots processando
        RestoreProcessingLoops();
    }

    // cria AudioSource para cada slot
void SetupAudioSources()
    {
        foreach (var slot in slots)
        {
            // cria objeto filho
            GameObject audioObject =
                new GameObject("DrinkAudioSource");

            // define como filho do slot
            audioObject.transform.SetParent(
                slot.holdPoint
            );

            // zera posição local
            audioObject.transform.localPosition =
                Vector3.zero;

            // adiciona AudioSource
            AudioSource source =
                audioObject.AddComponent<AudioSource>();

            // configurações
            source.playOnAwake = false;
            source.loop = true;

            // envia para mixer SFX
            source.outputAudioMixerGroup =
                Resources.Load<AudioMixer>(
                    "MainAudioMixer"
                ).FindMatchingGroups("SFX")[0];

            // salva referência
            slot.audioSource = source;
        }
    }

    // ===== PROCESSAMENTO =====
    void ProcessSlots()
    {
        foreach (var slot in slots)
        {
            if (!slot.HasItem()) continue;

            Item item = slot.GetItem();

            // ===== PROCESSANDO =====
            if (slot.isProcessing)
            {
                slot.timer += Time.deltaTime;

                if (slot.progressBar != null)
                {
                    slot.progressBar.value = slot.timer / slot.maxTime;
                }

                if (slot.timer >= slot.maxTime)
                {
                    if (slot.progressBarInstance != null)
                    {
                        Destroy(slot.progressBarInstance);
                    }

                    slot.isProcessing = false;
                    // para som contínuo
                    StopLoopSound(slot);

                    slot.isReady = true;

                    // reseta sistema de tick
                    slot.tickTimer = 0f;

                    // reseta delay inicial
                    slot.tickDelayTimer = 0f;

                    // começa com tick lento
                    slot.currentTickInterval =
                        slowTickInterval;

                    // toca som de pronto
                    SFXManager.Instance.PlaySFX(
                        SFXType.DrinkReady
                    );

                    // mostra ícone de pronto
                    item.ShowReadyIcon();

                    item.isProcessed = true;

                    // DEFINE COMO PERFEITO
                    item.quality = ItemQuality.Perfect;

                    slot.timer = slot.maxTime;
                    slot.readyTimer = 0f;

                    Debug.Log("Item PERFEITO");
                }
            }
            // ===== APÓS PRONTO =====
            else if (slot.isReady)
            {
                slot.readyTimer += Time.deltaTime;

                // processa ticks de alerta
                ProcessTickAlert(slot);

                // PERFECT → OVERCOOKED
                if (
                    item.quality == ItemQuality.Perfect &&
                    slot.readyTimer >= readyDuration
                )
                {
                    item.quality = ItemQuality.Overcooked;

                    Debug.Log("Item passou do ponto!");
                }

                // OVERCOOKED → SPOILED
                else if (
                    item.quality == ItemQuality.Overcooked &&
                    slot.readyTimer >= readyDuration * 2f
                )
                {
                    item.quality = ItemQuality.Spoiled;

                    // toca som de estragado
                    SFXManager.Instance.PlaySFX(
                        SFXType.DrinkSpoiled
                    );

                    // troca visual para estragado
                    item.SetSpoiledVisual();

                    // para sistema de alerta
                    slot.isReady = false;

                    // visual
                    Renderer rend = item.GetComponent<Renderer>();

                    if (rend != null)
                    {
                        rend.material.color =
                            new Color(0.4f, 0.2f, 0.1f);
                    }

                    Debug.Log("Item ESTRAGOU!");
                }
            }
        }
    }

    // ===== INTERAÇÃO =====
    public void Interact(Player player)
    {
        if (player.HasItem())
            TryPlaceItem(player);
        else
            TryTakeItem(player);
    }

    // ===== COLOCAR ITEM =====
    void TryPlaceItem(Player player)
    {
        Item playerItem = player.GetItem();
        if (playerItem == null) return;

        if (!allowedItems.Contains(playerItem.itemType) || playerItem.isProcessed)
            return;

        DrinkSlot closestSlot = GetClosestAvailableSlot(player.transform.position);
        if (closestSlot == null) return;

        float distance = Vector3.Distance(player.transform.position, closestSlot.holdPoint.position);
        if (distance > interactDistance) return;

        playerItem.SetHolder(closestSlot);

        // marca que o item já passou pela DrinkBench
        playerItem.hasPassedDrinkBench = true;

        closestSlot.timer = playerItem.processProgress;
        closestSlot.maxTime = processTime;
        closestSlot.isProcessing = true;
            // inicia som contínuo
            StartLoopSound(closestSlot);

        closestSlot.isReady = false;
        closestSlot.readyTimer = 0f;

     // posição da barra
    Vector3 barPosition;

    // usa UIAnchor se existir
    if (closestSlot.uiAnchor != null)
    {
        barPosition = closestSlot.uiAnchor.position;
    }
    else
    {
        // fallback antigo
        barPosition =
            closestSlot.holdPoint.position +
            progressBarOffset;
    }

    closestSlot.progressBarInstance = Instantiate(
        progressBarPrefab,
        barPosition,
        closestSlot.uiAnchor.rotation
    );

        closestSlot.progressBar = closestSlot.progressBarInstance.GetComponentInChildren<Slider>();
    }

    // ===== PEGAR ITEM =====
    void TryTakeItem(Player player)
    {
        DrinkSlot closestSlot = GetClosestAnySlot(player.transform.position);
        if (closestSlot == null) return;

        float distance = Vector3.Distance(player.transform.position, closestSlot.holdPoint.position);
        if (distance > interactDistance) return;

        Item item = closestSlot.GetItem();

        // salva progresso
        item.processProgress = closestSlot.timer;

        item.SetHolder(player);

        // ===== DEFINE QUALIDADE FINAL AO PEGAR =====
        if (!item.isProcessed)
        {
            float progressPercent = closestSlot.timer / closestSlot.maxTime;

            // MENOS DE 50% → CRU
            if (progressPercent < 0.5f)
            {
                item.quality = ItemQuality.Crude;
                Debug.Log("Pegou CRU");
            }
            else
            {
                item.quality = ItemQuality.Undercooked;
                Debug.Log("Pegou INCOMPLETO");
            }
        }
        else if (item.quality == ItemQuality.Overcooked)
        {
            Debug.Log("Pegou PASSOU DO PONTO");
        }
        else if (item.quality == ItemQuality.Spoiled)
        {
            Debug.Log("Pegou ESTRAGADO");
        }
        else
        {
            Debug.Log("Pegou PERFEITO");
        }

        // limpa UI
        if (closestSlot.progressBarInstance != null)
        {
            Destroy(closestSlot.progressBarInstance);
        }

        // para som contínuo
        StopLoopSound(closestSlot);

        closestSlot.ClearItem();
        closestSlot.timer = 0f;
        closestSlot.isReady = false;
        closestSlot.isProcessing = false;
    }

    // ===== SLOT VAZIO =====
    DrinkSlot GetClosestAvailableSlot(Vector3 playerPos)
    {
        DrinkSlot closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var slot in slots)
        {
            if (!slot.HasItem())
            {
                float distance = Vector3.Distance(playerPos, slot.holdPoint.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = slot;
                }
            }
        }

        return closest;
    }

    // ===== QUALQUER SLOT =====
    DrinkSlot GetClosestAnySlot(Vector3 playerPos)
    {
        DrinkSlot closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var slot in slots)
        {
            if (slot.HasItem())
            {
                float distance = Vector3.Distance(playerPos, slot.holdPoint.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = slot;
                }
            }
        }

        return closest;
    }

    // inicia som contínuo do slot
    void StartLoopSound(DrinkSlot slot)
    {
        // pega clip do database
        AudioClip clip =
            SFXManager.Instance
            .sfxDatabase
            .GetClip(SFXType.DrinkLoop);

        // segurança
        if (clip == null)
            return;

        // configura source
        slot.audioSource.clip = clip;

        // toca loop
        slot.audioSource.Play();
    }

    // para som contínuo do slot
    void StopLoopSound(DrinkSlot slot)
    {
        // segurança
        if (slot.audioSource == null)
            return;

        slot.audioSource.Stop();
    }

    // restaura loops após pause
    void RestoreProcessingLoops()
    {
        foreach (var slot in slots)
        {
            // ignora slots inválidos
            if (!slot.HasItem())
                continue;

            // só restaura se estiver processando
            if (!slot.isProcessing)
                continue;

            // segurança
            if (slot.audioSource == null)
                continue;

            // já está tocando
            if (slot.audioSource.isPlaying)
                continue;

            // restaura loop
            StartLoopSound(slot);
        }
    }

    // processa ticks de alerta
    void ProcessTickAlert(DrinkSlot slot)
    {
        // acumula delay inicial
        slot.tickDelayTimer += Time.deltaTime;

        // espera delay antes dos ticks
        if (slot.tickDelayTimer < tickStartDelay)
            return;

        // acumula tempo do tick
        slot.tickTimer += Time.deltaTime;

        // calcula progresso até estragar
        float progress =
            slot.readyTimer / readyDuration;

        // reduz intervalo gradualmente
        slot.currentTickInterval =
            Mathf.Lerp(
                slowTickInterval,
                fastTickInterval,
                progress
            );

        // toca tick quando atingir intervalo
        if (slot.tickTimer >= slot.currentTickInterval)
        {
            // reseta timer
            slot.tickTimer = 0f;

            // toca tick
            SFXManager.Instance.PlaySFX(
                SFXType.DrinkTick
            );

            // mostra alerta visual
            slot.GetItem().ShowAlertIcon();
        }
    }

    // para todos os sons da DrinkBench
    void StopAllSlotAudio()
    {
        foreach (var slot in slots)
        {
            // para loop contínuo
            if (slot.audioSource != null)
            {
                slot.audioSource.Stop();
            }
        }
    }
}