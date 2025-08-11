using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleRPG.Core
{
    /// Tiered metals – extend whenever you add a new currency icon.
    public enum Metal
    {
        Copper, Bronze, Silver, Gold, Platinum, Mithril
    }

    /// Holds an unlimited stack for each metal and raises an event on change.
    [Serializable]
    public class Wallet
    {
        readonly Dictionary<Metal, ulong> balance = new();

        public ulong Get(Metal metal) => balance.TryGetValue(metal, out var v) ? v : 0UL;

        public void Add(Metal metal, ulong amount)
        {
            if (amount == 0) return;
            balance[metal] = Get(metal) + amount;
            OnChanged?.Invoke(metal, balance[metal]);
        }

        public void ClearAll()
        {
            foreach (Metal m in Enum.GetValues(typeof(Metal)))
            {
                if (Get(m) != 0UL)
                    OnChanged?.Invoke(m, 0UL);
            }
            balance.Clear();
        }

        // Optional: precise setter for loads/tools
        public void Set(Metal metal, ulong amount)
        {
            if (amount == 0UL) balance.Remove(metal);
            else balance[metal] = amount;
            OnChanged?.Invoke(metal, Get(metal));
        }

        /// metal → new total
        public event Action<Metal, ulong> OnChanged;
    }
}
