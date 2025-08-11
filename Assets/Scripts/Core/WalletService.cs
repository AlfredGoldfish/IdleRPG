using System;
using UnityEngine;

namespace IdleRPG.Core
{
    /// <summary>
    /// Scene-facing wrapper that owns the single Wallet instance.
    /// Keeps Wallet as a plain class; exposes pass-through API + event.
    /// </summary>
    public class WalletService : MonoBehaviour
    {
        [SerializeField] private Wallet wallet = new Wallet();   // POCO instance

        public Wallet Data => wallet;

        // Event passthrough
        public event Action<Metal, ulong> OnChanged
        {
            add { wallet.OnChanged += value; }
            remove { wallet.OnChanged -= value; }
        }

        // Convenience passthroughs
        public ulong Get(Metal m) => wallet.Get(m);
        public void Add(Metal m, ulong amt) => wallet.Add(m, amt);
        public void Set(Metal m, ulong amt) => wallet.Set(m, amt);
        public void ClearAll() => wallet.ClearAll();
    }
}
