using UnityEngine;

[RequireComponent(typeof(Health2D))]
public class DamagePopupOnHealth2D : MonoBehaviour
{
    [Header("Popup Prefab")]
    [SerializeField] DamagePopup3D popupPrefab;

    [Header("Anchor (explicit)")]
    [Tooltip("World Transform above the enemy (child of this prefab). Do NOT assign UI/Canvas objects.")]
    [SerializeField] Transform anchor;

    [Tooltip("If true and no valid anchor is assigned, we log an error and skip spawning.")]
    [SerializeField] bool requireAnchor = false;

    [Tooltip("Parent under anchor so it follows movement. (Ignored if anchor is UI)")]
    [SerializeField] bool parentToAnchor = false;   // safer default

    [Header("Placement Controls")]
    [Tooltip("Extra offset from the anchor/top (X=right/left, Y=up/down).")]
    [SerializeField] Vector2 spawnOffset = new Vector2(0f, 0.15f);

    [Tooltip("Small random offset per popup (X,Y).")]
    [SerializeField] Vector2 microJitter = new Vector2(0.08f, 0.00f);

    [Tooltip("Apply spawnOffset in the anchor's local axes (right/up). If false, uses world axes.")]
    [SerializeField] bool useLocalAxes = true;

    [Tooltip("Depth nudge toward camera. Use 0 for ortho cameras and rely on sorting order.")]
    [SerializeField] float zOffset = 0f;

    Health2D health;

    void Awake()
    {
        health = GetComponent<Health2D>();

        // Reject UI anchors (RectTransform under non-WorldSpace Canvas)
        if (anchor)
        {
            var rect = anchor.GetComponent<RectTransform>();
            var canvas = anchor.GetComponentInParent<Canvas>();
            if (rect && canvas && canvas.renderMode != RenderMode.WorldSpace)
            {
                Debug.LogWarning($"[{name}] Anchor '{anchor.name}' is UI ({canvas.renderMode}); using position-only and ignoring parenting.");
                parentToAnchor = false;
            }

            // Warn if the anchor isn't inside this prefab hierarchy (scene object assigned by mistake)
            if (!anchor.IsChildOf(transform))
            {
                Debug.LogWarning($"[{name}] Anchor '{anchor.name}' is not a child of this prefab. Assign a local child (e.g., 'PopupAnchor').");
            }
        }
        else if (requireAnchor)
        {
            Debug.LogError($"[{name}] No anchor assigned and requireAnchor is true; popups will be skipped.");
        }
    }

    void OnEnable() { if (health) health.OnDamaged += HandleDamaged; }
    void OnDisable() { if (health) health.OnDamaged -= HandleDamaged; }

    void HandleDamaged(int amount)
    {
        if (amount <= 0 || !popupPrefab) return;

        // Enforce explicit anchor if requested
        if (requireAnchor && !IsValidWorldAnchor(anchor)) return;

        var popup = SpawnPopup();
        if (!popup) return;

        if (popup.text == null) popup.text = popup.GetComponentInChildren<TMPro.TMP_Text>();
        if (popup.text) popup.text.text = amount.ToString();
    }

    DamagePopup3D SpawnPopup()
    {
        var jx = microJitter.x == 0 ? 0 : Random.Range(-microJitter.x, microJitter.x);
        var jy = microJitter.y == 0 ? 0 : Random.Range(-microJitter.y, microJitter.y);

        if (IsValidWorldAnchor(anchor) && parentToAnchor)
        {
            var p = Instantiate(popupPrefab, anchor);
            p.transform.localPosition = new Vector3(spawnOffset.x + jx, spawnOffset.y + jy, zOffset);
            p.transform.localRotation = Quaternion.identity;
            p.transform.localScale = Vector3.one;
            return p;
        }
        else
        {
            Vector3 basePos = anchor ? anchor.position : WorldFromBoundsTop();

            if (anchor && useLocalAxes)
            {
                basePos += (Vector3)(anchor.right * spawnOffset.x + anchor.up * spawnOffset.y);
            }
            else
            {
                basePos.x += spawnOffset.x;
                basePos.y += spawnOffset.y;
            }

            basePos.z += zOffset;
            return Instantiate(popupPrefab, basePos, Quaternion.identity);
        }
    }

    bool IsValidWorldAnchor(Transform t)
    {
        if (!t) return false;
        var rect = t.GetComponent<RectTransform>();
        var canvas = t.GetComponentInParent<Canvas>();
        return !(rect && canvas && canvas.renderMode != RenderMode.WorldSpace);
    }

    Vector3 WorldFromBoundsTop()
    {
        if (TryGetWorldBounds(out var b))
            return new Vector3(b.center.x, b.max.y, transform.position.z);
        return transform.position;
    }

    bool TryGetWorldBounds(out Bounds result)
    {
        result = default; bool has = false;
        var cols = GetComponentsInChildren<Collider2D>(true);
        foreach (var c in cols) { if (!c.enabled) continue; if (!has) { result = c.bounds; has = true; } else result.Encapsulate(c.bounds); }
        if (has) return true;
        var rends = GetComponentsInChildren<Renderer>(true);
        foreach (var r in rends) { if (!r.enabled) continue; if (!has) { result = r.bounds; has = true; } else result.Encapsulate(r.bounds); }
        return has;
    }

    [ContextMenu("Spawn Test Popup Here")]
    void DebugSpawnTest()
    {
        var p = SpawnPopup();
        if (p && p.text) p.text.text = "TEST";
        Debug.Log($"[{name}] Test popup at {(IsValidWorldAnchor(anchor) && parentToAnchor ? "anchor(local)" : "world")}, offset {spawnOffset}.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pos = anchor ? anchor.position : WorldFromBoundsTop();
        if (anchor && useLocalAxes)
            pos += (Vector3)(anchor.right * spawnOffset.x + anchor.up * spawnOffset.y);
        else
        {
            pos.x += spawnOffset.x; pos.y += spawnOffset.y;
        }
        Gizmos.DrawWireSphere(pos, 0.06f);
    }
}
