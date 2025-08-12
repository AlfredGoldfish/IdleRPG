using UnityEngine;

namespace IdleRPG.Loot
{
    [DisallowMultipleComponent]
    public class MagnetCollector2D : MonoBehaviour
    {
        [Header("Reach & Feel")]
        [SerializeField] private float radius = 3f;
        [SerializeField] private float basePullSpeed = 10f;  // target speed near edge of radius
        [SerializeField] private float maxCoinSpeed = 20f;   // hard cap
        [SerializeField] private float accel = 50f;          // how fast coins approach target speed

        [Header("Filtering")]
        [SerializeField] private LayerMask coinLayer = ~0;   // set to your Coin/Pickup layer

        [Header("Center Override")]
        [SerializeField] private Transform centerOverride;   // drag collector_trigger here if magnet is on player root

        // Non-alloc buffer & contact filter
        private readonly Collider2D[] _hits = new Collider2D[64];
        private ContactFilter2D _filter;

        private void Awake() => RefreshFilter();
        private void OnValidate() => RefreshFilter();
        private void Reset()
        {
            coinLayer = ~0;
            RefreshFilter();
        }

        private void RefreshFilter()
        {
            _filter = new ContactFilter2D
            {
                useLayerMask = true,
                useTriggers = true  // coins are triggers
            };
            _filter.SetLayerMask(coinLayer);
        }

        private void FixedUpdate()
        {
            Vector2 center = centerOverride ? (Vector2)centerOverride.position : (Vector2)transform.position;

            // Modern, non-alloc overlap (no obsolete warning)
            int count = Physics2D.OverlapCircle(center, radius, _filter, _hits);

            for (int i = 0; i < count; i++)
            {
                var col = _hits[i];
                _hits[i] = null; // clear as we go

                if (col == null) continue;
                if (!col.TryGetComponent(out CoinPickup2D _)) continue;
                if (!col.TryGetComponent(out Rigidbody2D rb)) continue;

                float d = Vector2.Distance(rb.position, center);
                if (d <= 0.001f) continue;

                float t = Mathf.Clamp01(1f - d / radius);     // 0 at edge, 1 near center
                float targetSpeed = Mathf.Lerp(0f, basePullSpeed, t);

                Vector2 dir = (center - rb.position).normalized;
                Vector2 targetVel = dir * targetSpeed;

                // On newer Unity that exposes linearVelocity, you can swap to it:
                // rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVel, accel * Time.fixedDeltaTime);
                // rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxCoinSpeed);

                rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVel, accel * Time.fixedDeltaTime);
                rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxCoinSpeed);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 p = centerOverride ? centerOverride.position : transform.position;
            Gizmos.color = new Color(1f, 1f, 0f, 0.35f);
            Gizmos.DrawWireSphere(p, radius);
        }
    }
}
