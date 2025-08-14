using UnityEngine;

namespace IdleRPG.Core
{
    /// <summary>
    /// Singleton host for the player's Wallet.
    /// </summary>
    public class PlayerEconomy : MonoBehaviour
    {
        public static PlayerEconomy Instance { get; private set; }
        public Wallet Wallet { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (Wallet == null) Wallet = new Wallet();
        }

        /// <summary>Ensure there is a PlayerEconomy in the scene and return it.</summary>
        public static PlayerEconomy EnsureExists()
        {
            if (Instance != null) return Instance;
            var go = new GameObject("_PlayerEconomy");
            var pe = go.AddComponent<PlayerEconomy>();
            return pe;
        }

        // Convenience pass-throughs for compatibility
        public void AddCurrency(Metal m, ulong delta) => Wallet?.Add(m, delta);
        public void AddCurrency(string key, ulong delta) => Wallet?.Add(key, delta);
    }
}
