using System.IO;
using System.Reflection;
using UnityEngine;
using IdleRPG.Core; // for Wallet/Metal

/// Centralized save/load bootstrap for the Wallet owned by PlayerEconomy.
/// - Loads once on startup
/// - Saves on pause/quit
/// - Optional autosave when the wallet changes (throttled)
[DefaultExecutionOrder(-200)]
public class SystemsBootstrap : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] private bool loadOnStart = true;
    [SerializeField] private bool saveOnQuit = true;
    [SerializeField] private bool autosaveOnChange = true;
    [SerializeField] private float autosaveCooldown = 0.5f;

    [Header("File")]
    [SerializeField] private string fileName = "wallet.json"; // must match WalletSave

    private Wallet _wallet;         // data object resolved from PlayerEconomy
    private float _nextWriteTime;

    private string PathFull => Path.Combine(Application.persistentDataPath, fileName);

    private void Awake()
    {
        var pe = PlayerEconomy.Instance;
        if (pe == null)
        {
            Debug.LogWarning("[SystemsBootstrap] PlayerEconomy.Instance not found. Save/Load disabled.");
            return;
        }

        // Try to resolve Wallet model from PlayerEconomy in a tolerant way.
        _wallet = TryResolveWalletFromPlayerEconomy(pe);

        if (_wallet == null)
        {
            Debug.LogWarning("[SystemsBootstrap] Could not resolve Wallet from PlayerEconomy. Save/Load disabled.");
            return;
        }

        if (loadOnStart)
        {
            WalletSave.LoadInto(_wallet); // Load is additive; call ONCE at startup
    #if UNITY_EDITOR
            Debug.Log($"[SystemsBootstrap] Loaded wallet from {PathFull}");
    #endif
        }
    }

    private void OnEnable()
    {
        if (autosaveOnChange && _wallet != null)
            _wallet.OnChanged += OnWalletChanged;
    }

    private void OnDisable()
    {
        if (autosaveOnChange && _wallet != null)
            _wallet.OnChanged -= OnWalletChanged;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) SaveNow();
    }

    private void OnApplicationQuit()
    {
        if (saveOnQuit) SaveNow();
    }

    private void OnWalletChanged(Metal m, ulong _)
    {
        if (Time.unscaledTime < _nextWriteTime) return;
        _nextWriteTime = Time.unscaledTime + autosaveCooldown;
        SaveNow();
    }

    [ContextMenu("Save Now")]
    public void SaveNow()
    {
        if (_wallet == null) return;
        WalletSave.Save(_wallet);
    #if UNITY_EDITOR
        Debug.Log($"[SystemsBootstrap] Saved wallet → {PathFull}");
    #endif
    }

    [ContextMenu("Reset Save")]
    public void ResetSave()
    {
        if (_wallet == null) return;
        _wallet.ClearAll();
        if (File.Exists(PathFull)) File.Delete(PathFull);
        WalletSave.Save(_wallet); // write a clean zeroed file (optional)
    #if UNITY_EDITOR
        Debug.Log($"[SystemsBootstrap] Reset wallet & file at {PathFull}");
    #endif
    }

    private static Wallet TryResolveWalletFromPlayerEconomy(PlayerEconomy pe)
    {
        // Try common property names first
        var t = pe.GetType();
        foreach (var propName in new[] { "Wallet", "WalletData" })
        {
            var p = t.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (p != null && typeof(Wallet).IsAssignableFrom(p.PropertyType))
            {
                var val = p.GetValue(pe) as Wallet;
                if (val != null) return val;
            }
        }

        // Try to find a field of type Wallet
        foreach (var f in t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (typeof(Wallet).IsAssignableFrom(f.FieldType))
            {
                var val = f.GetValue(pe) as Wallet;
                if (val != null) return val;
            }
        }

        return null;
    }
}
