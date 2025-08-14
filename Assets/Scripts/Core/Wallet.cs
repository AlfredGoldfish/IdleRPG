using System;
using System.Collections.Generic;

namespace IdleRPG.Core
{
    public class Wallet
    {
        private readonly Dictionary<string, ulong> _totals = new Dictionary<string, ulong>();

        public event Action<string, ulong> OnChanged;
        public event Action<Metal, ulong> OnChangedMetal;

        public ulong Get(string id)
        {
            return _totals.TryGetValue(id, out var v) ? v : 0UL;
        }

        public ulong Get(CurrencyDef def) => Get(def.Id);

        public ulong Get(Metal metal) => Get(metal.ToString().ToLower());

        public void Set(string id, ulong total)
        {
            _totals[id] = total;
            OnChanged?.Invoke(id, total);
        }

        public void Set(CurrencyDef def, ulong total) => Set(def.Id, total);

        public void Set(Metal metal, ulong total)
        {
            Set(metal.ToString().ToLower(), total);
            OnChangedMetal?.Invoke(metal, total);
        }

        public void Add(string id, ulong delta)
        {
            var now = Get(id) + delta;
            _totals[id] = now;
            OnChanged?.Invoke(id, now);
        }

        public void Add(CurrencyDef def, ulong delta) => Add(def.Id, delta);

        public void Add(Metal metal, ulong delta)
        {
            Add(metal.ToString().ToLower(), delta);
            OnChangedMetal?.Invoke(metal, Get(metal));
        }
    }
}
