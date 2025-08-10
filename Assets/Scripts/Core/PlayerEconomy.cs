using UnityEngine;

//
// Lives once and exposes the Wallet + helper methods.
// This version includes tiny hooks to load/save the wallet as JSON.
//
public class PlayerEconomy : MonoBehaviour
{
    public static PlayerEconomy Instance { get; private set; }

    public Wallet Wallet { get; private set; } = new Wallet();

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Optional: load wallet on boot if file exists
        WalletSave.LoadInto(Wallet);
    }

#if UNITY_EDITOR
    // Debug hotkey: add 123 copper
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddCurrency(Metal.Copper, 123);
            Debug.Log("Added 123 copper (test)");
        }
    }
#endif

    void OnApplicationQuit()
    {
        // Optional: persist wallet to JSON on quit
        WalletSave.Save(Wallet);
    }

    // Convenience helper for other systems (enemies, chests, quests)
    public void AddCurrency(Metal metal, ulong amount) => Wallet.Add(metal, amount);
}
