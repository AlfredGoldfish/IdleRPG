using UnityEngine;

namespace IdleRPG.Loot
{
    /// <summary>
    /// Gentle coin vacuum. Finds CoinPickup2D within a radius and nudges their Rigidbody2D toward the player.
    /// Uses NonAlloc overlap to avoid GC spikes.
    /// </summary>
    public class MagnetCollector2D : MonoBehaviour
    {
        [Header("Reach & Feel")]
        [SerializeField] private float radius = 3f;
        [SerializeField] private float basePullSpeed = 10f;   // target speed near the edge of radius
        [SerializeField] private float maxCoinSpeed = 20f;    // hard cap
        [SerializeField] private float accel = 50f;           // how quickly coins approach target speed

        [Header("Filtering")]
        [SerializeField] private LayerMask coinLayer;         // set to your Coin/Pickup layer

        [Header("Center Override")]
        [SerializeField] private Transform centerOverride;    // drag collector_trigger here if magnet lives on player root

        // Reusable buffer to avoid allocations
        private readonly Collider2D[] _hits = new Collider2D[64];

        private void Reset()
        {
            coinLayer = ~0;
        }

        private void FixedUpdate()
        {
            Vector2 me = (centerOverride != null) ? (Vector2)centerOverride.position : (Vector2)transform.position;
            int count = Physics2D.OverlapCircleNonAlloc(me, radius, _hits, coinLayer);

            for (int i = 0; i < count; i++)
            {
                var col = _hits[i];
                if (col == null) continue;
                if (!col.TryGetComponent<CoinPickup2D>(out _)) continue;
                if (!col.TryGetComponent<Rigidbody2D>(out var rb)) continue;

                float d = Vector2.Distance(rb.position, me);
                if (d <= 0.001f) continue;

                float t = Mathf.Clamp01(1f - d / radius); // 0 at edge, 1 near player
                float targetSpeed = Mathf.Lerp(0f, basePullSpeed, t);

                Vector2 dir = (me - rb.position).normalized;
                Vector2 targetVel = dir * targetSpeed;

                rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVel, accel * Time.fixedDeltaTime);
                rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxCoinSpeed);
            }

            for (int i = 0; i < count; i++) _hits[i] = null;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 p = (centerOverride != null) ? centerOverride.position : transform.position;
            Gizmos.color = new Color(1f, 1f, 0f, 0.35f);
            Gizmos.DrawWireSphere(p, radius);
        }
    }
}
