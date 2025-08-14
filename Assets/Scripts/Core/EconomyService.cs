using System;
using System.Collections.Generic;
using UnityEngine;
using IdleRPG.Data;

namespace IdleRPG.Core
{
    [DefaultExecutionOrder(-500)]
    public class EconomyService : MonoBehaviour
    {
        public static EconomyService Instance { get; private set; }

        [Header("Config")]
        [SerializeField] private GameConfig gameConfig;

        [Header("Debug")]
        [SerializeField] private bool devHotkeyC_Add1 = true;
        [SerializeField] private CurrencyDef debugCurrencyOverride;

        public Wallet Wallet { get; } = new Wallet();
        public event Action<CurrencyDef, ulong> OnCurrencyChanged;

        private Dictionary<string, CurrencyDef> _byId = new Dictionary<string, CurrencyDef>();

        private void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            RebuildLookup();
            Wallet.OnChanged += HandleWalletChanged;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            Wallet.OnChanged -= HandleWalletChanged;
        }

        private void RebuildLookup()
        {
            _byId.Clear();
            if (!gameConfig) return;
            foreach (var c in gameConfig.currencies)
            {
                if (!c || string.IsNullOrEmpty(c.id)) continue;
                _byId[c.id] = c;
            }
        }

        private void HandleWalletChanged(string currencyId, ulong newTotal)
        {
            if (_byId.TryGetValue(currencyId, out var def))
                OnCurrencyChanged?.Invoke(def, newTotal);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (devHotkeyC_Add1 && Input.GetKeyDown(KeyCode.C))
            {
                var def = debugCurrencyOverride ? debugCurrencyOverride : gameConfig ? gameConfig.defaultCurrency : null;
                if (def) Add(def, 1);
            }
        }
#endif

        public ulong Get(CurrencyDef def) => (!def || string.IsNullOrEmpty(def.id)) ? 0UL : Wallet.Get(def.id);
        public void Add(CurrencyDef def, ulong amount) { if (def && !string.IsNullOrEmpty(def.id) && amount != 0UL) Wallet.Add(def.id, amount); }
        public void Set(CurrencyDef def, ulong amount) { if (def && !string.IsNullOrEmpty(def.id)) Wallet.Set(def.id, amount); }
        public void SetConfig(GameConfig cfg) { gameConfig = cfg; RebuildLookup(); }
    }
}
