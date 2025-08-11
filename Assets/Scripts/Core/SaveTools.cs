using System;
using System.IO;
using UnityEngine;

namespace IdleRPG.Core
{
    public class SaveTools : MonoBehaviour
    {
        [SerializeField] private Wallet wallet;
        [SerializeField] private string fileName = "wallet.json";

        private string PathFull => Path.Combine(Application.persistentDataPath, fileName);

        [ContextMenu("Save Now")]
        public void SaveNow() => WalletSave.Save(wallet);

        [ContextMenu("Load Now (fresh)")]
        public void LoadFresh()
        {
            // zero in-memory first to avoid additive double-count
            wallet.ClearAll();          // see tiny addition below if you don't have this yet
            WalletSave.LoadInto(wallet);
        }

        [ContextMenu("Reset Save")]
        public void ResetSave()
        {
            wallet.ClearAll();          // zero in-memory
            if (File.Exists(PathFull))  // delete file
                File.Delete(PathFull);
            WalletSave.Save(wallet);    // write a clean zeroed file (optional but nice)
#if UNITY_EDITOR
            Debug.Log($"[SaveTools] Reset → {PathFull}");
#endif
        }
    }
}
