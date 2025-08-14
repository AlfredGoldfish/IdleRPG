using System.Collections.Generic;
using UnityEngine;

namespace IdleRPG.Data
{
    [CreateAssetMenu(menuName = "IdleRPG/GameConfig", fileName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public List<CurrencyDef> currencies = new List<CurrencyDef>();
        public CurrencyDef defaultCurrency;

#if UNITY_EDITOR
        private void OnValidate()
        {
            var seen = new System.Collections.Generic.HashSet<string>();
            foreach (var c in currencies)
            {
                if (!c) continue;
                if (string.IsNullOrEmpty(c.id))
                    c.id = System.Guid.NewGuid().ToString("N");
                if (seen.Contains(c.id))
                    Debug.LogWarning($"[GameConfig] Duplicate currency ID detected: {c.name} ({c.id})");
                seen.Add(c.id);
            }
        }
#endif
    }
}
