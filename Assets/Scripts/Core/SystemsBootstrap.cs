using UnityEngine;

namespace IdleRPG.Core
{
    /// <summary>
    /// Guarantees PlayerEconomy exists and can later be expanded to wire save/load and other singletons.
    /// </summary>
    public class SystemsBootstrap : MonoBehaviour
    {
        [Tooltip("When true, marks PlayerEconomy as DontDestroyOnLoad")]
        public bool persistAcrossScenes = true;

        private void Start()
        {
            var pe = PlayerEconomy.EnsureExists();
            if (persistAcrossScenes)
            {
                // Already set in Awake, but harmless to repeat
                DontDestroyOnLoad(pe.gameObject);
            }

            // Hook wallet changed here later for autosave if/when WalletSave exists.
            // Example (guarded): WalletSave.TryAutoHook(pe.Wallet);
        }

#if UNITY_EDITOR
        [ContextMenu("Log Wallet Totals")]
        private void LogTotals()
        {
            var pe = PlayerEconomy.EnsureExists();
            // Basic log for quick sanity checks; replace with your metals as needed.
            long copper = pe.Wallet.Get((Metal)0);
            long silver = pe.Wallet.Get((Metal)1);
            long gold   = pe.Wallet.Get((Metal)2);
            long plat   = pe.Wallet.Get((Metal)3);
            Debug.Log($"Wallet — Cu:{copper} Ag:{silver} Au:{gold} Pt:{plat}");
        }
#endif
    }
}
