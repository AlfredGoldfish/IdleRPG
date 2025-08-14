using System;
using System.Collections.Generic;
using System.Globalization;

namespace IdleRPG.Core
{
    /// <summary>
    /// Compatibility-first wallet: non-negative currency stored as ulong.
    /// Supports both Metal enum and string keys. Fires (string, ulong) OnChanged.
    /// </summary>
    public class Wallet
    {
        private readonly Dictionary<string, ulong> _totalsByKey = new Dictionary<string, ulong>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Fired after Add/Set/Clear operations for a given key.</summary>
        public event Action<string, ulong> OnChanged;

        #region --- Helpers: key mapping ---
        private static bool TryKeyFromMetal(Metal metal, out string key)
        {
            key = metal.ToString();
            return true;
        }

        private static bool TryMetalFromKey(string key, out Metal metal)
        {
            metal = Metal.Copper;
            if (string.IsNullOrWhiteSpace(key)) return false;
            return Enum.TryParse(key, true, out metal);
        }
        #endregion

        #region --- Get ---
        public ulong Get(string metalKey)
        {
            if (string.IsNullOrWhiteSpace(metalKey)) return 0UL;
            return _totalsByKey.TryGetValue(metalKey, out var v) ? v : 0UL;
        }

        public ulong Get(Metal metal)
        {
            TryKeyFromMetal(metal, out var key);
            return Get(key);
        }

        // Legacy signed shim
        public long GetSigned(Metal metal) => unchecked((long)Math.Min(Get(metal), (ulong)long.MaxValue));
        public long GetSigned(string metalKey) => unchecked((long)Math.Min(Get(metalKey), (ulong)long.MaxValue));
        #endregion

        #region --- Set ---
        public void Set(string metalKey, ulong total)
        {
            if (string.IsNullOrWhiteSpace(metalKey)) return;
            _totalsByKey[metalKey] = total;
            OnChanged?.Invoke(metalKey, total);
        }

        public void Set(Metal metal, ulong total)
        {
            TryKeyFromMetal(metal, out var key);
            Set(key, total);
        }

        // Legacy signed shims
        public void Set(Metal metal, long total) => Set(metal, total <= 0 ? 0UL : (ulong)total);
        public void Set(string metalKey, long total) => Set(metalKey, total <= 0 ? 0UL : (ulong)total);
        #endregion

        #region --- Add ---
        public void Add(string metalKey, ulong delta)
        {
            if (string.IsNullOrWhiteSpace(metalKey) || delta == 0UL) return;
            var now = Get(metalKey);
            unchecked
            {
                var next = now + delta;
                if (next < now) next = ulong.MaxValue; // saturate on overflow
                _totalsByKey[metalKey] = next;
            }
            OnChanged?.Invoke(metalKey, _totalsByKey[metalKey]);
        }

        public void Add(Metal metal, ulong delta)
        {
            TryKeyFromMetal(metal, out var key);
            Add(key, delta);
        }

        // Legacy signed shims
        public void Add(Metal metal, long delta)
        {
            if (delta <= 0) return;
            Add(metal, (ulong)delta);
        }

        public void Add(string metalKey, long delta)
        {
            if (delta <= 0) return;
            Add(metalKey, (ulong)delta);
        }
        #endregion

        #region --- Enumerate & Clear ---
        public IEnumerable<KeyValuePair<string, ulong>> Enumerate()
        {
            foreach (var kv in _totalsByKey)
                yield return kv;
        }

        public void ClearAll()
        {
            var keys = new List<string>(_totalsByKey.Keys);
            _totalsByKey.Clear();
            foreach (var k in keys)
            {
                OnChanged?.Invoke(k, 0UL);
            }
        }
        #endregion
    }
}
