using UnityEngine;

[CreateAssetMenu(menuName = "Gizmo/Item Status Icon Database")]
public class ItemStatusIconDatabase : ScriptableObject
{
    [Header("Ícone de pronto")]
    public Sprite readyIcon;

    [Header("Ícone de alerta")]
    public Sprite alertIcon;

    [Header("Ícone de estragado")]
    public Sprite spoiledIcon;
}