using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleRPG.Data
{
    [CreateAssetMenu(menuName = "IdleRPG/Stage")]
    public class StageDef : ScriptableObject
    {
        public string displayName;
        public List<WeightedEnemy> enemies = new();

        public EnemyDef PickRandomEnemy()
        {
            int total = 0;
            foreach (var we in enemies) total += Mathf.Max(0, we.weight);
            if (total <= 0 || enemies.Count == 0) return null;

            int r = UnityEngine.Random.Range(1, total + 1);
            foreach (var we in enemies)
            {
                r -= Mathf.Max(0, we.weight);
                if (r <= 0) return we.enemy;
            }
            return enemies[enemies.Count - 1].enemy; // fallback
        }
    }

    [Serializable]
    public class WeightedEnemy
    {
        public EnemyDef enemy;
        [Min(0)] public int weight = 1;
    }
}
