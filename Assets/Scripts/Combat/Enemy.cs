using UnityEngine;
using System.Collections;

/// <summary>
/// Enemy owns the global HUD bar, auto-attacks the player once per second,
/// spawns MeshRenderer damage numbers, and supports crits.
/// </summary>
public class Enemy : MonoBehaviour
{
    /* ───── Designer Tweaks ───── */
    [Header("Combat")]
    [Range(0f, 1f)] public float critChance = 0.20f;
    public float critMultiplier = 2f;
    [SerializeField] int attackDamage = 1;

    [Header("Damage-Popup")]
    public GameObject damagePopup3DPrefab;   // drag DamageText3D prefab
    public float popupBaseHeight = 1.5f;     // baseline Y above enemy centre
    public float popupJitter = 0.30f;    // ±X randomisation

    [Header("Meta")]
    public string displayName = "Monster";

    /* ───── Runtime ───── */
    public EnemySpawner spawner;

    public int maxHealth = 10;
    int currentHealth;
    bool isDying;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    /* ─────────────────── */
    void Start()
    {
        currentHealth = maxHealth;

        // Claim the HUD bar
        HUDHealthBar.Instance.Attach(this);

        // Begin auto-attack loop
        StartCoroutine(AutoAttack());
    }

    /* ─── Manual click test ─── */
    void OnMouseDown() => TakeDamage(1);

    /* ─── Damage intake ─── */
    public void TakeDamage(int baseDamage)
    {
        bool isCrit = Random.value < critChance;
        int finalDamage = isCrit ? Mathf.RoundToInt(baseDamage * critMultiplier)
                                  : baseDamage;

        currentHealth -= finalDamage;
        SpawnDamagePopup(finalDamage, isCrit);

        if (currentHealth <= 0) Die();
    }

    /* ─── Pop-up numbers ─── */
    void SpawnDamagePopup(int dmg, bool isCrit)
    {
        if (!damagePopup3DPrefab) return;

        Vector3 worldPos = transform.position +
                           new Vector3(Random.Range(-popupJitter, popupJitter),
                                       popupBaseHeight,
                                       0f);

        var popObj = Instantiate(damagePopup3DPrefab, worldPos, Quaternion.identity);
        var popScr = popObj.GetComponent<DamagePopup3D>();
        if (popScr) popScr.Init(dmg, isCrit);
    }

    /* ─── Auto-attack coroutine ─── */
    IEnumerator AutoAttack()
    {
        Player player = Object.FindFirstObjectByType<Player>();   // Unity-6 friendly
        while (!isDying && player != null)
        {
            player.TakeDamage(attackDamage);
            yield return new WaitForSeconds(1f);   // one hit per second
        }
    }

    /* ─── Death ─── */
    void Die()
    {
        if (isDying) return;
        isDying = true;

        HUDHealthBar.Instance.Detach(this);
        if (spawner) spawner.OnEnemyDefeated();

        Destroy(gameObject);
    }
}
