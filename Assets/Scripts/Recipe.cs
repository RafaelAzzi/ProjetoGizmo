using UnityEngine;

[CreateAssetMenu(menuName = "Gizmo/Recipe")]
public class Recipe : ScriptableObject
{
    public ItemType itemA; // primeiro item
    public ItemType itemB; // segundo item    

    public GameObject resultPrefab; // resultado

    public float craftTime; // tempo futuro (ainda não vamos usar)
    // teste
}