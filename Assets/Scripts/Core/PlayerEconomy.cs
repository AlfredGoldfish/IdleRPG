using UnityEngine;
using IdleRPG.Core;

public class PlayerEconomy : MonoBehaviour
{
    public static PlayerEconomy Instance { get; private set; }
    public Wallet Wallet { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (Wallet == null) Wallet = new Wallet();
    }

    public static void EnsureExists()
    {
        if (Instance != null) return;
        var go = new GameObject("_PlayerEconomy");
        var pe = go.AddComponent<PlayerEconomy>();
        // Awake will initialize Wallet and DontDestroyOnLoad
    }

    // Convenience helpers expected by project code
    public void AddCurrency(Metal metal, ulong delta) => Wallet?.Add(metal, delta);
    public void AddCurrency(string metalKey, ulong delta) => Wallet?.Add(metalKey, delta);
}
