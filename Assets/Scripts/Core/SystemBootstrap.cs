using System.IO;
using UnityEngine;

namespace IdleRPG.Core
{
    /// <summary>
    /// Centralizes Wallet load/save so gameplay scripts don't do persistence.
    /// - Loads once on startup
    /// - Saves on quit/pause
    /// - Optional autosave-on-change (throttled)
    /// - Public Reset for a UI button
    /// </summary>
    [DefaultExecutionOrder(-200)]
    public class SystemsBootstrap : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Wallet wallet;          // drag your Wallet here

        [Header("Behavior")]
        [SerializeField] private bool loadOnStart = true;
        [SerializeField] private bool saveOnQuit = true;
        [SerializeField] private bool autosaveOnChange = true;
        [SerializeField] private float autosaveCooldown = 0.5f;

        [Header("File")]
        [SerializeField] private string fileName = "wallet.json";

        private float _nextWriteTime;
        private string PathFull => Path.Combine(Application.persistentDataPath, fileName);

        private void Awake()
        {
            if (wallet == null) wallet = FindAnyObjectByType<Wallet>();
            if (loadOnStart) WalletSave.LoadInto(wallet);   // your existing loader (additive; call once)
        }

        private void OnEnable()
        {
            if (autosaveOnChange && wallet != null)
                wallet.OnChanged += SaveThrottled;
        }

        private void OnDisable()
        {
            if (autosaveOnChange && wallet != null)
                wallet.OnChanged -= SaveThrottled;
        }

        private void OnApplicationPause(bool pause) { if (pause) SaveNow(); }
        private void OnApplicationQuit() { if (saveOnQuit) SaveNow(); }

        private void SaveThrottled()
        {
            if (Time.unscaledTime < _nextWriteTime) return;
            _nextWriteTime = Time.unscaledTime + autosaveCooldown;
            SaveNow();
        }

        [ContextMenu("Save Now")]
        public void SaveNow() => WalletSave.Save(wallet);     // uses your existing saver

        [ContextMenu("Reset Save")]
        public void ResetSave()
        {
            // zero in-memory, delete file, write clean file (optional)
            wallet.ClearAll();                                // add this to Wallet if you don't have it
            if (File.Exists(PathFull)) File.Delete(PathFull);
            WalletSave.Save(wallet);
#if UNITY_EDITOR
            Debug.Log($"[SystemsBootstrap] Reset → {PathFull}");
#endif
        }
    }
}
