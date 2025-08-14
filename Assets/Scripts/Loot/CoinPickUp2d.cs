using UnityEngine;
using IdleRPG.Core;

public class CoinPickup2D : MonoBehaviour
{
    [Header("Legacy Fields")]
    public Metal metal; // legacy enum
    public float lifeSeconds = 10f; // legacy life span

    [Header("New System")]
    public CurrencyDef currencyDef;
    public ulong amount = 1;

    // Legacy Initialize method
    public void Initialize(Metal m, ulong value)
    {
        metal = m;
        amount = value;
        if (currencyDef == null)
        {
            // Attempt to map Metal to CurrencyDef via GameConfig if available
            var cfg = Resources.Load<GameConfig>("GameConfig");
            if (cfg != null)
            {
                currencyDef = cfg.GetCurrencyById(m.ToString().ToLower());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerEconomy.Instance != null && PlayerEconomy.Instance.Wallet != null)
            {
                if (currencyDef != null)
                    PlayerEconomy.Instance.Wallet.Add(currencyDef, amount);
                else
                    PlayerEconomy.Instance.Wallet.Add(metal, amount);
            }
            Destroy(gameObject);
        }
    }
}
