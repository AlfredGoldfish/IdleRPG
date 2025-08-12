using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using IdleRPG.Core; // Wallet/Metal

/// Centralized save/load bootstrap for the Wallet owned by PlayerEconomy.
/// Robust hookup: tries in Start(), on scene load, and with a short retry.
/// Loads once per session, autosaves on change, and exposes ResetSave() for UI.
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

    private Wallet _wallet;                       // data object resolved from PlayerEconomy
    private float _nextWriteTime;
    private static bool _loadedThisSession;       // guard against duplicate loads
    private bool _hooked;                         // whether we've successfully hooked wallet

    private string PathFull => Path.Combine(Application.persistentDataPath, fileName);

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (autosaveOnChange && _wallet != null)
            _wallet.OnChanged -= OnWalletChanged;
    }

    private void Start()
    {
        TryHookup();
        if (!_hooked) StartCoroutine(HookupRetryFrames(60)); // try for ~1 sec at 60fps
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!_hooked) TryHookup();
    }

    private System.Collections.IEnumerator HookupRetryFrames(int frames)
    {
        for (int i = 0; i < frames && !_hooked; i++)
        {
            yield return null;
            TryHookup();
        }
    }

    private void TryHookup()
    {
        if (_hooked) return;

        var pe = PlayerEconomy.Instance ?? Object.FindAnyObjectByType<PlayerEconomy>();
        if (pe == null) return;

        _wallet = TryResolveWalletFromPlayerEconomy(pe);
        if (_wallet == null) return;

        if (loadOnStart && !_loadedThisSession)
        {
            WalletSave.LoadInto(_wallet); // idempotent set semantics
            _loadedThisSession = true;
        }

        if (autosaveOnChange)
            _wallet.OnChanged += OnWalletChanged;

        _hooked = true;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) SaveNow(); // silent save on pause
    }

    private void OnApplicationQuit()
    {
        if (saveOnQuit) SaveNow();
    }

    private void OnDestroy()
    {
        // Extra guard to persist in-editor if quit wasn't seen
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
        // Leave this log for manual Save Now actions; normal autosaves stay quiet.
        // Debug.Log($"[SystemsBootstrap] Saved wallet → {PathFull}");
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
