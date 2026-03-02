using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10f; 
    public Transform holdPoint; // Onde o obejto vai ficar
    public float pickupRange = 2f; // Distância para pegar o objeto
    public KeyCode pickupKey = KeyCode.E; 
    private GameObject heldObject; // Guardar qual obejto está sendo segurado no momento 

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal"); // Pega os Input padrão da Unity, que vai de -1 a 1
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical); // Cria vetor de movimento, neste caso X e Z

        direction = Quaternion.Euler(0, 45, 0) * direction; // Faz o personagem andar de acordo com o ângulo da câmera
        direction = direction.normalized;

        transform.Translate(direction * speed * Time.deltaTime, Space.World); 
        // Movimento do personagem: Direção calculada, velocidade, movimento independente do frame e o Space World que move no eixo global do mundo

        if (Input.GetKeyDown(pickupKey)) // Sistema de pegar obejto
        {
            if (heldObject == null)
            {
                TryPickup();
            }
            else
            {
                DropObject();
            }
        }
        
    }
    void TryPickup()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange); 
        // Criar uma esfera em volta do jogador. Todos os colliders dentro dessa esfera são detectados

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Pickable")) // Só pega objetos com a Tag Pickable
            {
                heldObject = hit.gameObject; // Ao pegar, guarda o objeto
                Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                rb.useGravity = false; // Desliga a gravidade e impede o objeto de cair
                rb.isKinematic = true;

                heldObject.transform.position = holdPoint.position; // Move o obejto para o HoldPoint e faz ele acompanhar o jogador
                heldObject.transform.parent = holdPoint; 
                
                break;
            }
        }
    }
    void DropObject() // Remove o objeto do jogador 
    {
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        heldObject.transform.parent = null; 

        rb.useGravity = true; // Reativa a física 
        rb.isKinematic = false;

        heldObject = null; // Não está segurando mais nada 

    }
}
