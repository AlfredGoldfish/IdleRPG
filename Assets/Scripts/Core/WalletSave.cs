using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using IdleRPG.Core;

/// <summary>
/// Minimal JSON persistence for the Wallet. Stores parallel lists for metals and amounts.
/// LoadInto uses SET semantics (idempotent).
/// </summary>
public static class WalletSave
{
    [Serializable]
    private class Data
    {
        public List<string> metals = new List<string>();
        public List<ulong>  amounts = new List<ulong>();
        public string version = "0.2";
    }

    private static string PathFull(string fileName = "wallet.json")
        => Path.Combine(Application.persistentDataPath, fileName);

    public static void Save(Wallet wallet, string fileName = "wallet.json")
    {
        if (wallet == null) return;

        var data = new Data();
        foreach (Metal m in Enum.GetValues(typeof(Metal)))
        {
            ulong v = wallet.Get(m);
            if (v == 0UL) continue;
            data.metals.Add(m.ToString());
            data.amounts.Add(v);
        }

        var json = JsonUtility.ToJson(data);
        File.WriteAllText(PathFull(fileName), json);
    #if UNITY_EDITOR
        Debug.Log($"[WalletSave] Saved → {PathFull(fileName)}");
    #endif
    }

    public static void LoadInto(Wallet wallet, string fileName = "wallet.json")
    {
        if (wallet == null) return;

        string path = PathFull(fileName);
        if (!File.Exists(path)) return;

        var json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<Data>(json);
        if (data == null || data.metals == null || data.amounts == null) return;

        int count = Math.Min(data.metals.Count, data.amounts.Count);
        for (int i = 0; i < count; i++)
        {
            if (Enum.TryParse<Metal>(data.metals[i], out var m))
            {
                wallet.Set(m, data.amounts[i]);
            }
        }
    #if UNITY_EDITOR
        Debug.Log($"[WalletSave] Loaded ← {path} (set semantics)");
    #endif
    }
}
