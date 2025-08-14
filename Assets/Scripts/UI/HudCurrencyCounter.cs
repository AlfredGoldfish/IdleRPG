using UnityEngine;
using UnityEngine.UI;
using IdleRPG.Core; // access PlayerEconomy

public class HudCurrencyCounter : MonoBehaviour
{
    [SerializeField] private Text label;
    [SerializeField] private Metal metal; // still in use for inspector selection

    private void OnEnable()
    {
        if (PlayerEconomy.Instance != null && PlayerEconomy.Instance.Wallet != null)
        {
            // Subscribe to both events for compatibility
            PlayerEconomy.Instance.Wallet.OnChanged += OnWalletChanged;
            PlayerEconomy.Instance.Wallet.OnChangedMetal += OnWalletChanged;
        }
    }

    private void OnDisable()
    {
        if (PlayerEconomy.Instance != null && PlayerEconomy.Instance.Wallet != null)
        {
            PlayerEconomy.Instance.Wallet.OnChanged -= OnWalletChanged;
            PlayerEconomy.Instance.Wallet.OnChangedMetal -= OnWalletChanged;
        }
    }

    // New signature for Wallet.OnChanged (string, ulong)
    private void OnWalletChanged(string metalKey, ulong total)
    {
        if (metalKey.Equals(metal.ToString(), System.StringComparison.OrdinalIgnoreCase))
        {
            label.text = total.ToString();
        }
    }

    // Corrected signature for Wallet.OnChangedMetal (Metal, ulong)
    private void OnWalletChanged(Metal metalType, ulong total)
    {
        if (metalType == metal)
        {
            label.text = total.ToString();
        }
    }
}
