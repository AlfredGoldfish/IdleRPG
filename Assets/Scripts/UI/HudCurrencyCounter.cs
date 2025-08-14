using UnityEngine;
using UnityEngine.UI;
using IdleRPG.Core; // Added to access PlayerEconomy

public class HudCurrencyCounter : MonoBehaviour
{
    [SerializeField] private Text label;
    [SerializeField] private Metal metal; // still in use for inspector selection

    private void OnEnable()
    {
        if (PlayerEconomy.Instance != null && PlayerEconomy.Instance.Wallet != null)
        {
            // Subscribe to new string/ulong event
            PlayerEconomy.Instance.Wallet.OnChanged += OnWalletChanged;
            // Optional: subscribe to Metal/long if needed
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

    // New signature for Wallet.OnChanged
    private void OnWalletChanged(string metalKey, ulong total)
    {
        if (metalKey.Equals(metal.ToString(), System.StringComparison.OrdinalIgnoreCase))
        {
            label.text = total.ToString();
        }
    }

    // Compatibility overload for Wallet.OnChangedMetal
    private void OnWalletChanged(Metal metalType, long total)
    {
        if (metalType == metal)
        {
            label.text = total.ToString();
        }
    }
}
