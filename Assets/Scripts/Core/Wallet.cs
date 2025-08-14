using System;
using System.Collections.Generic;
using System.Globalization;

namespace IdleRPG.Core
{
    /// <summary>
    /// Wallet with non-negative currency totals, keyed by Metal but addressable by string.
    /// Fires both (string, ulong) and (Metal, ulong) events for compatibility.
    /// </summary>
    public class Wallet
    {
        private readonly Dictionary<Metal, ulong> _totals = new Dictionary<Metal, ulong>();

        // Primary event for new code (string key)
        public event Action<string, ulong> OnChanged;
        // Secondary event to keep older listeners working (Metal key)
        public event Action<Metal, ulong> OnChangedMetal;

        public ulong Get(Metal metal)
        {
            return _totals.TryGetValue(metal, out var v) ? v : 0UL;
        }

        public ulong Get(string metalKey)
        {
            if (TryParseMetal(metalKey, out var m)) return Get(m);
            return 0UL;
        }

        public void Set(Metal metal, ulong total)
        {
            _totals[metal] = total;
            FireChanged(metal, total);
        }

        public void Set(string metalKey, ulong total)
        {
            if (TryParseMetal(metalKey, out var m)) Set(m, total);
        }

        // Legacy signed shims (clamped to >= 0)
        public void Set(Metal metal, long total) => Set(metal, total < 0 ? 0UL : (ulong)total);
        public void Set(string metalKey, long total) => Set(metalKey, total < 0 ? 0UL : (ulong)total);

        public void Add(Metal metal, ulong delta)
        {
            if (delta == 0UL) return;
            var now = Get(metal) + delta; // ulong add
            _totals[metal] = now;
            FireChanged(metal, now);
        }

        public void Add(string metalKey, ulong delta)
        {
            if (TryParseMetal(metalKey, out var m)) Add(m, delta);
        }

        // Legacy signed shims
        public void Add(Metal metal, long delta)
        {
            if (delta == 0) return;
            ulong now = Get(metal);
            if (delta < 0)
            {
                var dec = (ulong)(-delta);
                now = dec > now ? 0UL : now - dec;
            }
            else
            {
                now += (ulong)delta;
            }
            _totals[metal] = now;
            FireChanged(metal, now);
        }
        public void Add(string metalKey, long delta)
        {
            if (TryParseMetal(metalKey, out var m)) Add(m, delta);
        }

        public void ClearAll()
        {
            var keys = new List<Metal>(_totals.Keys);
            _totals.Clear();
            foreach (var m in keys)
            {
                FireChanged(m, 0UL);
            }
        }

        public IEnumerable<KeyValuePair<string, ulong>> Enumerate()
        {
            foreach (var kv in _totals)
                yield return new KeyValuePair<string, ulong>(kv.Key.ToString(), kv.Value);
        }

        private void FireChanged(Metal metal, ulong total)
        {
            OnChanged?.Invoke(metal.ToString(), total);
            OnChangedMetal?.Invoke(metal, total);
        }

        private static bool TryParseMetal(string key, out Metal metal)
        {
            return Enum.TryParse(key, true, out metal);
        }
    }
}
