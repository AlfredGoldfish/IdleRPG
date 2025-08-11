using UnityEngine;

public class CoinPickup2D : MonoBehaviour
{
    public Metal metal = Metal.Copper;
    public int value = 1;
    public float lifeSeconds = 20f;

    Rigidbody2D rb;
    Collider2D col;
    SpriteRenderer sr;
    bool collected;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
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
        if (other.CompareTag("Player") || other.CompareTag("Collector") || other.GetComponent<Player>() != null)
            Collect();
    }

    void OnMouseDown()
    {
        if (!collected) Collect();
    }

    public void Initialize(Metal m, int v)
    {
        metal = m;
        value = v < 0 ? 0 : v;
    }

    void Collect()
    {
        collected = true;
        if (PlayerEconomy.Instance != null)
            PlayerEconomy.Instance.AddCurrency(metal, (ulong)(value < 0 ? 0 : value));

        if (col) col.enabled = false;
        if (sr) sr.enabled = false;

        Destroy(gameObject);
    }

    void Despawn()
    {
        if (!collected) Destroy(gameObject);
    }
}