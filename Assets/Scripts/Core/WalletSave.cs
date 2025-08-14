using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Minimal JSON save/load for Wallet data. Global namespace for drop-in compatibility.
/// File: {persistentDataPath}/wallet.json
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

    public static string FilePath =>
        Path.Combine(Application.persistentDataPath, "wallet.json");

    public static void Save(IdleRPG.Core.Wallet wallet)
    {
        var data = new Data();
        foreach (var name in Enum.GetNames(typeof(Metal)))
        {
            var val = wallet.Get(name);
            if (val > 0)
            {
                data.entries.Add(new Entry { metal = name, amount = (ulong)val });
            }
        }
        var json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(FilePath, json);
#if UNITY_EDITOR
        Debug.Log($"WalletSave: wrote {data.entries.Count} entries to {FilePath}");
#endif
    }

    public static bool TryLoad(out Data data)
    {
        data = new Data();
        try
        {
            if (!File.Exists(FilePath)) return false;
            var json = File.ReadAllText(FilePath);
            var loaded = JsonUtility.FromJson<Data>(json);
            if (loaded != null && loaded.entries != null) data = loaded;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"WalletSave: load failed: {e}");
            return false;
        }
    }

    public static void Clear()
    {
        try
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
#if UNITY_EDITOR
            Debug.Log("WalletSave: cleared file");
#endif
        }
        catch (Exception e)
        {
            Debug.LogWarning($"WalletSave: clear failed: {e}");
        }
    }
}
