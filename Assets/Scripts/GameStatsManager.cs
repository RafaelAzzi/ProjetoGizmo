using UnityEngine;

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance;

    // ===== ESTATÍSTICAS =====

    public int ordersCompleted = 0;
    public int ordersFailed = 0;

    public int rareItemsDelivered = 0;
    public int legendaryItemsDelivered = 0;
    public int oilsDelivered = 0;

    void Awake()
    {
        // singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        ResetStats();
    }

    // ===== RESET =====
    public void ResetStats()
    {
        ordersCompleted = 0;
        ordersFailed = 0;

        rareItemsDelivered = 0;
        legendaryItemsDelivered = 0;
        oilsDelivered = 0;
    }
}