using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        // faz o ícone sempre olhar para a câmera
        if (Camera.main != null)
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}
