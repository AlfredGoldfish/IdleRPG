using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleRPG.Data
{
    [CreateAssetMenu(menuName = "IdleRPG/Loot Table")]
    public class LootTable : ScriptableObject
    {
        public List<DropRule> drops = new(); // each rule can award 0..N items independently
    }

    [Serializable]
    public class DropRule
    {
        public ItemDef item;
        [Range(0, 100)] public float chance = 100f;  // % chance to drop
        public int min = 1;
        public int max = 1;
    }

    public static class LootResolver
    {
        // Independent rolls: each rule can drop
        public static List<(ItemDef item, int qty)> RollIndependent(LootTable table, System.Random rng = null)
        {
            rng ??= new System.Random();
            var result = new List<(ItemDef, int)>();
            if (table == null || table.drops == null) return result;

            foreach (var r in table.drops)
            {
                if (r.item == null || r.max <= 0) continue;
                var roll = rng.NextDouble() * 100.0;
                if (roll > r.chance) continue;
                int qty = UnityEngine.Random.Range(r.min, r.max + 1);
                if (qty > 0) result.Add((r.item, qty));
            }
            return result;
        }
    }
}
