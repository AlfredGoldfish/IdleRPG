using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IdleRPG.Core
{
    public static class SaveManager
    {
        [Serializable] private class Entry { public string id; public ulong amount; }
        [Serializable] private class Data { public string version = "1"; public List<Entry> currencies = new List<Entry>(); }

        private static string PathFull(string fileName) => Path.Combine(Application.persistentDataPath, fileName);

        public static void Save(Wallet wallet, string fileName = "wallet.json")
        {
            if (wallet == null) return;
            var data = new Data();
            foreach (var kv in wallet.Enumerate())
            {
                if (kv.Value == 0UL) continue;
                data.currencies.Add(new Entry { id = kv.Key, amount = kv.Value });
            }
            var json = JsonUtility.ToJson(data);
            File.WriteAllText(PathFull(fileName), json);
#if UNITY_EDITOR
            Debug.Log($"[SaveManager] Saved → {PathFull(fileName)}");
#endif
        }

        public static void LoadInto(Wallet wallet, string fileName = "wallet.json")
        {
            if (wallet == null) return;
            var path = PathFull(fileName);
            if (!File.Exists(path)) return;
            var json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<Data>(json);
            if (data == null || data.currencies == null) return;
            foreach (var e in data.currencies)
            {
                if (string.IsNullOrEmpty(e.id)) continue;
                wallet.Set(e.id, e.amount);
            }
#if UNITY_EDITOR
            Debug.Log($"[SaveManager] Loaded ← {path} (set semantics)");
#endif
        }

        public static void ResetAndSave(Wallet wallet, string fileName = "wallet.json")
        {
            if (wallet == null) return;
            wallet.ClearAll();
            var path = PathFull(fileName);
            if (File.Exists(path)) File.Delete(path);
            Save(wallet, fileName);
#if UNITY_EDITOR
            Debug.Log($"[SaveManager] Reset save at {PathFull(fileName)}");
#endif
        }
    }
}
