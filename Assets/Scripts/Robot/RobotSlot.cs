using UnityEngine;

// representa um ponto onde o robô pode ficar
public class RobotSlot : MonoBehaviour
{
    public bool isOccupied = false;

    // ponto para onde o robô deve olhar
    public Transform lookPoint;

    // retorna posição do slot
    public Transform GetPoint()
    {
        return transform;
    }
}