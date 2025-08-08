using UnityEngine;

/// Lives once in the scene (mark DontDestroyOnLoad so it persists)
public class PlayerEconomy : MonoBehaviour
{
    public static PlayerEconomy Instance { get; private set; }

    public Wallet Wallet { get; private set; } = new Wallet();

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ─── TEST ONLY ───
#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddCurrency(Metal.Copper, 123);
            Debug.Log("Added 123 copper (test)");
        }
    }
#endif
    //────────────────

    /// Convenience helper for other systems
    public void AddCurrency(Metal metal, ulong amount) => Wallet.Add(metal, amount);
}
