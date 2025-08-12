using UnityEngine;

namespace IdleRPG.Loot
{
    /// <summary>
    /// Centralized drop math for coin count & value.
    /// Inspector-driven (no ScriptableObjects). Place one in scene (e.g., on _Systems).
    /// CoinDropper2D can query this to avoid per-prefab tuning.
    /// </summary>
    [DisallowMultipleComponent]
    public class LootFormula : MonoBehaviour
    {
        [Header("Baselines (Stage 1)")]
        [Min(0)] public int baseMinCoins = 1;
        [Min(0)] public int baseMaxCoins = 3;
        [Min(0)] public int baseMinValue = 1;
        [Min(0)] public int baseMaxValue = 2;

        [Header("Stage Scaling (X = stage index, starting at 1)")]
        [Tooltip("Multiplies coin COUNT based on stage. X=stage (1..N), Y=multiplier.")]
        public AnimationCurve stageCountCurve = AnimationCurve.Linear(1f, 1f, 10f, 3f);

        [Tooltip("Multiplies coin VALUE based on stage. X=stage (1..N), Y=multiplier.")]
        public AnimationCurve stageValueCurve = AnimationCurve.Linear(1f, 1f, 10f, 2f);

        [Header("Tier Multipliers")]
        [Tooltip("Multiplier applied to coin COUNT per enemy tier.")]
        public float trashCountMul = 1f;
        public float eliteCountMul = 1.5f;
        public float bossCountMul  = 3f;

        [Tooltip("Multiplier applied to coin VALUE per enemy tier.")]
        public float trashValueMul = 1f;
        public float eliteValueMul = 1.25f;
        public float bossValueMul  = 2f;

        [Header("Clamps")]
        [Tooltip("Global max clamp for coins per drop after scaling.")]
        public int maxCoinsClamp = 50;
        [Tooltip("Global max clamp for value per coin after scaling.")]
        public int maxValueClamp = 1000000;

        public LootRoll Evaluate(int stage, EnemyTier tier)
        {
            if (stage < 1) stage = 1;

            float countMul = Mathf.Max(0f, stageCountCurve.Evaluate(stage)) * GetTierCountMul(tier);
            float valueMul = Mathf.Max(0f, stageValueCurve.Evaluate(stage)) * GetTierValueMul(tier);

            int minCoins = Mathf.RoundToInt(baseMinCoins * countMul);
            int maxCoins = Mathf.RoundToInt(baseMaxCoins * countMul);
            int minValue = Mathf.RoundToInt(baseMinValue * valueMul);
            int maxValue = Mathf.RoundToInt(baseMaxValue * valueMul);

            // Defensive clamps & order
            minCoins = Mathf.Clamp(minCoins, 0, maxCoinsClamp);
            maxCoins = Mathf.Clamp(Mathf.Max(minCoins, maxCoins), 0, maxCoinsClamp);

            minValue = Mathf.Clamp(minValue, 0, maxValueClamp);
            maxValue = Mathf.Clamp(Mathf.Max(minValue, maxValue), 0, maxValueClamp);

            return new LootRoll
            {
                minCoins = minCoins,
                maxCoins = maxCoins,
                minValue = minValue,
                maxValue = maxValue
            };
        }

        private float GetTierCountMul(EnemyTier t)
        {
            switch (t)
            {
                case EnemyTier.Elite: return eliteCountMul;
                case EnemyTier.Boss:  return bossCountMul;
                default:              return trashCountMul;
            }
        }

        private float GetTierValueMul(EnemyTier t)
        {
            switch (t)
            {
                case EnemyTier.Elite: return eliteValueMul;
                case EnemyTier.Boss:  return bossValueMul;
                default:              return trashValueMul;
            }
        }
    }

    public enum EnemyTier { Trash, Elite, Boss }

    public struct LootRoll
    {
        public int minCoins, maxCoins;
        public int minValue, maxValue;
    }
}
