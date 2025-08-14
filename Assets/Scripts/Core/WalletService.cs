using System;
using UnityEngine;

namespace IdleRPG.Core
{
    /// <summary>
    /// Bridges Wallet events to the rest of the game. Subscribes to (string, ulong) variant.
    /// </summary>
    public class WalletService : MonoBehaviour
    {
        public static WalletService Instance { get; private set; }

        public event Action<string, ulong> OnWalletChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            var pe = PlayerEconomy.EnsureExists();
            pe.Wallet.OnChanged += HandleWalletChanged;
        }

        private void OnDestroy()
        {
            if (PlayerEconomy.Instance != null && PlayerEconomy.Instance.Wallet != null)
            {
                PlayerEconomy.Instance.Wallet.OnChanged -= HandleWalletChanged;
            }
        }

        private void HandleWalletChanged(string metalKey, ulong total)
        {
            OnWalletChanged?.Invoke(metalKey, total);
        }
    }
}
