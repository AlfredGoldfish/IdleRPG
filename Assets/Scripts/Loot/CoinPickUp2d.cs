using UnityEngine;
using IdleRPG.Core; // for Metal

namespace IdleRPG.Loot
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class CoinPickup2D : MonoBehaviour
    {
        [Header("Coin")]
        public Metal metal = Metal.Copper;
        [Min(0)] public int value = 1;
        [Tooltip("Seconds before auto-despawn (0 = never)")]
        public float lifeSeconds = 20f;

        [Header("Dev")]
        [Tooltip("Allow click-to-collect for quick testing")]
        public bool clickPickupForTests = true;

        Rigidbody2D rb;
        Collider2D col;
        SpriteRenderer sr;
        bool collected;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            sr = GetComponent<SpriteRenderer>();

            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;
                if (rb.linearDamping < 0.1f) rb.linearDamping = 1.0f;
            }
            if (col != null) col.isTrigger = true;
        }

        void OnEnable()
        {
            if (lifeSeconds > 0f) Invoke(nameof(Despawn), lifeSeconds);
        }

        void OnDisable()
        {
            CancelInvoke();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (collected) return;

            if (other.CompareTag("Collector"))
            {
                Collect();
                return;
            }

            if (other.GetComponentInParent<PlayerEconomy>() != null)
            {
                Collect();
            }
        }

        void OnMouseDown()
        {
            if (clickPickupForTests && !collected) Collect();
        }

        public void Initialize(Metal m, int v)
        {
            metal = m;
            value = Mathf.Max(0, v);
        }

        void Collect()
        {
            collected = true;

            var pe = PlayerEconomy.Instance;
            if (pe != null)
            {
                ulong amt = (ulong)Mathf.Max(0, value);
                pe.AddCurrency(metal, amt);
            }

            if (col) col.enabled = false;
            if (sr) sr.enabled = false;

            Destroy(gameObject);
        }

        void Despawn()
        {
            if (!collected) Destroy(gameObject);
        }
    }
}
