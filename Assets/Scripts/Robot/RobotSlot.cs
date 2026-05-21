using UnityEngine;

// representa um ponto onde o robô pode ficar
public class RobotSlot : MonoBehaviour
{
    public bool isOccupied = false;

    // retorna posição do slot
    public Transform GetPoint()
    {
        return transform;
    }
}