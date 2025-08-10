using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
class WalletSaveDto
{
    public List<string> metals = new();
    public List<ulong> amounts = new();
}

public static class WalletSave
{
    static string PathFile => System.IO.Path.Combine(Application.persistentDataPath, "wallet.json");

    public static void Save(Wallet w)
    {
        var dto = new WalletSaveDto();
        foreach (Metal m in Enum.GetValues(typeof(Metal)))
        {
            var amt = w.Get(m);
            if (amt > 0) { dto.metals.Add(m.ToString()); dto.amounts.Add(amt); }
        }
        File.WriteAllText(PathFile, JsonUtility.ToJson(dto));
    }

    public static void LoadInto(Wallet w)
    {
        if (!File.Exists(PathFile)) return;
        var dto = JsonUtility.FromJson<WalletSaveDto>(File.ReadAllText(PathFile));
        if (dto?.metals == null) return;

        for (int i = 0; i < dto.metals.Count; i++)
            if (Enum.TryParse<Metal>(dto.metals[i], out var m)) w.Add(m, dto.amounts[i]);
    }
}
