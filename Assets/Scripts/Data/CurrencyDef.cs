using UnityEngine;

[CreateAssetMenu(menuName = "IdleRPG/CurrencyDef", fileName = "NewCurrencyDef")]
public class CurrencyDef : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;
    [SerializeField] private Color uiColor = Color.white;

    public string Id => id;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public Color UIColor => uiColor;
}
