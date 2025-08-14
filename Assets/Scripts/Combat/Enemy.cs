using UnityEngine;
using System.Collections;

//
// Enemy: exposes health/displayName for UI, handles click-to-damage,
// auto-attacks player, and notifies spawner on death.
// Health is delegated to Health2D. Currency/loot will be handled by a separate
// CoinDropper2D component in the next step.
//
[RequireComponent(typeof(Health2D))]
public class Enemy : MonoBehaviour
{
    /* ───── Designer Tweaks ───── */
    [Header("Combat")]
    [Range(0f, 1f)] public float critChance = 0.20f;
    public float critMultiplier = 2f;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackInterval = 1f;

    [Header("Click to Damage")]
    [SerializeField] int clickDamage = 1;      // amount dealt when you click

    [Header("Identification / Spawner hook")]
   // [SerializeField] string enemyId = "Slime";  // kept for future use (e.g., Codex)
    public string displayName = "Slime";
    [HideInInspector] public EnemySpawner spawner;

    /* ───── Runtime ───── */
    Health2D health;
    bool isDying;
    Coroutine attackLoop;

    // UI scripts expect these:
    public int CurrentHealth => health ? health.Current : 0;
    public int MaxHealth => health ? health.Max : 1;

    void Awake()
    {
        if (!health) health = GetComponent<Health2D>();
    }

    void OnEnable()
    {
        if (HUDHealthBar.Instance) HUDHealthBar.Instance.Attach(this);

        // react to death from any source (clicks, player skills, etc.)
        health.OnDied += OnDied;

        attackLoop = StartCoroutine(AIAttackLoop());
    }

    void OnDisable()
    {
        if (attackLoop != null) StopCoroutine(attackLoop);
        if (HUDHealthBar.Instance) HUDHealthBar.Instance.Detach(this);

        if (health) health.OnDied -= OnDied;
    }

    IEnumerator AIAttackLoop()
    {
        var wait = new WaitForSeconds(attackInterval);
        while (true)
        {
            yield return wait;
            TryAttackPlayer();
        }
    }

    void TryAttackPlayer()
    {
        var player = Object.FindFirstObjectByType<Player>();
        if (!player) return;

        int dmg = attackDamage;
        bool isCrit = Random.value < critChance;
        if (isCrit) dmg = Mathf.CeilToInt(dmg * critMultiplier);

        player.TakeDamage(dmg);
    }

    // === CLICK HANDLER ===
    void OnMouseDown()
    {
        // Requires a Collider/Collider2D on this GameObject.
        if (!health || isDying) return;
        health.TakeDamage(clickDamage);
        if (health.IsDead) Die();
    }

    // Called by Health2D event
    void OnDied() => Die();

    void Die()
    {
        if (isDying) return;
        isDying = true;

        // ── Loot/Currency will be spawned/handled by CoinDropper2D in the next step ──

        if (HUDHealthBar.Instance) HUDHealthBar.Instance.Detach(this);
        if (spawner) spawner.OnEnemyDefeated();

        Destroy(gameObject);
    }
}
