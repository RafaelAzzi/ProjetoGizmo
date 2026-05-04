using UnityEngine;

// Script que define os dados de uma fase
[CreateAssetMenu(fileName = "NewDifficulty", menuName = "Game/Difficulty")]
public class DifficultySettings : ScriptableObject
{
    public float spawnDelay = 4f;    // tempo entre tentativas de spawn
    public float orderTime = 40f;    // tempo para completar pedido
}