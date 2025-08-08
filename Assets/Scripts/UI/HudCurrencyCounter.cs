using UnityEngine;
using TMPro;

public class HUDCurrencyCounter : MonoBehaviour
{
    [SerializeField] Metal metal = Metal.Copper;
    [SerializeField] TMP_Text amountText;

    void Start()           // ← subscribe here
    {
        PlayerEconomy.Instance.Wallet.OnChanged += OnWalletChanged;
        Refresh();
    }

    void OnDestroy()       // ← always unsubscribe
    {
        if (PlayerEconomy.Instance)
            PlayerEconomy.Instance.Wallet.OnChanged -= OnWalletChanged;
    }

    void OnWalletChanged(Metal m, ulong total)
    {
        if (m == metal) UpdateText(total);
    }

    void Refresh() => UpdateText(PlayerEconomy.Instance.Wallet.Get(metal));

    void UpdateText(ulong amt) => amountText.text = Format(amt);

    static string Format(ulong v)
    {
        if (v >= 1_000_000_000UL) return (v / 1_000_000_000f).ToString("0.#") + " B";
        if (v >= 1_000_000UL) return (v / 1_000_000f).ToString("0.#") + " M";
        if (v >= 1_000UL) return (v / 1_000f).ToString("0.#") + " K";
        return v.ToString();
    }
}
