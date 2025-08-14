using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "IdleRPG/GameConfig", fileName = "GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("All Currencies")]
    [SerializeField] private List<CurrencyDef> currencies = new List<CurrencyDef>();

    [Header("Default Currency")]
    public CurrencyDef defaultCurrency; // added for compatibility

    public IReadOnlyList<CurrencyDef> Currencies => currencies;

    public CurrencyDef GetCurrencyById(string id)
    {
        for (int i = 0; i < currencies.Count; i++)
        {
            if (currencies[i] != null && currencies[i].id == id)
                return currencies[i];
        }
        return null;
    }
}
