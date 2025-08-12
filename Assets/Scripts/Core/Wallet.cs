using System;
using System.Collections.Generic;

namespace IdleRPG.Core
{
    /// Metals/currencies you support. Extend as needed.
    public enum Metal
    {
        Copper, Bronze, Silver, Gold, Platinum, Mithril
    }

    /// Plain data model for your currency balances.
    /// Not a MonoBehaviour on purpose (simple, testable).
    [Serializable]
    public class Wallet
    {
        private readonly Dictionary<Metal, ulong> balance = new();

        /// Event: fired whenever a metal's total changes.
        /// Args: (metal, newTotal)
        public event Action<Metal, ulong> OnChanged;

        public ulong Get(Metal metal)
            => balance.TryGetValue(metal, out var v) ? v : 0UL;

        public void Add(Metal metal, ulong amount)
        {
            if (amount == 0UL) return;
            var newTotal = Get(metal) + amount;
            balance[metal] = newTotal;
            OnChanged?.Invoke(metal, newTotal);
        }

        /// Set an exact amount (used by loads/tools).
        public void Set(Metal metal, ulong amount)
        {
            if (amount == 0UL) balance.Remove(metal);
            else balance[metal] = amount;
            OnChanged?.Invoke(metal, Get(metal));
        }

        /// Clear all balances to zero and notify listeners.
        public void ClearAll()
        {
            foreach (Metal m in Enum.GetValues(typeof(Metal)))
            {
                if (Get(m) != 0UL)
                    OnChanged?.Invoke(m, 0UL);
            }
            balance.Clear();
        }
    }
}
