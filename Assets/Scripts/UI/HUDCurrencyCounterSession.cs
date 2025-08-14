using UnityEngine;
using UnityEngine.UI;
using IdleRPG.Core;

public class HUDCurrencyCounterSession : MonoBehaviour
{
    [Header("Target Metal")]
    public Metal metal = Metal.Copper;

    [Header("UI")]
    [SerializeField] private Text label;

    private void Awake()
    {
        if (label == null) label = GetComponentInChildren<Text>();
    }

    private void OnEnable()
    {
        var pe = PlayerEconomy.EnsureExists();
        if (pe?.Wallet != null)
        {
            // subscribe to string,ulong event
            pe.Wallet.OnChanged += HandleWalletChanged;
            // prime the label
            HandleWalletChanged(metal.ToString(), pe.Wallet.Get(metal));
        }
    }

    private void OnDisable()
    {
        if (PlayerEconomy.Instance?.Wallet != null)
        {
            PlayerEconomy.Instance.Wallet.OnChanged -= HandleWalletChanged;
        }
    }

    private void HandleWalletChanged(string metalKey, ulong total)
    {
        if (!string.Equals(metalKey, metal.ToString(), System.StringComparison.OrdinalIgnoreCase)) return;
        if (label != null) label.text = total.ToString();
    }
}
