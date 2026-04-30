using UnityEngine;

public class RobotSpawner : MonoBehaviour
{
    public GameObject robotPrefab;

    public RobotSlot[] slots;

    public Transform spawnPoint;
    public Transform exitPoint;

    public OrderManager orderManager;

    public DifficultyManager difficultyManager;

    private float timer;

    void Update()
    {
        if (!GameManager.Instance.IsGamePlaying()) return;
        
        timer += Time.deltaTime;

        // só tenta spawnar quando atingir o delay da fase
        if (timer >= difficultyManager.GetSpawnDelay())
        {
            TrySpawnRobot();
            timer = 0f;
        }
    }

    void TrySpawnRobot()
    {
        // conta quantos robôs existem atualmente
        int currentRobots = CountActiveRobots();

        // pega limite da fase
        int maxRobots = difficultyManager.GetMaxRobots();

        // se já atingiu limite, não faz nada
        if (currentRobots >= maxRobots)
            return;

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