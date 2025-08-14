using UnityEngine;
using System;

// Global namespace on purpose to match existing references without needing 'using' edits.
public class PlayerEconomy : MonoBehaviour
{
    public static PlayerEconomy Instance { get; private set; }

    // Wallet host
    public IdleRPG.Core.Wallet Wallet { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize wallet if needed
        Wallet ??= new IdleRPG.Core.Wallet();
    }

    /// <summary>
    /// Ensure there's a PlayerEconomy in the scene. Creates one on demand if missing.
    /// </summary>
    public static PlayerEconomy EnsureExists()
    {
        if (Instance != null) return Instance;

        var host = new GameObject("_PlayerEconomy");
        var pe = host.AddComponent<PlayerEconomy>();
        return pe;
    }
}
