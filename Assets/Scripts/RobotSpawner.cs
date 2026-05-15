using UnityEngine;
using System.Collections.Generic;

// representa uma etapa do spawn
[System.Serializable]
public class SpawnStage
{
    [Header("Tempo da etapa")]
    public float startTime;
    public float endTime;

    [Header("Delay de spawn")]
    public float minSpawnDelay;
    public float maxSpawnDelay;
}

public class RobotSpawner : MonoBehaviour
{
    public GameObject robotPrefab;

    public RobotSlot[] slots;

    public Transform spawnPoint;
    public Transform exitPoint;

    public OrderManager orderManager;

    // lista de etapas do spawn
    public List<SpawnStage> spawnStages = new List<SpawnStage>();

    // timer interno do spawn
    private float timer;

    // próximo delay aleatório
    private float currentSpawnDelay;

    void Start()
    {
        currentSpawnDelay = GetCurrentSpawnDelay();
    }

    void Update()
    {
        if (!GameManager.Instance.IsGamePlaying()) return;
        
        timer += Time.deltaTime;

        // tenta spawnar quando atingir o delay atual
        if (timer >= currentSpawnDelay)
        {
            TrySpawnRobot();

            // reseta timer
            timer = 0f;

            // gera novo delay aleatório
            currentSpawnDelay = GetCurrentSpawnDelay();
        }
    }

    // pega delay aleatório baseado na etapa atual
    float GetCurrentSpawnDelay()
    {
        // calcula quanto tempo já passou da partida
        float elapsedTime =
            GameManager.Instance.matchTime -
            GameManager.Instance.GetTimeRemaining();

        // percorre todas as etapas
        foreach (SpawnStage stage in spawnStages)
        {
            // verifica se o tempo atual está dentro da etapa
            if (elapsedTime >= stage.startTime &&
                elapsedTime <= stage.endTime)
            {
                // retorna delay aleatório da etapa
                return Random.Range(
                    stage.minSpawnDelay,
                    stage.maxSpawnDelay
                );
            }
        }

        // fallback de segurança
        return 3f;
    }

   void TrySpawnRobot()
    {
        //  REMOVIDO limite de robôs

        // procura slot livre
        foreach (RobotSlot slot in slots)
        {
            if (!slot.isOccupied)
            {
                SpawnRobot(slot);
                return;
            }
        }
    }

    void SpawnRobot(RobotSlot slot)
    {
        GameObject robotGO = Instantiate(robotPrefab, spawnPoint.position, Quaternion.identity);

        RobotCustomer robot = robotGO.GetComponent<RobotCustomer>();

        // configura o robô
        robot.Setup(slot.GetPoint(), exitPoint, orderManager);

        // marca slot como ocupado
        slot.isOccupied = true;

        // libera slot quando robô sair
        StartCoroutine(FreeSlotWhenDestroyed(robot, slot));
    }

    int CountActiveRobots()
    {
        // conta todos robôs na cena
        return FindObjectsOfType<RobotCustomer>().Length;
    }

    System.Collections.IEnumerator FreeSlotWhenDestroyed(RobotCustomer robot, RobotSlot slot)
    {
        while (robot != null)
        {
            yield return null;
        }

        slot.isOccupied = false;
    }
}