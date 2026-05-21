using UnityEngine;
using System.Collections;

// spawner pode ser interagido
public class ItemSpawner : MonoBehaviour, IInteractable
{
    public GameObject itemPrefab; 
    public float spawnDelay = 2f;

    private GameObject currentItem; // Item atual
    private float timer = 0f;

    void Start()
    {
        SpawnItem();
    }

    void Update()
    {
        // Se não existe item, começa contagem para respawn
        if (currentItem == null)
        {
            timer += Time.deltaTime;

            if (timer >= spawnDelay)
            {
                SpawnItem();
                timer = 0f;
            }
        }
    }

    void SpawnItem()
    {
        currentItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);

        // item nasce como filho do spawner
        currentItem.transform.parent = transform;

        // desativa física
        Rigidbody rb = currentItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // inicia rotina para mostrar ícone
        StartCoroutine(ShowSpawnIcon());
    }

    // ===== INTERAÇÃO DO PLAYER =====
    public void Interact(Player player)
    {
        // se não tem item, não faz nada
        if (currentItem == null) return;

        // se player já tem item, não pega
        if (player.HasItem()) return;

        // pega o script Item
        Item item = currentItem.GetComponent<Item>();

        // player pega o item
        player.PickupItem(item);

        // remove referência (para permitir respawn)
        currentItem = null;
    }

    // espera 1 frame antes de mostrar o ícone
    IEnumerator ShowSpawnIcon()
    {
        // espera o Start() do Item acontecer
        yield return null;

        // segurança
        if (currentItem == null)
            yield break;

        // pega item
        Item item = currentItem.GetComponent<Item>();

        // mostra ícone
        if (item != null)
        {
            item.ShowIcon();
        }
    }
}