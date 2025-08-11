using System.IO;
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

    // Must match your WalletSave's filename.
    [SerializeField] private string fileName = "wallet.json";

    private Wallet _wallet;         // the actual data object from PlayerEconomy
    private float _nextWriteTime;   // throttle for autosave

    private string PathFull => Path.Combine(Application.persistentDataPath, fileName);

    private void Awake()
    {
        // Grab the single PlayerEconomy in the scene and its Wallet.
        var pe = PlayerEconomy.Instance;
        if (pe == null)
        {
            Debug.LogWarning("[SystemsBootstrap] PlayerEconomy.Instance not found. Save/Load disabled.");
            return;
        }

        // NOTE: Add this one-liner property to your PlayerEconomy if you don't already have it:
        // public IdleRPG.Core.Wallet Wallet => wallet;
        _wallet = pe.WalletData;

        if (_wallet == null)
        {
            Debug.LogWarning("[SystemsBootstrap] PlayerEconomy.Wallet is null. Save/Load disabled.");
            return;
        }

        if (loadOnStart)
        {
            // Uses your existing WalletSave utility in the project.
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
}
