using UnityEngine;
using IdleRPG.Core;
using IdleRPG.Loot;
namespace IdleRPG.Loot;
/// Spawns a burst of coin pickups when the attached Health2D dies.
/// Inspector-driven: no Update, no JSON, minimal allocations.
[RequireComponent(typeof(Health2D))]

public class CoinDropper2D : MonoBehaviour
{
    [Header("Coin Prefab")]
    [Tooltip("A prefab that has CoinPickup2D + Rigidbody2D + Collider2D(isTrigger).")]
    [SerializeField] private CoinPickup2D coinPrefab;

    [Header("How many coins?")]
    [Min(0)] public int minCoins = 2;
    [Min(0)] public int maxCoins = 5;

    [Header("Value per coin")]
    [Min(0)] public int minValue = 1;
    [Min(0)] public int maxValue = 2;

    [Header("Spawn placement")]
    [Tooltip("Coins spawn inside this radius around the enemy (world units).")]
    [Min(0f)] public float spawnRadius = 0.6f;

    [Tooltip("Extra upward nudge added to the random direction (helps side-view).")]
    [Range(0f, 1f)] public float upwardBias = 0.25f;

    [Header("Burst physics")]
    [Tooltip("Impulse force applied to each coin (world units per second).")]
    [Min(0f)] public float impulse = 2.5f;

    [Tooltip("Random torque magnitude applied to each coin.")]
    [Min(0f)] public float torque = 10f;

    [Header("Lifetime")]
    [Tooltip("Override CoinPickup2D.lifeSeconds? 0 = use prefab's value.")]
    [Min(0f)] public float lifeSecondsOverride = 0f;

    Health2D health;

    void Awake()
    {
        health = GetComponent<Health2D>();
        if (!coinPrefab)
            Debug.LogWarning($"[{name}] CoinDropper2D: coinPrefab not assigned.");
    }

    void OnEnable() { if (health != null) health.OnDied += HandleDeath; }
    void OnDisable() { if (health != null) health.OnDied -= HandleDeath; }

    void HandleDeath()
    {
        if (!coinPrefab) return;

        // Clamp ranges defensively
        int count = Random.Range(Mathf.Min(minCoins, maxCoins), Mathf.Max(minCoins, maxCoins) + 1);
        int vmin = Mathf.Min(minValue, maxValue);
        int vmax = Mathf.Max(minValue, maxValue);

        var origin = transform.position;

        for (int i = 0; i < count; i++)
        {
            // Random point inside a circle on XY plane
            Vector2 offset2D = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = origin + new Vector3(offset2D.x, offset2D.y, 0f);

            // Spawn coin
            var coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            int value = Random.Range(vmin, vmax + 1);
            coin.Initialize(coin.metal, value); // metal comes from the prefab; we just set value

            if (lifeSecondsOverride > 0f) coin.lifeSeconds = lifeSecondsOverride;

            // Physics burst: random direction + upward bias
            var rb = coin.GetComponent<Rigidbody2D>();
            if (rb)
            {
                Vector2 dir = offset2D.normalized;
                // Add a bit of upward bias (for side-view games)
                dir = (dir + Vector2.up * upwardBias).normalized;

                rb.AddForce(dir * impulse, ForceMode2D.Impulse);

                if (torque > 0f)
                {
                    float t = Random.Range(-torque, torque);
                    rb.AddTorque(t, ForceMode2D.Impulse);
                }
            }
            else
            {
                Debug.LogWarning($"[{name}] CoinDropper2D: spawned coin has no Rigidbody2D.");
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.85f, 0.2f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
#endif
}
