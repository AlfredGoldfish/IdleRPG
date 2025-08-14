using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "IdleRPG/GameConfig", fileName = "GameConfig")]
public class GameConfig : ScriptableObject
{
    [SerializeField] private List<CurrencyDef> currencies = new List<CurrencyDef>();

    public IReadOnlyList<CurrencyDef> Currencies => currencies;

    public CurrencyDef GetCurrencyById(string id)
    {
        for (int i = 0; i < currencies.Count; i++)
        {
            if (currencies[i] != null && currencies[i].Id == id)
                return currencies[i];
        }
        return null;
    }
}
