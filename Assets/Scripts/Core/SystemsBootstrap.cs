using UnityEngine;
using IdleRPG.Core; // Added to access PlayerEconomy

public class SystemsBootstrap : MonoBehaviour
{
    private void Start()
    {
        // Ensure PlayerEconomy exists
        var pe = PlayerEconomy.EnsureExists();
        var wallet = pe.Wallet;

        // Explicit casts from ulong to long
        long copper = (long)wallet.Get("Copper");
        long silver = (long)wallet.Get("Silver");
        long gold = (long)wallet.Get("Gold");
        long platinum = (long)wallet.Get("Platinum");

        Debug.Log($"Wallet Totals - Cu:{copper} Ag:{silver} Au:{gold} Pt:{platinum}");
    }
}
