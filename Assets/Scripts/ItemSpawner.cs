using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab; // Prefab do item que será criado
    public float spawnDelay = 2f; 

    private GameObject currentItem; // Item atual
    private float timer = 0f;

    void Start()
    {
        SpawnItem(); 
    }

    void Update()
    {
        // Se existe item mas ele não é mais filho do spawner (foi pego)
        if (currentItem != null && currentItem.transform.parent != transform)
        {
            currentItem = null; // Limpa referência
        }

        // Se não existe item, começa contagem
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

        // Faz o item nascer como filho do spawner
        currentItem.transform.parent = transform;
    }
}