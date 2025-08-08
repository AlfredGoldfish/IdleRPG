using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Updates the on-screen player HP bar.
/// Attach this to the PlayerBar panel in your Canvas.
/// </summary>
public class PlayerHealthBar : MonoBehaviour
{
    public static PlayerHealthBar Instance { get; private set; }

    [SerializeField] Image fill;
    [SerializeField] TMP_Text valueText;       // optional “10 / 20”

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>Refresh bar & numeric label from a Player reference.</summary>
    public void Refresh(Player p)
    {
        float pct = (float)p.CurrentHealth / p.maxHealth;
        fill.fillAmount = pct;

        if (valueText)
            valueText.text = $"{p.CurrentHealth}/{p.maxHealth}";
    }
}
