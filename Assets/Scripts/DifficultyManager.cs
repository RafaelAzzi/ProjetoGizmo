using UnityEngine;

// Gerencia qual fase está ativa
public class DifficultyManager : MonoBehaviour
{
    public DifficultySettings currentDifficulty;

    // tempo entre spawns
    public float GetSpawnDelay()
    {
        return currentDifficulty.spawnDelay;
    }
}
