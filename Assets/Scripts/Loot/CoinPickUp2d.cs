using UnityEngine;
using IdleRPG.Core;

public class CoinPickup2D : MonoBehaviour
{
    [SerializeField] private CurrencyDef currencyDef;
    [SerializeField] private ulong amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerEconomy.Instance != null && PlayerEconomy.Instance.Wallet != null)
            {
                PlayerEconomy.Instance.Wallet.Add(currencyDef, amount);
            }
            Destroy(gameObject);
        }
    }
}
