using UnityEngine;

// Gerencia qual fase está ativa
public class DifficultyManager : MonoBehaviour
{
    public DifficultySettings currentDifficulty;

    // retorna quantos robôs podem existir
    public int GetMaxRobots()
    {
        return currentDifficulty.maxRobots;
    }

    // tempo entre spawns
    public float GetSpawnDelay()
    {
        return currentDifficulty.spawnDelay;
    }

    // tempo de pedido
    public float GetOrderTime()
    {
        return currentDifficulty.orderTime;
    }
}
