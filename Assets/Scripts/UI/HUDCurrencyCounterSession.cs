using System.Collections.Generic;
using UnityEngine;
using IdleRPG.Core; // Metal, Wallet

#if TMP_PRESENT || UNITY_EDITOR
using TMPro;
#endif
using UnityEngine.UI;

namespace IdleRPG.UI
{
    /// <summary>
    /// Session-only HUD counter for a single Metal.
    /// Anchor-based: after Reset, we anchor to the current wallet total and display 0.
    /// Display = max(0, newTotal - anchor). Robust to missed events or wallet clears.
    /// </summary>
    [DisallowMultipleComponent]
    public class HUDCurrencyCounterSession : MonoBehaviour
    {
        [SerializeField] private Metal metal = Metal.Copper;
        [SerializeField] private string format = "{0}";

        private PlayerEconomy pe;
        private Wallet wallet;

        private ulong anchorTotal;   // wallet total at the time of last Reset/enable
        private ulong displayTotal;  // computed: max(0, current - anchor)

#if TMP_PRESENT || UNITY_EDITOR
        [SerializeField] private TMP_Text tmpText;
#endif
        [SerializeField] private Text uiText;

        // Registry so a single button can reset all session counters
        private static readonly List<HUDCurrencyCounterSession> _registry = new List<HUDCurrencyCounterSession>();
        public static IReadOnlyList<HUDCurrencyCounterSession> Registry => _registry;

        private void Reset()
        {
#if TMP_PRESENT || UNITY_EDITOR
            if (!tmpText) tmpText = GetComponent<TMP_Text>();
#endif
            if (!uiText) uiText = GetComponent<Text>();
        }

        private void OnEnable()
        {
            _registry.Add(this);

            pe = PlayerEconomy.Instance ?? FindAnyObjectByType<PlayerEconomy>();
            if (pe == null)
            {
                Debug.LogWarning("[HUDCurrencyCounterSession] PlayerEconomy not found. Counter inactive.");
                return;
            }
            wallet = pe.Wallet; // same shared model

            // Anchor to current wallet value and show 0
            anchorTotal = wallet.Get(metal);
            displayTotal = 0;
            RefreshText();

            wallet.OnChanged += HandleWalletChanged;
        }

        private void OnDisable()
        {
            _registry.Remove(this);
            if (wallet != null) wallet.OnChanged -= HandleWalletChanged;
        }

        private void HandleWalletChanged(Metal m, ulong newTotal)
        {
            if (m != metal) return;

            if (newTotal < anchorTotal)
            {
                // Wallet was cleared or reduced; re-anchor
                anchorTotal = newTotal;
                displayTotal = 0;
            }
            else
            {
                displayTotal = newTotal - anchorTotal;
            }
            RefreshText();
        }

        public void ResetSession()
        {
            if (wallet != null) anchorTotal = wallet.Get(metal);
            displayTotal = 0;
            RefreshText();
        }

        private void RefreshText()
        {
            string text = string.Format(format, displayTotal);
#if TMP_PRESENT || UNITY_EDITOR
            if (tmpText) { tmpText.text = text; return; }
#endif
            if (uiText) { uiText.text = text; return; }
        }
    }
}
