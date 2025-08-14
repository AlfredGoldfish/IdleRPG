using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using IdleRPG.Core;

/// <summary>
/// Minimal JSON persistence for Wallet (string key + ulong amount).
/// </summary>
public static class WalletSave
{
    [Serializable]
    public class Entry
    {
        public string metal;
        public ulong amount;
    }

    [Serializable]
    public class Data
    {
        public List<Entry> entries = new List<Entry>();
    }

    private static string FilePath => Path.Combine(Application.persistentDataPath, "wallet.json");

    public static void Save(Wallet wallet)
    {
        if (wallet == null) return;
        var data = new Data();
        foreach (var kv in wallet.Enumerate())
        {
            data.entries.Add(new Entry { metal = kv.Key, amount = kv.Value });
        }
        var json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(FilePath, json);
#if UNITY_EDITOR
        Debug.Log($"[WalletSave] Saved {data.entries.Count} entries to {FilePath}");
#endif
    }

    public static bool TryLoad(out Data data)
    {
        data = null;
        if (!File.Exists(FilePath)) return false;
        try
        {
            var json = File.ReadAllText(FilePath);
            data = JsonUtility.FromJson<Data>(json);
            if (data == null) data = new Data();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[WalletSave] Load failed: {e.Message}");
            data = new Data();
            return false;
        }
    }

    public static void LoadInto(Wallet wallet)
    {
        if (wallet == null) return;
        if (TryLoad(out var data) && data != null)
        {
            foreach (var e in data.entries)
            {
                if (string.IsNullOrWhiteSpace(e.metal)) continue;
                wallet.Set(e.metal, e.amount);
            }
        }
#if UNITY_EDITOR
        Debug.Log("[WalletSave] LoadInto complete.");
#endif
    }

    public static void Clear()
    {
        try
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
#if UNITY_EDITOR
            Debug.Log("[WalletSave] Cleared file.");
#endif
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[WalletSave] Clear failed: {e.Message}");
        }
    }
}
