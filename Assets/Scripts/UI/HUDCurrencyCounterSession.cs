using UnityEngine;
using UnityEngine.UI;
using IdleRPG.Core;

public class HUDCurrencyCounterSession : MonoBehaviour
{
    [SerializeField] private Text label;
    [SerializeField] private Metal metal; // for backward compatibility
    [SerializeField] private CurrencyDef currencyDef; // new SO reference

    private void OnEnable()
    {
        if (PlayerEconomy.Instance != null && PlayerEconomy.Instance.Wallet != null)
        {
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

    private void OnWalletChanged(string id, ulong total)
    {
        if (currencyDef != null && id == currencyDef.Id)
        {
            label.text = total.ToString();
        }
    }

    private void OnWalletChanged(Metal m, ulong total)
    {
        if (m == metal)
        {
            label.text = total.ToString();
        }
    }
}
