using System.Collections;
using UnityEngine;

namespace IdleRPG.Core
{
    [DefaultExecutionOrder(-200)]
    public class SystemsBootstrap : MonoBehaviour
    {
        [Header("Behavior")]
        [SerializeField] private bool loadOnStart = true;
        [SerializeField] private bool saveOnQuit = true;
        [SerializeField] private bool autosaveOnChange = true;
        [SerializeField] private float autosaveCooldown = 0.5f;

        [Header("File")]
        [SerializeField] private string fileName = "wallet.json";

        private EconomyService _eco;
        private float _nextWriteTime;
        private bool _hooked;

        private void Start() => StartCoroutine(TryHookupRoutine());

        private IEnumerator TryHookupRoutine()
        {
            for (int i = 0; i < 60 && !_hooked; i++)
            {
                TryHookup();
                if (_hooked) break;
                yield return null;
            }
        }

        private void TryHookup()
        {
            if (_hooked) return;
            _eco = EconomyService.Instance ?? Object.FindAnyObjectByType<EconomyService>();
            if (_eco == null) return;

            if (loadOnStart) SaveManager.LoadInto(_eco.Wallet, fileName);
            if (autosaveOnChange) _eco.Wallet.OnChanged += OnWalletChanged;
            _hooked = true;
        }

        private void OnDisable()
        {
            if (_hooked && autosaveOnChange && _eco != null)
                _eco.Wallet.OnChanged -= OnWalletChanged;
        }

        private void OnApplicationPause(bool pause) { if (pause) SaveNow(); }
        private void OnApplicationQuit() { if (saveOnQuit) SaveNow(); }
        private void OnDestroy() { if (saveOnQuit) SaveNow(); }

        private void OnWalletChanged(string _, ulong __)
        {
            if (Time.unscaledTime < _nextWriteTime) return;
            _nextWriteTime = Time.unscaledTime + autosaveCooldown;
            SaveNow();
        }

        [ContextMenu("Save Now")]
        public void SaveNow()
        {
            if (_eco == null) return;
            SaveManager.Save(_eco.Wallet, fileName);
        }

        [ContextMenu("Reset Save")]
        public void ResetSave()
        {
            if (_eco == null) return;
            SaveManager.ResetAndSave(_eco.Wallet, fileName);
        }
    }
}
