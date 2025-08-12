using UnityEngine;
using IdleRPG.Core;

//
// Lives once and exposes the Wallet + helper methods.
// Loads once on boot; saves on quit. Dev hotkey C adds 1 copper.
//
public class PlayerEconomy : MonoBehaviour
{
    public static PlayerEconomy Instance { get; private set; }

    // The single data model instance (POCO). Not serialized by Unity (it contains a Dictionary).
    private readonly Wallet wallet = new Wallet();

    // Expose for other systems (both names to avoid refactor churn).
    public Wallet WalletData => wallet;
    public Wallet Wallet => wallet;

    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load once at startup (Json is additive into an empty wallet).
        WalletSave.LoadInto(wallet);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddCurrency(Metal.Copper, 1);
            Debug.Log("Added 1 copper (test)");
        }
    }
#endif

    private void OnApplicationQuit()
    {
        // Persist on exit
        WalletSave.Save(wallet);
    }

    // Convenience for coins/chests/quests/etc.
    public void AddCurrency(Metal metal, ulong amount) => wallet.Add(metal, amount);
}
