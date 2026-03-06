using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab; // Prefab do item que esse spawner vai criar
    public float spawnDelay = 3f;
    private GameObject currentItem; // Guarda referência do item que está spawnado 
    private float timer;

    void Start()
    {
        SpawnItem();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentItem == null && currentItem.transform.parent != transform) 
        {
            currentItem = null; // Limpa referência
        }

        // Se não existir item, começa a contagem
        if(currentItem == null)
        {
            timer += Time.deltaTime; // Faz o tempo contar corretamente

            if(timer >= spawnDelay)
            {
                SpawnItem();
                timer = 0f;
            }
        }
    }
    void SpawnItem()
    {
        // Crie o item na posição do spawner
        currentItem = Instantiate(itemPrefab,transform.position,Quaternion.identity); 

        currentItem.transform.parent = transform;
    }
}
