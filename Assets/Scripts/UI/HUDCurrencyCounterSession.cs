using UnityEngine;
using TMPro;
using IdleRPG.Core;

public class HUDCurrencyCounterSession : MonoBehaviour
{
    public string currencyId; // matches WalletService event signature in current repo
    public TextMeshProUGUI label;

    private ulong sessionAmount;
    private WalletService walletService;

    private void Awake()
    {
        // Get WalletService via EconomyService in SystemsBootstrap
        var bootstrap = FindFirstObjectByType<SystemsBootstrap>();
        if (bootstrap != null && bootstrap.EconomyService != null)
        {
            walletService = bootstrap.EconomyService.WalletService;
        }
    }

    private void OnEnable()
    {
        if (walletService != null)
        {
            walletService.OnWalletChanged += HandleWalletChanged;
            sessionAmount = 0;
            UpdateLabel();
        }
    }

    private void OnDisable()
    {
        if (walletService != null)
            walletService.OnWalletChanged -= HandleWalletChanged;
    }

    private void HandleWalletChanged(string changedCurrencyId, ulong newAmount)
    {
        if (changedCurrencyId == currencyId)
        {
            sessionAmount = newAmount;
            UpdateLabel();
        }
    }

    private void UpdateLabel()
    {
        if (label != null)
            label.text = !string.IsNullOrEmpty(currencyId) ? $"{currencyId}: {sessionAmount}" : sessionAmount.ToString();
    }
}
