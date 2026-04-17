using UnityEngine;

public class RobotSpawner : MonoBehaviour
{
    public GameObject robotPrefab;

    public RobotSlot[] slots;

    public Transform spawnPoint;
    public Transform exitPoint;

    public OrderManager orderManager;

    public float spawnInterval = 5f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            TrySpawnRobot();
            timer = 0f;
        }
    }

    void TrySpawnRobot()
    {
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

    System.Collections.IEnumerator FreeSlotWhenDestroyed(RobotCustomer robot, RobotSlot slot)
    {
        while (robot != null)
        {
            yield return null;
        }

        slot.isOccupied = false;
    }
}