using UnityEngine;

[CreateAssetMenu(menuName = "IdleRPG/CurrencyDef", fileName = "NewCurrencyDef")]
public class CurrencyDef : ScriptableObject
{
    [Tooltip("Unique ID for this currency (use lowercase, no spaces)")]
    public string id; // made public for compatibility

    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;
    [SerializeField] private Color uiColor = Color.white;

    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public Color UIColor => uiColor;
}
