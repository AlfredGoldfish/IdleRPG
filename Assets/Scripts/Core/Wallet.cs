using System;
using System.Collections.Generic;

namespace IdleRPG.Core
{
    /// <summary>
    /// Minimal wallet used by services/UI. Stores totals per Metal and raises OnChanged on mutation.
    /// Added string/ulong overloads for compatibility with existing code.
    /// </summary>
    public class Wallet
    {
        private readonly Dictionary<Metal, long> _totals = new Dictionary<Metal, long>();

        /// <summary>Raised after any Add/Set/Clear operation. Args: (metal, newTotal)</summary>
        public event Action<Metal, long> OnChanged;

        public long Get(Metal metal)
        {
            return _totals.TryGetValue(metal, out var v) ? v : 0L;
        }

        public void Set(Metal metal, long total)
        {
            if (total < 0) total = 0;
            _totals[metal] = total;
            OnChanged?.Invoke(metal, total);
        }

        public void Add(Metal metal, long delta)
        {
            if (delta == 0) return;
            var now = Get(metal) + delta;
            if (now < 0) now = 0;
            _totals[metal] = now;
            OnChanged?.Invoke(metal, now);
        }

        public void ClearAll()
        {
            // Capture keys to fire events consistently
            var keys = new List<Metal>(_totals.Keys);
            _totals.Clear();
            foreach (var m in keys)
            {
                OnChanged?.Invoke(m, 0L);
            }
        }

        // --- Compatibility overloads (string + ulong) ---

        public long Get(string metalKey)
        {
            if (TryParseMetal(metalKey, out var m)) return Get(m);
            return 0L;
        }

        public void Set(string metalKey, ulong total)
        {
            if (!TryParseMetal(metalKey, out var m)) return;
            long cast = total > long.MaxValue ? long.MaxValue : (long)total;
            Set(m, cast);
        }

        public void Add(string metalKey, ulong delta)
        {
            if (!TryParseMetal(metalKey, out var m)) return;
            long cast = delta > long.MaxValue ? long.MaxValue : (long)delta;
            Add(m, cast);
        }

        private static bool TryParseMetal(string key, out Metal metal)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                metal = default;
                return false;
            }
            return Enum.TryParse(key, true, out metal);
        }
    }
}
