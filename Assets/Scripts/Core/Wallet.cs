using System;
using System.Collections.Generic;
using UnityEngine;


public void ClearAll()
{
    // zero out your internal amounts (array/dictionary—whatever you use)
    for (int i = 0; i < _amounts.Length; i++) _amounts[i] = 0;
    OnChanged?.Invoke();
}

/// Tiered metals – extend whenever you add a new currency icon.
public enum Metal
{
    Copper, Bronze, Silver, Gold, Platinum, Mithril
}

/// Holds an unlimited stack for each metal and raises an event on change.
[Serializable]
public class Wallet
{
    readonly Dictionary<Metal, ulong> balance = new();

    public ulong Get(Metal metal) => balance.TryGetValue(metal, out var v) ? v : 0UL;

    public void Add(Metal metal, ulong amount)
    {
        if (amount == 0) return;

        balance[metal] = Get(metal) + amount;
        OnChanged?.Invoke(metal, balance[metal]);
    }

    /// metal → new total
    public event Action<Metal, ulong> OnChanged;
}
