using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDHealthBar : MonoBehaviour
{
    /* ──  Inspector  ── */
    [Header("UI Refs")]
    public Image fill;       // the foreground image you scale
    public TMP_Text nameText;  // optional monster name

    /* ──  Singleton  ── */
    public static HUDHealthBar Instance { get; private set; }

    /* ──  Runtime  ── */
    private Enemy current;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Attach(Enemy e)
    {
        current = e;
        nameText.text = e.displayName;          // add a string field on Enemy if desired
        UpdateBar();                               // immediate refresh
    }

    public void Detach(Enemy e)
    {
        if (current != e) return;
        current = null;
        fill.fillAmount = 0f;                      // bar stays but empties
        nameText.text = string.Empty;
    }

    void Update()
    {
        if (!current) return;
        UpdateBar();
    }

    private void UpdateBar()
    {
        float pct = (float)current.CurrentHealth / current.MaxHealth;
        fill.fillAmount = pct;
    }
}
